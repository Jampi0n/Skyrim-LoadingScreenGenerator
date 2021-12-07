using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;

namespace LoadScreenGen.Settings {

    public class BorderSettings {
        public bool includeNormal = true;
        public bool includeCrop = true;
        public bool includeFixedHeight = true;
        public bool includeFixedWidth = true;
        public bool includeStretch = true;
    }
    public class ResolutionSettings {
        [SynthesisTooltip("Additionally creates 4K textures (2K option is always included).")]
        public bool fourK = true;
        [SynthesisTooltip("Additionally creates 8K textures (2K option is always included).")]
        public bool eightK = false;
    }

    public class AuthorSettings {
        [SynthesisTooltip("Creates a FOMOD installer with many options.")]
        public bool enableAuthorMode = false;

        [SynthesisTooltip("Must be a valid file name. Part of the archive file name and visible in the FOMOD installer.")]
        public string modName = "Nazeem's Loading Screen Mod";
        [SynthesisTooltip("Part of the archive file name and visible in the FOMOD installer.")]
        public string modVersion = "1.0.0";
        [SynthesisTooltip("Visible in the FOMOD installer and the plugin header.")]
        public string modAuthor = "Nazeem";
        [SynthesisTooltip("Visible in the FOMOD installer.")]
        public string modLink = "https://www.nexusmods.com/skyrimspecialedition/mods/36556";
        [SynthesisTooltip("Visible in the plugin header.")]
        public string modDescription = "Adds beautiful loading screens painted by the glorious Nazeem.";

        [SynthesisTooltip("Name of the plugin file.")]
        public string pluginName = "NazeemsLoadingScreenMod.esp";
        [SynthesisTooltip("Prefix for all new records ins the plugin file.")]
        public string pluginPrefix = "Nzm_";

        [SynthesisTooltip("Sub folder used for the mod: \"meshes\\Mod Folder\" and \"textures\\Mod Folder\". It is recommended to use a unique mod folder to avoid conflicts with other mods.")]
        public string modFolder = "NazeemLoadScreens";

        public BorderSettings borderSettings = new();

        public ResolutionSettings resolutionSettings = new();

        [SynthesisTooltip("List of target aspect ratios. e.g. \"4:3,16:10,16:9,21:9\"")]
        public string aspectRatios = "4:3,16:10,16:9,21:9";
    }
}
