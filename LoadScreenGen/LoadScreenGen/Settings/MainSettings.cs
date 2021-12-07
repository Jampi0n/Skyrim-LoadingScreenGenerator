using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;

namespace LoadScreenGen.Settings {

    public class MainSettings {

        [SynthesisSettingName("Source Path")]
        public string sourcePath = "";

        [SynthesisSettingName("Include sub directories")]
        public bool includeSubDirs  = true;

        [SynthesisSettingName("Debug Mode")]
        public bool debugMode = false;


        [SynthesisSettingName("User Settings (Generate Loading Screens for you)")]
        public UserSettings userSettings = new();

        [SynthesisSettingName("Author Settings (Create Loading Screen Mod with FOMOD installer)")]
        public AuthorSettings authorSettings = new();
    }
}
