using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadScreenGen {

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

    public static class Enums {

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