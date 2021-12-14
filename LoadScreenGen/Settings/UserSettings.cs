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
        public string sourcePath = "";

        [SynthesisSettingName("Include sub directories")]
        public bool includeSubDirs = true;

        [SynthesisSettingName("Aspect Ratio: Width")]
        [SynthesisTooltip("If you have a 16:9 screen, put 16 here.")]
        public int screenWidth = 16;

        [SynthesisSettingName("Aspect Ratio: Height")]
        [SynthesisTooltip("If you have a 16:9 screen, put 9 here.")]
        public int screenHeight = 9;

        [SynthesisSettingName("Image Resolution")]
        [SynthesisTooltip("The image resolution of the loading screen texture. It should be larger than your screen resolution.")]
        public int imageResolution = 2048;

        [SynthesisSettingName("Loading Screen Choice")]
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityCombined)]
        public LoadingScreenPriority loadScreenPriority = LoadingScreenPriority.Mcm;

        [SynthesisSettingName("Frequency")]
        [SynthesisTooltip("How frequently loading screens are used. Only applies to the loading screen choices MCM and Frequency.")]
        public int frequency = 100;

        [SynthesisSettingName("Border Option")]
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionCombined)]
        public BorderOption borderOption = BorderOption.Normal;

        public bool includeLoadingScreenText = true;


        public string defaultModFolder { get; } = "JLoadScreens";
        public string defaultPrefix { get; } = "JLS_";
        public string defaultPluginName { get; } = "JLoadScreens.esp";
    }
}
