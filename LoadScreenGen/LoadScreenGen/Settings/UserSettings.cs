using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace LoadScreenGen.Settings {
    /*public enum TextureResolution : int{
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192
    }*/

    public class UserSettings {
        [SynthesisSettingName("Source Path")]
        public string sourcePath = "";

        [SynthesisSettingName("Include sub directories")]
        public bool includeSubDirs = true;

        [SynthesisSettingName("Aspect Ratio: Width")]
        [SynthesisTooltip("If you have a 16:9 screen, put 16 here.")]
        public int screenWidth { get; set; } = 16;

        [SynthesisSettingName("Aspect Ratio: Height")]
        [SynthesisTooltip("If you have a 16:9 screen, put 9 here.")]
        public int screenHeight { get; set; } = 9;

        [SynthesisSettingName("Image Resolution")]
        [SynthesisTooltip("The image resolution of the loading screen texture. It should be larger than your screen resolution.")]
        public int imageResolution { get; set; } = 2048;

        [SynthesisSettingName("Loading Screen Choice")]
        [SynthesisTooltip("Determines when the loading screens will be used.\nStandalone: Loading screens will be added and used alongside vanilla loading screens.\nReplacer: Loading screens will be prioritized over vanilla loading screens.\nFrequency: Loading screens appear at the given frequency instead of vanilla loading screens.\nMcm: The given frequency can additionally be configured in a MCM.\nDebug: The loading screen that appears depends on a global variable. Change the variable in the in-game console to force a certain loading screen.")]
        public LoadScreenChoice loadScreenChoice { get; set; } = LoadScreenChoice.Mcm;

        [SynthesisSettingName("Frequency")]
        [SynthesisTooltip("How frequently loading screens are used. Only applies to the loading screen choices MCM and Frequency.")]
        public int frequency { get; set; } = 100;

        [SynthesisSettingName("Border Option")]
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\nNormal: The image is extended with black to fit the screen. The image will be fully visible.\nCrop: The image is cropped to fit the screen. Parts of the image will be hidden.\nFixedHeight: Image will be fit to the screen height. On the sides, the image is cropped or extended with black to fit the screen.\nFixedWidth: Image will be fit to the screen width. On the top and bottom, the image is cropped or extended with black to fit the screen.\nStretch: Image will be distorted to fill the entire screen.")]
        public BorderOption borderOption { get; set; } = BorderOption.Normal;

        public bool includeLoadingScreenText = true;


        public string defaultModFolder { get; } = "JLoadScreens";
        public string defaultPrefix { get; } = "JLS_";
        public string defaultPluginName { get; } = "JLoadScreens.esp";
    }
}
