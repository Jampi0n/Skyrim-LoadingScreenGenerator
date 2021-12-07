using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;

namespace LoadScreenGen.Settings {
    public class UserSettings {
        [SynthesisSettingName("Screen Width")]
        public int screenWidth { get; set; } = 16;

        [SynthesisSettingName("Screen Height")]
        public int screenHeight { get; set; } = 9;

        [SynthesisSettingName("Image Resolution")]
        public int imageResolution { get; set; } = 2048;

        [SynthesisSettingName("Frequency")]
        public int frequency { get; set; } = 100;

        [SynthesisSettingName("Border Option")]
        public BorderOption borderOption { get; set; } = BorderOption.Black;

        [SynthesisSettingName("Loading Screen Choice")]
        public LoadScreenChoice loadScreenChoice { get; set; } = LoadScreenChoice.Mcm;


        public string defaultModFolder { get; } = "JLoadScreens";
        public string defaultPrefix { get; } = "JLS_";
        public string defaultPluginName { get; } = "JLoadScreens.esp";
    }
}
