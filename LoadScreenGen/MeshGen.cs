using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using System.IO;
using LoadScreenGen.Settings;

namespace LoadScreenGen {

    public static class MeshGen {
        const double sourceUpperWidth = 44.8;
        const double sourceHeight = 27.7;
        const double sourceOffsetX = 2.5;
        const double sourceRatio = 1.6;

        static double heightFactor = 0;
        static double widthFactor = 0;
        static double yOffset = 0;

        public static void FitToDisplayRatio(double displayRatio, double imageRatio, BorderOption borderOption) {
            // In the first part, the factors are adjusted, so the model fills the entire screen.
            // A width of 1.0 means the entire width of the image is visible on the screen, so width stays at 1.
            // For wider screens (ratioFactor > 1.0), the height is reduced.
            // Likewise for slimmer screens (ratioFactor < 1.0), the height is increased.
            double ratioFactor = displayRatio / sourceRatio;
            double width = 1.0;
            double height = 1.0 / ratioFactor;

            // Now the model fills the entire screen.
            // In order to keep the aspect ratio of the image, the model must be modified.
            // Here, the model only becomes smaller, in order to add black bars.

            if(borderOption != BorderOption.Stretch) {
                if(displayRatio > imageRatio) {
                    if(borderOption == BorderOption.FixedWidth) {
                        height *= displayRatio / imageRatio;
                    } else if(borderOption == BorderOption.FixedHeight) {
                        width = width * imageRatio / displayRatio;
                    } else if(borderOption == BorderOption.Crop) {
                        height = height * displayRatio / imageRatio;
                    } else if(borderOption == BorderOption.Normal) {
                        width = width * imageRatio / displayRatio;
                    }
                } else if(displayRatio < imageRatio) {
                    if(borderOption == BorderOption.FixedWidth) {
                        height = height * displayRatio / imageRatio;
                    } else if(borderOption == BorderOption.FixedHeight) {
                        width = width * imageRatio / displayRatio;
                    } else if(borderOption == BorderOption.Crop) {
                        width = width * imageRatio / displayRatio;
                    } else if(borderOption == BorderOption.Normal) {
                        height = height * displayRatio / imageRatio;
                    }
                }
            }

            // Write result.
            widthFactor = width;
            heightFactor = height;
        }

        public static void CreateMeshes(List<Image> imageList, string targetDirectory, string textureDirectory, string templatePath, double displayRatio, BorderOption borderOption, IniCompatibilitySettings iniCompatibilitySettings) {
            Logger.DebugMsg("CreateMeshes(List<Image>(" + imageList.Count + "), " + targetDirectory + ", " + textureDirectory + ", " + templatePath + ", " + displayRatio + ", " + borderOption + ");");
            try {
                _ = new NifFile();
            } catch(Exception) {
                Logger.Log("Failed to create nif file.");
                throw;
            }
            var templateNif = new NifFile();
            if(!File.Exists(templatePath)) {
                throw new IOException("Nif template does not exist at path: " + templatePath);
            }
            try {
                templateNif.Load(templatePath);
            } catch(Exception) {
                Logger.Log("Failed to load template nif file.");
                throw;
            }
            int i = 0;
            foreach(var image in imageList) {
                var imagePath = image.skyrimPath;
                var newNif = new NifFile(templateNif);
                NiShape shape = newNif.GetShapes()[0];
                if(shape != null) {
                    newNif.SetTextureSlot(shape, Path.Combine(textureDirectory, imagePath + ".dds"));
                    var imageRatio = image.width * 1.0 / image.height;
                    FitToDisplayRatio(displayRatio, imageRatio, borderOption);

                    var verts = newNif.GetVertsForShape(shape);
                    if(borderOption == BorderOption.Normal) {
                        if(imageRatio > displayRatio) {
                            yOffset = 0.37;
                        } else {
                            yOffset = -0.108 * displayRatio + 0.622;
                        }
                    } else if(borderOption == BorderOption.Crop) {
                        yOffset = -0.108 * displayRatio + 0.622;
                    } else if(borderOption == BorderOption.FixedHeight) {
                        yOffset = -0.108 * displayRatio + 0.622;
                    } else if(borderOption == BorderOption.FixedWidth) {
                        if(imageRatio > displayRatio) {
                            yOffset = 0.37;
                        } else {
                            yOffset = -0.108 * displayRatio + 0.622;
                        }
                    } else {
                        yOffset = -0.108 * displayRatio + 0.622;
                    }

                    var fovScale = (iniCompatibilitySettings.fUIMistMenu_CameraFOV_G - 3) / 72.0;
                    var iniX = iniCompatibilitySettings.fUIAltLogoModel_TranslateX_G * 0.5;
                    var iniY = -iniCompatibilitySettings.fUIAltLogoModel_TranslateZ_G * 0.5;
                    widthFactor *= fovScale;
                    heightFactor *= fovScale;

                    // Top Left
                    verts[0].x = (float)(sourceOffsetX - sourceUpperWidth * widthFactor + iniX);
                    verts[0].y = (float)(yOffset + sourceHeight * heightFactor + iniY);

                    // Bottom Left
                    verts[1].x = (float)(sourceOffsetX - sourceUpperWidth * widthFactor + iniX);
                    verts[1].y = (float)(yOffset - sourceHeight * heightFactor + iniY);
                    verts[1].z = (float)(8.0 * heightFactor);

                    // Bottom Right
                    verts[2].x = (float)(sourceOffsetX + sourceUpperWidth * widthFactor + iniX);
                    verts[2].y = (float)(yOffset - sourceHeight * heightFactor + iniY);
                    verts[2].z = (float)(8.0 * heightFactor);

                    // Top Right
                    verts[3].x = (float)(sourceOffsetX + sourceUpperWidth * widthFactor + iniX);
                    verts[3].y = (float)(yOffset + sourceHeight * heightFactor + iniY);

                    newNif.SetVertsForShape(shape, verts);
                } else {
                    throw new IOException("Nif template is invalid.");
                }
                var savePath = Path.Combine(targetDirectory, image.skyrimPath + ".nif");
                Directory.CreateDirectory(Path.Combine(savePath, ".."));
                newNif.Save(savePath);
                if(!File.Exists(savePath)) {
                    throw new IOException("Failed to save nif file at path: " + savePath);
                } else {
                    Logger.DebugMsg("Saved nif file at path: " + savePath);
                }
                shape?.Dispose();
                newNif.Dispose();
                i++;
            }
            templateNif.Dispose();
        }
    }
}
