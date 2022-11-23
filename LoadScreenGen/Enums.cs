using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadScreenGen {

    public enum TextureCompression : int {
        BC1 = 1,
        BC7 = 7,
        Uncompressed = -1
    }

    public enum TextureCompressionLE : int{
        BC1 = TextureCompression.BC1,
        Uncompressed = TextureCompression.Uncompressed
    }

    public enum TextureCompressionSE : int{
        BC1 = TextureCompression.BC1,
        BC7 = TextureCompression.BC7,
        Uncompressed = TextureCompression.Uncompressed
    }

    public enum LoadingScreenText {
        Always,
        Never,
        Optional
    }

    public enum TargetRelease {
        LE_and_SE,
        LE_Only,
        SE_Only,
    }

    public enum BorderOption {
        Normal,
        Crop,
        FixedHeight,
        FixedWidth,
        Stretch,
    }

    public enum LoadingScreenPriority {
        Standalone,
        Replacer,
        Frequency,
        Mcm,
        Debug
    }

    public enum TextureResolutionOption {
        MainArchive,
        ExtraArchive,
        None
    }

    public class NamedIniCompatibilitySettings {
        public string name = "";
        public IniCompatibilitySettings iniCompatibilitySettings = new();
    }

    public class IniCompatibilitySettings {
        [SynthesisSettingName("fUIMistMenu_CameraFOV_G")]
        public float fUIMistMenu_CameraFOV_G = 75;
        [SynthesisSettingName("fUIAltLogoModel_TranslateX_G")]
        public float fUIAltLogoModel_TranslateX_G = 0;
        [SynthesisSettingName("fUIAltLogoModel_TranslateZ_G")]
        public float fUIAltLogoModel_TranslateZ_G = 0;
    }

    public static class Enums {
        public const string compressionBC1 = "Low file size, but lower quality. More noticeable on images with different shades of dark colors.";
        public const string compressionBC7 = "Twice the file size as BC1, but much better quality. Almost the same quality as uncompressed. Highly recommended.";
        public const string compressionUncompressed = "Six times the file size as BC1. Best quality.";

        public const string compressionLE = compressionBC1 + "\n" + compressionUncompressed;
        public const string compressionSE = compressionBC1 + "\n" + compressionBC7 + "\n" + compressionUncompressed;

        public const string textureResolutionMainArchive = "Main Archive: Includes the textures in the main FOMOD installer.";
        public const string textureResolutionExtraArchive = "Extra Archive: Generates a separate archive.";
        public const string textureResolutionNone = "None: Generates no textures. Much faster.";
        public const string textureResolutionCombined = textureResolutionMainArchive + "\n" + textureResolutionExtraArchive + "\n" + textureResolutionNone;

        public const string borderOptionNormal = "Normal: The image is extended with black to fit the screen. The image will be fully visible.";
        public const string borderOptionCrop = "Crop: The image is cropped to fit the screen. Parts of the image will be hidden.";
        public const string borderOptionFixedHeight = "FixedHeight: Image will be fit to the screen height. On the sides, the image is cropped or extended with black to fit the screen.";
        public const string borderOptionFixedWidth = "FixedWidth: Image will be fit to the screen width. On the top and bottom, the image is cropped or extended with black to fit the screen.";
        public const string borderOptionStretch = "Stretch: Image will be distorted to fill the entire screen.";

        public const string borderOptionCombined = borderOptionNormal + "\n" + borderOptionCrop + "\n" + borderOptionFixedHeight + "\n" + borderOptionFixedWidth + "\n" + borderOptionStretch;

        public static string ToDescription(this BorderOption borderOption) {
            return borderOption switch {
                BorderOption.Normal => borderOptionNormal,
                BorderOption.Crop => borderOptionCrop,
                BorderOption.FixedHeight => borderOptionFixedHeight,
                BorderOption.FixedWidth => borderOptionFixedWidth,
                BorderOption.Stretch => borderOptionStretch,
                _ => "",
            };
        }

        public const string loadingScreenPriorityStandalone = "Standalone: Loading screens will be added and used alongside vanilla loading screens.";
        public const string loadingScreenPriorityReplacer = "Replacer: Loading screens will be prioritized over vanilla loading screens.";
        public const string loadingScreenPriorityFrequency = "Frequency: Loading screens appear at a certain frequency instead of vanilla loading screens.";
        public const string loadingScreenPriorityMcm = "Mcm: Loading screens appear at a certain frequency instead of vanilla loading screens. The frequency can be configured in a Mod Configuration Menu.";
        public const string loadingScreenPriorityDebug = "Debug: The loading screen that appears depends on a global variable. Change the variable in the in-game console to force a certain loading screen.";

        public const string loadingScreenPriorityCombined = loadingScreenPriorityStandalone + "\n" + loadingScreenPriorityReplacer + "\n" + loadingScreenPriorityFrequency + "\n" + loadingScreenPriorityMcm + "\n" + loadingScreenPriorityDebug;

        public static string ToDescription(this LoadingScreenPriority borderOption) {
            return borderOption switch {
                LoadingScreenPriority.Standalone => loadingScreenPriorityStandalone,
                LoadingScreenPriority.Replacer => loadingScreenPriorityReplacer,
                LoadingScreenPriority.Frequency => loadingScreenPriorityFrequency,
                LoadingScreenPriority.Mcm => loadingScreenPriorityMcm,
                LoadingScreenPriority.Debug => loadingScreenPriorityDebug,
                _ => "",
            };
        }

    }


}
