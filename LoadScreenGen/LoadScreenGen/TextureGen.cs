using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

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

        public Image(string path) {
            this.path = path;
            this.skyrimPath = Path.GetRelativePath(Program.Settings.sourcePath, Path.ChangeExtension(path, ""));
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
        static string ShellExecuteWait(string fileName, string args) {
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
        public static Image[] ProcessTextures(string sourceDirectory, string[] targetDirectory, int[] imageResolution) {
            var imageList = new List<string>();
            SearchOption searchOption = Program.Settings.includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            // Add all image files to a list.
            Logger.Log("Scanning source directory for valid source images...\n");

            imageList.AddRange(Directory.GetFiles(sourceDirectory, "*.dds", searchOption));
            imageList.AddRange(Directory.GetFiles(sourceDirectory, "*.png", searchOption));
            imageList.AddRange(Directory.GetFiles(sourceDirectory, "*.jpg", searchOption));

            var imageCount = imageList.Count;
            var imageArray = new Image[imageCount];
            int i = 0;
            foreach(var imagePath in imageList) {

                imageArray[i] = new Image(imagePath);
                i++;
            }

            Logger.Log(imageCount + " images found in the source directory.");

            var numResolutions = Math.Min(targetDirectory.Length, imageResolution.Length);
            var uniquePaths = new HashSet<string>();

            Logger.Log("	Creating textures from source images...");
            for(i = 0; i < imageCount; i += 1) {
                var image = imageArray[i];

                string s = Path.GetFileNameWithoutExtension(image.path);
                Logger.Log("	" + (i + 1) + "/" + imageCount + ": " + s);
                if(!uniquePaths.Contains(s)) {

                    uniquePaths.Add(s);

                    bool srgb;
                    string srgbCmd = "";

                    // use texdiag to read input format

                    string texInfo = ShellExecuteWait(Path.Combine(Program.extraDataPath, "DirectXTex", "texdiag.exe"), "info \"" + image.path + "\" -nologo");
                    srgb = texInfo.Contains("SRGB");
                    var lines = texInfo.Split("\n");
                    image.width = int.Parse(ParseTexDiagOutput(lines[1]));
                    image.height = int.Parse(ParseTexDiagOutput(lines[2]));
                    var textFile = Path.ChangeExtension(image.path, ".txt");
                    if(File.Exists(textFile)) {
                        image.text = File.ReadAllText(textFile);
                    }
                    if(srgb) {
                        srgbCmd = "-srgb ";
                    }
                    for(int j = 0; j < numResolutions; ++j) {
                        int resolution = imageResolution[j];
                        // Execute texconv.exe (timeout = 10 seconds)
                        Directory.CreateDirectory(targetDirectory[j]);
                        string args = "-f BC1_UNORM " + srgbCmd + "-o \"" + targetDirectory[j] + "\" -y -w " + resolution + " -h " + resolution + " \"" + image.path + "\"";
                        ShellExecuteWait(Path.Combine(Program.extraDataPath, "DirectXTex", "texconv.exe"), args);
                    }
                }

            }
            return imageArray;
        }
    }
}
