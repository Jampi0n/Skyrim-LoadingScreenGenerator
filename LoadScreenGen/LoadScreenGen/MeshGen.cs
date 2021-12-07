using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using nifly;
using System.IO;
using LoadScreenGen.Settings;

namespace LoadScreenGen {
    public enum BorderOption {
        Normal,
        Crop,
        FixedHeight,
        FixedWidth,
        Stretch,
    }

    public static class MeshGen {
        const double sourceUpperWidth = 45.5;
        const double sourceLowerWidth = 1.1;
        const double sourceHeightOffset = 1.0;
        const double sourceHeight = 29.0;
        const double sourceOffsetX = 2.5;
        const double sourceOffsetY = 0.65;
        const double sourceRatio = 1.6;

        static double heightFactor = 0;
        static double widthFactor = 0;

        public static void FitToDisplayRatio(double displayRatio, double imageRatio, BorderOption borderOption) {
            // In the first part, the factors are adjusted, so the model fills the entire screen.
            // A width of 1.0 means the entire width of the image is visible on the screen, so width stays at 1.
            // For wider screens (ratioFactor > 1.0), the height is reduced.
            // Likewise for slimmer screens (ratioFactor < 1.0), the height is increased.
            double ratioFactor = displayRatio / sourceRatio;
            double width = 1.0;
            double height = 1.0 / ratioFactor;

            Logger.Log("FitToDisplayRatio");
            Logger.Log("" + displayRatio);
            Logger.Log("" + imageRatio);

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
            Logger.Log(""+width);
            Logger.Log(""+height);
        }

        public static void CreateMeshes(List<Image> imageList, string targetDirectory, string textureDirectory, string templatePath, double displayRatio, BorderOption borderOption) {
            /*var templateNif = new NifFile();
            templateNif.Load(templatePath);
            int i = 0;
            int n = imageList.Count;
            foreach(var image in imageList) {
                var imagePath = image.skyrimPath;
                Console.WriteLine("	" + (i + 1) + "/" + n + ": " + Path.Combine(targetDirectory, imagePath + ".nif"));

                var newNif = new NifFile(templateNif);

                NiShape shape = newNif.GetShapes()[0];
                if(shape != null) {
                    newNif.SetTextureSlot(shape, Path.Combine(textureDirectory, imagePath + ".dds"));
                    FitToDisplayRatio(displayRatio, image.width * 1.0 / image.height, borderOption);
                    var geo = shape.GetGeomData();

                    // Top Left

                    geo.vertices[0].x = (float)(sourceOffsetX - sourceUpperWidth * widthFactor);
                    geo.vertices[0].y = (float)(sourceOffsetY + sourceHeight * heightFactor - sourceHeightOffset * heightFactor);

                    // Bottom Left
                    geo.vertices[1].x = (float)(sourceOffsetX - sourceUpperWidth * widthFactor - sourceLowerWidth * widthFactor * heightFactor);
                    geo.vertices[1].y = (float)(sourceOffsetY - sourceHeight * heightFactor - sourceHeightOffset * heightFactor);

                    // Bottom Right
                    geo.vertices[2].x = (float)(sourceOffsetX + sourceUpperWidth * widthFactor + sourceLowerWidth * widthFactor * heightFactor);
                    geo.vertices[2].y = (float)(sourceOffsetY - sourceHeight * heightFactor - sourceHeightOffset * heightFactor);

                    // Top Right
                    geo.vertices[3].x = (float)(sourceOffsetX + sourceUpperWidth * widthFactor);
                    geo.vertices[3].y = (float)(sourceOffsetY + sourceHeight * heightFactor - sourceHeightOffset * heightFactor);

                    shape.SetGeomData(geo);
                    geo.Dispose();
                }

                var savePath = Path.Combine(targetDirectory, image.skyrimPath + ".nif");
                newNif.Save(savePath);
                shape?.Dispose();
                newNif.Dispose();
                i++;
            }
            templateNif.Dispose();*/
        }
    }
}
