using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace LoadScreenGen.Settings {

    public class UserSettings {
        [SynthesisSettingName("Source Path")]
        [SynthesisTooltip("Images from this directory are used to create the loading screens.")]
        public string SourcePath { set; get; } = "";

        [SynthesisSettingName("Include sub directories")]
        public bool IncludeSubDirs { set; get; } = true;

        [SynthesisSettingName("Aspect Ratio: Width")]
        [SynthesisTooltip("If you have a 16:9 screen, put 16 here.")]
        public int ScreenWidth { set; get; } = 16;

        [SynthesisSettingName("Aspect Ratio: Height")]
        [SynthesisTooltip("If you have a 16:9 screen, put 9 here.")]
        public int ScreenHeight { set; get; } = 9;

        [SynthesisSettingName("Image Resolution")]
        [SynthesisTooltip("The image resolution of the loading screen texture. Allowed values: 1024, 2048, 4096, 8192. Larger textures take much longer. For best quality, pick the lowest resolution that is not smaller than your screen resolution.")]
        public int ImageResolution { set; get; } = 2048;

        [SynthesisSettingName("Loading Screen Priority")]
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityCombined)]
        public LoadingScreenPriority LoadScreenPriority { set; get; } = LoadingScreenPriority.Standalone;

        [SynthesisSettingName("Frequency")]
        [SynthesisTooltip("How frequently loading screens are used. Only applies to the loading screen choices MCM and Frequency.")]
        public int Frequency { set; get; } = 100;

        [SynthesisSettingName("Border Option")]
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionCombined)]
        public BorderOption BorderOption { set; get; } = BorderOption.Normal;

        public bool IncludeLoadingScreenText { set; get; } = true;


        [SynthesisSettingName("Texture Compression LE")]
        public TextureCompressionLE textureCompressionLE = TextureCompressionLE.BC1;

        [SynthesisSettingName("Texture Compression SE")]
        public TextureCompressionSE textureCompressionSE = TextureCompressionSE.BC7;

        public IniCompatibilitySettings IniCompatibilitySettings = new();

        public string DefaultModFolder { get; } = "JLoadScreens";
        public string DefaultPrefix { get; } = "JLS_";
        public string DefaultPluginName { get; } = "JLoadScreens.esp";
    }
}
