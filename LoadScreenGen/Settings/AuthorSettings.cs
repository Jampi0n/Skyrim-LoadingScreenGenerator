using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;

namespace LoadScreenGen.Settings {


    public class BorderSettings {
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionNormal)]
        public bool includeNormal = true;
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionCrop)]
        public bool includeCrop = true;
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionFixedHeight)]
        public bool includeFixedHeight = true;
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionFixedWidth)]
        public bool includeFixedWidth = true;
        [SynthesisTooltip("How image borders are handled for images that differ from your display aspect ratio.\n" + Enums.borderOptionStretch)]
        public bool includeStretch = true;
        [SynthesisTooltip("Default border option for the fomod installer.")]
        public BorderOption defaultBorderOption = BorderOption.Normal;
    }

    public class PrioritySettings {
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityStandalone)]
        public bool includeStandalone = true;
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityReplacer)]
        public bool includeReplacer = true;
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityFrequency)]
        public bool includeFrequency = true;
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityMcm)]
        public bool includeMcm = true;
        [SynthesisTooltip("Determines when the loading screens will be used.\n" + Enums.loadingScreenPriorityDebug)]
        public bool includeDebug = false;
        [SynthesisTooltip("Default loading screen priority for the fomod installer.")]
        public LoadingScreenPriority defaultPrioritySetting = LoadingScreenPriority.Standalone;
    }

    public class ResolutionSettings {
        [SynthesisTooltip("Additionally creates 4K textures (2K option is always included). Takes much longer.")]
        public bool fourK = true;
        [SynthesisTooltip("Additionally creates 8K textures (2K option is always included). Takes much much longer. Not recommended.")]
        public bool eightK = false;
    }

    public class AuthorSettings {
        [SynthesisTooltip("Creates a FOMOD installer with many options.")]
        public bool EnableAuthorMode { set; get; } = false;

        [SynthesisTooltip("For which game release to create FOMOD installers. The SE version works with AE and VR.")]
        public TargetRelease TargetRelease { set; get; } = TargetRelease.LE_and_SE;

        [SynthesisSettingName("Source Path")]
        [SynthesisTooltip("Images from this directory are used to create the loading screens.")]
        public string SourcePath { set; get; } = "";

        [SynthesisSettingName("Include sub directories")]
        public bool IncludeSubDirs { set; get; } = true;

        [SynthesisTooltip("Must be a valid file name. Part of the archive file name and visible in the FOMOD installer.")]
        public string ModName { set; get; } = "Nazeem's Loading Screen Mod";

        [SynthesisTooltip("If the MCM option is used, this will be the name of the MCM. Keep it short.")]
        public string McmName { set; get; } = "Nazeem's Loading Screen Mod";

        [SynthesisTooltip("Part of the archive file name and visible in the FOMOD installer.")]
        public string ModVersion { set; get; } = "1.0.0";

        [SynthesisTooltip("Visible in the FOMOD installer and the plugin header.")]
        public string ModAuthor { set; get; } = "Nazeem";

        [SynthesisTooltip("Visible in the FOMOD installer.")]
        public string ModLinkLE { set; get; } = "https://www.nexusmods.com/skyrimspecialedition/mods/36556";

        [SynthesisTooltip("Visible in the FOMOD installer.")]
        public string ModLinkSE { set; get; } = "https://www.nexusmods.com/skyrimspecialedition/mods/36556";

        [SynthesisTooltip("Visible in the plugin header.")]
        public string ModDescription { set; get; } = "Adds beautiful loading screens painted by the glorious Nazeem.";

        [SynthesisTooltip("Name of the plugin file.")]
        public string PluginName { set; get; } = "NazeemsLoadingScreenMod.esp";

        [SynthesisTooltip("Prefix for all new records ins the plugin file.")]
        public string PluginPrefix { set; get; } = "Nzm_";

        [SynthesisTooltip("Sub folder used for the mod: \"meshes\\Mod Folder\" and \"textures\\Mod Folder\". It is recommended to use a unique mod folder to avoid conflicts with other mods.")]
        public string ModFolder { set; get; } = "NazeemLoadScreens";

        public BorderSettings borderSettings = new();

        public ResolutionSettings resolutionSettings = new();

        [SynthesisTooltip("List of target aspect ratios. e.g. \"4:3,16:10,16:9,21:9\"")]
        public string AspectRatios { set; get; } = "4:3,16:10,16:9,21:9";

        [SynthesisTooltip("Default aspect ratio for the fomod installer.")]
        public string DefaultAspectRatio { set; get; } = "16:9";

        [SynthesisSettingName("Loading Screen Priority")]

        public PrioritySettings prioritySettings = new();

        [SynthesisSettingName("Frequency List")]
        [SynthesisTooltip("List of possible frequency choices. The first element will be the default selected item in the Fomod installer.")]
        public string FrequencyList { set; get; } = "15,30,50,100";

        [SynthesisSettingName("Texture Compression LE")]
        public TextureCompressionLE textureCompressionLE = TextureCompressionLE.BC1;

        [SynthesisSettingName("Texture Compression SE")]
        public TextureCompressionSE textureCompressionSE = TextureCompressionSE.BC7;

        [SynthesisTooltip("The Fomod installer will be created in this directory.")]
        public string OutputDirectory { set; get; } = "";

        public LoadingScreenText loadingScreenText = LoadingScreenText.Optional;
    }
}
