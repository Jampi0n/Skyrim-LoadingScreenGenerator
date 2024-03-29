using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LoadScreenGen {
    public class Image {
        /// <summary>
        /// Absolute path to the source image. e.g. "C:\Users\User\Pictures\Image.jpg"
        /// </summary>
        public string path = "";
        /// <summary>
        /// Path the image would have in skyrim. e.g. "Image"
        /// </summary>
        public string skyrimPath = "";
        /// <summary>
        /// Width of the source image in pixels.
        /// </summary>
        public int width;
        /// <summary>
        /// Height of the source image in pixels.
        /// </summary>
        public int height;
        /// <summary>
        /// Associated text of the source image.
        /// </summary>
        public string text;

        public Image(string path, string sourcePath) {
            this.path = path;
            this.skyrimPath = Path.GetRelativePath(sourcePath, Path.ChangeExtension(path, ""));
            this.width = 0;
            this.height = 0;
            this.text = "";
        }
    }


    public static class TextureGen {

        static string ParseTexDiagOutput(string output) {
            return output[16..];
        }
        /// <summary>
        /// Runs a process and returns the output.
        /// </summary>
        /// <param name="fileName">Program to run.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>Standard ouput of the program.</returns>
        public static string ShellExecuteWait(string fileName, string args) {
            var pProcess = new Process();
            pProcess.StartInfo.FileName = fileName;
            pProcess.StartInfo.Arguments = args;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
            return strOutput;
        }
        /// <summary>
        /// Searches for valid images in sourceDirectory and creates textures for them in the targetDirectorys for the given imageResolutions.
        /// </summary>
        /// <param name="sourceDirectory">The directory where image search starts.</param>
        /// <param name="targetDirectory">Array of target directories. Textures for the i-th image resolution will be created in the i-th target directory.</param>
        /// <param name="imageResolution">Array of image resolutions in pixels (e.g. 2048). Textures for the i-th image resolution will be created in the i-th target directory.</param>
        /// <returns>Array of images found.</returns>
        public static Image[] ProcessTextures(string sourceDirectory, string[] targetDirectory, int[] imageResolution, bool includeSubDirs, TextureCompression[] textureCompression) {
            Logger.DebugMsg("ProcessTextures(" + sourceDirectory + ", " + "[" + string.Join(",", targetDirectory) + "]" + ", " + "[" + string.Join(",", imageResolution) + "]" + ", " + includeSubDirs + ", " + "[" + string.Join(",", textureCompression) + "]" + ");");
            var imageList = new List<string>();
            SearchOption searchOption = includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            // Add all image files to a list.
            Logger.Log("Scanning source directory for valid source images...\n");

            imageList.AddRange(Directory.GetFiles(sourceDirectory, "*.dds", searchOption));
            imageList.AddRange(Directory.GetFiles(sourceDirectory, "*.png", searchOption));
            imageList.AddRange(Directory.GetFiles(sourceDirectory, "*.jpg", searchOption));

            var imageCount = imageList.Count;
            var imageArray = new Image[imageCount];
            int i = 0;
            foreach(var imagePath in imageList) {

                imageArray[i] = new Image(imagePath, sourceDirectory);
                i++;
            }

            var numResolutions = imageResolution.Length;
            if(numResolutions * textureCompression.Length != targetDirectory.Length) {
                throw new ArgumentException("The number of target directories must be equal the number of resolution times the number of texture compressions.");
            }

            var uniquePaths = new HashSet<string>();

            var uniqueImageList = new List<Image>();

            for(i = 0; i < imageCount; i += 1) {
                var image = imageArray[i];
                string s = image.skyrimPath;
                if(!uniquePaths.Contains(s)) {
                    uniqueImageList.Add(image);
                    uniquePaths.Add(s);
                } else {
                    Logger.Log("Warning: " + s + " exists multiple times with different extensions. Only one of them will be used!");
                }
            }
            imageArray = uniqueImageList.ToArray();
            imageCount = imageArray.Length;
            var imageException = new Exception?[imageCount];

            Logger.Log(imageCount + " valid images found in the source directory.");
            Logger.Log("	Creating textures from source images...");

            int progressCounter = 0;

            Parallel.For(0, imageCount, new ParallelOptions() {
                MaxDegreeOfParallelism = 8,
            }, (i) => {
                var image = imageArray[i];
                try {
                    string s = Path.GetFileNameWithoutExtension(image.path);
                    Logger.Log("	" + Interlocked.Increment(ref progressCounter) + "/" + imageCount + ": " + s);

                    bool srgb;
                    string srgbCmd = "";

                    // use texdiag to read input format

                    string texInfo = ShellExecuteWait(Path.Combine(Program.resourceDirectory, "DirectXTex", "texdiag.exe"), "info \"" + image.path + "\" -nologo");
                    srgb = texInfo.Contains("SRGB");
                    var lines = texInfo.Split("\n");
                    if(lines.Length < 2 || lines[0].Contains("FAILED")) {
                        throw new IOException("texdiag.exe failed to read the source image. Output: >\n" + texInfo + "\n<");
                    }
                    if(!lines[1].TrimStart().StartsWith("width = ") || !lines[2].TrimStart().StartsWith("height = ")) {
                        throw new IOException("texdiag.exe output has unexpected format. Output: >\n" + texInfo + "\n<");
                    }
                    image.width = int.Parse(ParseTexDiagOutput(lines[1]));
                    image.height = int.Parse(ParseTexDiagOutput(lines[2]));
                    var textFile = Path.ChangeExtension(image.path, ".txt");
                    if(File.Exists(textFile)) {
                        image.text = File.ReadAllText(textFile);
                    }
                    if(srgb) {
                        srgbCmd = "-srgb ";
                    }
                    for(int k = 0; k < textureCompression.Length; ++k) {
                        string format = textureCompression[k] switch {
                            TextureCompression.BC1 => "BC1_UNORM",
                            TextureCompression.BC7 => "BC7_UNORM",
                            TextureCompression.Uncompressed => "R8G8B8A8_UNORM",
                            _ => "R8G8B8A8_UNORM",
                        };
                        for(int j = numResolutions - 1; j >= 0; --j) {
                            int dirIndex = k * numResolutions + j;
                            var sourcePath = image.path;
                            if(j < numResolutions - 1) {
                                // if multiple resolutions are used, read from existing textures, if they are smaller than the source image
                                if(image.width * image.height > imageResolution[j + 1] * imageResolution[j + 1]) {
                                    sourcePath = Path.Combine(targetDirectory[dirIndex + 1], image.skyrimPath + ".dds");
                                }
                            }
                            int resolution = imageResolution[j];
                            var outPath = Path.Combine(targetDirectory[dirIndex], image.skyrimPath, "..");

                            Directory.CreateDirectory(outPath);
                            string args = "-f " + format + " " + srgbCmd + "-o \"" + outPath + "\" -y -w " + resolution + " -h " + resolution + " \"" + sourcePath + "\"";
                            var texConvOut = ShellExecuteWait(Path.Combine(Program.resourceDirectory, "DirectXTex", "texconv.exe"), args);
                            if(texConvOut.Contains("FAILED")) {
                                throw new IOException("texconv.exe failed:\n\t" + texConvOut);
                            }
                            if(!File.Exists(Path.Combine(targetDirectory[dirIndex], image.skyrimPath + ".dds"))) {
                                throw new IOException("Texture conversion failed. No texture file was created at the expected path: " + Path.Combine(targetDirectory[dirIndex], image.skyrimPath + ".dds"));
                            }
                        }
                    }
                } catch(Exception e) {
                    imageException[i] = e;
                }
            });
            List<Image> generatedImages = new();
            for(i = 0;i<imageCount;++i) {
                if(imageException[i] != null) {
                    Logger.Log("Warning: Failed to create all textures for image " + imageArray[i].path + " with message: " + imageException[i]!.Message);
                } else {
                    generatedImages.Add(imageArray[i]);
                }
            }

            return generatedImages.ToArray();
        }
    }
}
