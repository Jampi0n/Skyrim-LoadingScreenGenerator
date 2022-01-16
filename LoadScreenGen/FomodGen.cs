using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LoadScreenGen.Settings;
using System.IO.Compression;
using System.Diagnostics;
using Mutagen.Bethesda.Skyrim;

namespace LoadScreenGen {

    public class InstallOption {
        public List<Tuple<string, string>> files = new();
        public List<Tuple<string, string>> folders = new();
        public List<Tuple<string, string>> flags = new();
        public string name;
        public string description;
        public string image = "";
        public bool isDefault = false;

        public InstallOption(string name, string description) {
            this.name = name;
            this.description = description;
        }

        public void AddFile(string src, string dest) {
            files.Add(new Tuple<string, string>(src, dest));
        }

        public void AddFolder(string src, string dest) {
            folders.Add(new Tuple<string, string>(src, dest));
        }

        public void AddFlag(string name, string value) {
            flags.Add(new Tuple<string, string>(name, value));
        }

        public void AddImage(string path) {
            image = Path.Combine("fomod", "images", path);
        }

        public void SetDefault() {
            isDefault = true;
        }
    }

    public class InstallStep {
        public string name;
        public List<InstallOption> options = new();
        public List<Tuple<string, string>> requiredFlags = new();

        public InstallStep(string name) {
            this.name = name;
        }

        public void AddOption(InstallOption option) {
            options.Add(option);
        }

        public void RequireFlag(string flagName, string flagValue) {
            requiredFlags.Add(new Tuple<string, string>(flagName, flagValue));
        }
    }

    public class Fomod {
        public string moduleName;
        public int indentationLevel = 0;
        public List<InstallStep> installSteps = new();
        public List<Tuple<string, string>> requiredFiles = new();
        public List<Tuple<string, string>> requiredFolders = new();
        public List<string> lines = new();
        public Fomod(string name) {
            moduleName = name;
        }

        public void AddRequiredFile(string src, string dest) {
            requiredFiles.Add(new Tuple<string, string>(src, dest));
        }
        public void AddRequiredFolder(string src, string dest) {
            requiredFolders.Add(new Tuple<string, string>(src, dest));
        }
        public void AddInstallStep(InstallStep step) {
            installSteps.Add(step);
        }

        public void Indent() {
            indentationLevel++;
        }
        public void Unindent() {
            indentationLevel--;
        }

        public void WriteLine(string line) {
            string indentation = "";
            for(int i = 0; i < indentationLevel; ++i) {
                indentation += "\t";
            }
            lines.Add(indentation + line);
        }

        public void Save(string path) {
            File.WriteAllLines(path, lines);
        }
        public void WriteFile() {
            WriteLine("<config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://qconsulting.ca/fo3/ModConfig5.0.xsd\"> ");
            Indent();
            WriteLine("<moduleName>" + moduleName + "</moduleName>");
            WriteLine("<requiredInstallFiles>");
            Indent();
            foreach(var tuple in requiredFiles) {
                var src = tuple.Item1;
                var dest = tuple.Item2;
                WriteLine("<file source=\"" + src + "\" destination=\"" + dest + "\" priority=\"0\" />");
            }
            foreach(var tuple in requiredFolders) {
                var src = tuple.Item1;
                var dest = tuple.Item2;
                WriteLine("<folder source=\"" + src + "\" destination=\"" + dest + "\" priority=\"0\" />");
            }
            Unindent();
            WriteLine("</requiredInstallFiles>");

            WriteLine("<installSteps order=\"Explicit\">");
            Indent();
            foreach(var step in installSteps) {

                WriteLine("<installStep name=\"" + step.name + "\">");
                Indent();
                if(step.requiredFlags.Count > 0) {
                    WriteLine("<visible>");
                    foreach(var tuple in step.requiredFlags) {
                        var flagName = tuple.Item1;
                        var flagValue = tuple.Item2;
                        WriteLine("<flagDependency flag=\"" + flagName + "\" value=\"" + flagValue + "\"/>");
                    }
                    WriteLine("</visible>");
                }
                WriteLine("<optionalFileGroups order=\"Explicit\">");
                Indent();
                WriteLine("<group name=\"" + step.name + "\" type=\"SelectExactlyOne\">");
                Indent();
                WriteLine("<plugins order=\"Explicit\">");
                Indent();
                foreach(var option in step.options) {
                    WriteLine("<plugin name=\"" + option.name + "\">");
                    Indent();
                    WriteLine("<description>" + option.description + "</description>");
                    if(option.image != "") {
                        WriteLine("<image path=\"" + option.image + "\"/>");
                    }
                    if(option.files.Count + option.folders.Count > 0) {
                        WriteLine("<files>");
                        Indent();
                        foreach(var tuple in option.files) {
                            var src = tuple.Item1;
                            var dest = tuple.Item2;
                            WriteLine("<file source=\"" + src + "\" destination=\"" + dest + "\" priority=\"0\" />");
                        }
                        foreach(var tuple in option.folders) {
                            var src = tuple.Item1;
                            var dest = tuple.Item2;
                            WriteLine("<folder source=\"" + src + "\" destination=\"" + dest + "\" priority=\"0\" />");
                        }
                        Unindent();
                        WriteLine("</files>");
                    }
                    if(option.flags.Count > 0) {
                        WriteLine("<conditionFlags>");
                        Indent();
                        foreach(var tuple in option.flags) {
                            var flagName = tuple.Item1;
                            var flagValue = tuple.Item2;
                            WriteLine("<flag name=\"" + flagName + "\">" + flagValue + "</flag>");
                        }
                        Unindent();
                        WriteLine("</conditionFlags>");
                    }
                    if(option.isDefault) {
                        WriteLine("<typeDescriptor>");
                        Indent();
                        WriteLine("<type name=\"Recommended\"/>");
                        Unindent();
                        WriteLine("</typeDescriptor>");
                    } else {
                        WriteLine("<typeDescriptor>");
                        Indent();
                        WriteLine("<type name=\"Optional\"/>");
                        Unindent();
                        WriteLine("</typeDescriptor>");
                    }
                    Unindent();
                    WriteLine("</plugin>");
                }
                Unindent();
                WriteLine("</plugins>");
                Unindent();
                WriteLine("</group>");
                Unindent();
                WriteLine("</optionalFileGroups>");
                Unindent();
                WriteLine("</installStep>");
            }
            Unindent();
            WriteLine("</installSteps>");
            Unindent();
            WriteLine("</config>");

        }
    }

    public class FomodGen {
        static void CopyScript(string scriptName, string dest) {
            File.Copy(Path.Combine(Program.resourceDirectory, "scripts", scriptName + ".pex"), Path.Combine(dest, scriptName + ".pex"));
            File.Copy(Path.Combine(Program.resourceDirectory, "scripts", "source", scriptName + ".psc"), Path.Combine(dest, "source", scriptName + ".psc"));
        }
        static void CopyScripts(string dest) {
            Directory.CreateDirectory(dest);
            Directory.CreateDirectory(Path.Combine(dest, "source"));
            CopyScript("JLS_MCM_Quest_Script", dest);
            CopyScript("JLS_TrackInSameCell", dest);
            CopyScript("JLS_XMarkerReferenceScript", dest);
        }

        static void CopyImage(string imageName, string dest) {
            File.Copy(Path.Combine(Program.resourceDirectory, "images", imageName), Path.Combine(dest, imageName));
        }

        static void CopyImages(string dest) {
            Directory.CreateDirectory(dest);
            CopyImage("Normal.png", dest);
            CopyImage("Crop.png", dest);
            CopyImage("FixedHeight.png", dest);
            CopyImage("FixedWidth.png", dest);
            CopyImage("Stretch.png", dest);
        }

        public static void CreateFomod(HashSet<AspectRatio> aspectRatios, HashSet<BorderOption> borderOptions, HashSet<LoadingScreenPriority> loadScreenPriorities, HashSet<SkyrimRelease> skyrimReleases, List<int> frequencyList, int defaultFrequency, List<int> imageResolution, AspectRatio defaultAspectRatio) {
            Logger.DebugMsg("CreateFomod([" + string.Join(",", aspectRatios) + "], [" + string.Join(",", borderOptions) + "], [" + string.Join(",", loadScreenPriorities) + "], [" + string.Join(",", skyrimReleases) + "], [" + string.Join(",", frequencyList) + "], " + defaultFrequency + ", [" + string.Join(",", imageResolution) + "], " + defaultAspectRatio + ");");

            var releaseSpecificTextures = Program.Settings.authorSettings.TargetRelease == TargetRelease.LE_and_SE && (int)(Program.Settings.authorSettings.textureCompressionLE) != (int)(Program.Settings.authorSettings.textureCompressionSE);

            var stopWatch = new Stopwatch();

            var rootDir = Program.fomodTmpPath;
            var fomodDir = Path.Combine(rootDir, "fomod");
            var mainDir = Path.Combine(fomodDir, "main");
            Directory.CreateDirectory(mainDir);
            bool useScripts = Program.Settings.authorSettings.prioritySettings.includeMcm || Program.Settings.authorSettings.prioritySettings.includeFrequency;
            if(useScripts) {
                CopyScripts(Path.Combine(fomodDir, "main", "scripts"));
            }
            var loadingScreenText = Program.Settings.authorSettings.loadingScreenText;
            var fomodSubDir = Path.Combine(fomodDir, "fomod");
            if(!releaseSpecificTextures) {
                Directory.Move(Path.Combine(rootDir, "textures", "2K", "textures"), Path.Combine(mainDir, "textures"));
            }
            CopyImages(Path.Combine(fomodSubDir, "images"));

            bool use7z = false;
            try {
                if(TextureGen.ShellExecuteWait("7z", "i").Contains("7z")) {
                    use7z = true;
                }
            } catch(Exception) { }

            foreach(var release in skyrimReleases) {
                if(releaseSpecificTextures) {
                    var texDir = Path.Combine(mainDir, "textures");
                    if(Directory.Exists(texDir)) {
                        Directory.Delete(texDir, true);
                    }
                    Directory.Move(Path.Combine(rootDir, "textures", release.ToString(), "2K", "textures"), texDir);
                }


                File.WriteAllLines(Path.Combine(fomodSubDir, "info.xml"), new string[] {
                    "<fomod>",
                    "   <Name>" + Program.Settings.authorSettings.ModName + "</Name>",
                    "   <Author>" + Program.Settings.authorSettings.ModAuthor + "</Author>",
                    "   <Version>" + Program.Settings.authorSettings.ModVersion + "</Version>",
                    "   <Website>" + (release ==SkyrimRelease.SkyrimLE ? Program.Settings.authorSettings.ModLinkLE  : Program.Settings.authorSettings.ModLinkSE) + "</Website>",
                    "   <Description>" + Program.Settings.authorSettings.ModDescription + "</Description>",
                    "</fomod>"
                });


                stopWatch.Restart();
                var fomod = new Fomod(Program.Settings.authorSettings.ModName);

                // textures
                fomod.AddRequiredFolder("main", "");

                // meshes
                if(aspectRatios.Count > 1) {
                    var chooseAspectRatio = new InstallStep("Aspect Ratio");
                    fomod.AddInstallStep(chooseAspectRatio);
                    foreach(var aspectRatio in aspectRatios) {
                        var ratioOption = new InstallOption("" + aspectRatio, "Use this option, if you have an aspect ratio of " + aspectRatio + ".");
                        if(borderOptions.Count > 1) {
                            ratioOption.AddFlag("aspect_ratio_" + aspectRatio, "true");
                            var chooseBorderOption = new InstallStep("Border Options");

                            foreach(var borderOption in borderOptions) {
                                var borderInstallOption = new InstallOption(borderOption.ToString(), borderOption.ToDescription());
                                borderInstallOption.AddFolder(Path.Combine("" + release, "meshes", aspectRatio.ToString(), borderOption.ToString()), "");
                                borderInstallOption.AddImage(borderOption + ".png");
                                if(Program.Settings.authorSettings.borderSettings.defaultBorderOption == borderOption) {
                                    borderInstallOption.SetDefault();
                                }
                                chooseBorderOption.AddOption(borderInstallOption);
                            }
                            chooseBorderOption.RequireFlag("aspect_ratio_" + aspectRatio, "true");
                            fomod.AddInstallStep(chooseBorderOption);
                        } else {
                            ratioOption.AddFolder(Path.Combine("" + release, "meshes", aspectRatio.ToString(), borderOptions.First().ToString()), "");
                        }
                        if(aspectRatio == defaultAspectRatio) {
                            ratioOption.SetDefault();
                        }
                        chooseAspectRatio.AddOption(ratioOption);
                    }
                } else {
                    if(borderOptions.Count > 1) {
                        var chooseBorderOption = new InstallStep("Border Options");
                        fomod.AddInstallStep(chooseBorderOption);
                        foreach(var borderOption in borderOptions) {
                            var borderInstallOption = new InstallOption(borderOption.ToString(), borderOption.ToDescription());
                            borderInstallOption.AddImage(borderOption + ".png");
                            borderInstallOption.AddFolder(Path.Combine("" + release, "meshes", aspectRatios.First().ToString(), borderOption.ToString()), "");
                            if(Program.Settings.authorSettings.borderSettings.defaultBorderOption == borderOption) {
                                borderInstallOption.SetDefault();
                            }
                            chooseBorderOption.AddOption(borderInstallOption);
                        }
                    } else {
                        fomod.AddRequiredFolder(Path.Combine("" + release, "meshes", aspectRatios.First().ToString(), borderOptions.First().ToString()), "");
                    }
                }

                // plugin
                // loading screen text
                // flag: loading_screen_messages
                if(loadingScreenText == LoadingScreenText.Optional) {
                    var chooseMessages = new InstallStep("Display Messages");
                    var yes = new InstallOption("Yes", "Enables loading screen messages.");
                    yes.AddFlag("loading_screen_messages", "true");
                    var no = new InstallOption("No", "Disables loading screen messages.");
                    no.AddFlag("loading_screen_messages", "false");
                    chooseMessages.AddOption(yes);
                    chooseMessages.AddOption(no);
                    fomod.AddInstallStep(chooseMessages);
                    // no other plugin choices
                    if(loadScreenPriorities.Count == 1 && frequencyList.Count == 1) {
                        yes.AddFolder(Path.Combine("" + release, "messages", loadScreenPriorities.First().ToString(), frequencyList.First().ToString()), "");
                        no.AddFolder(Path.Combine("" + release, "no_messages", loadScreenPriorities.First().ToString(), frequencyList.First().ToString()), "");
                    }
                } else if(loadingScreenText == LoadingScreenText.Always && loadScreenPriorities.Count == 1 && frequencyList.Count == 1) {
                    // no other plugin choices
                    fomod.AddRequiredFolder(Path.Combine("" + release, "messages", loadScreenPriorities.First().ToString(), frequencyList.First().ToString()), "");
                } else if(loadingScreenText == LoadingScreenText.Never && loadScreenPriorities.Count == 1 && frequencyList.Count == 1) {
                    // no other plugin choices
                    fomod.AddRequiredFolder(Path.Combine("" + release, "no_messages", loadScreenPriorities.First().ToString(), frequencyList.First().ToString()), "");
                }

                // from now on create two of everything to include the previous choice of loading screen text
                // yes = messages
                // no = no messages

                // has multiple loading screen priorities
                if(loadScreenPriorities.Count > 1) {

                    var chooseloadScreenPrioritiesYes = new InstallStep("Loading Screen Priority");
                    var chooseloadScreenPrioritiesNo = new InstallStep("Loading Screen Priority");

                    // loop all available priorities
                    foreach(var loadScreenPriority in loadScreenPriorities) {
                        var additionalDesc = "";
                        // whether there will be an option to select the frequency in the following page
                        bool selectFrequency = (loadScreenPriority == LoadingScreenPriority.Frequency || loadScreenPriority == LoadingScreenPriority.Mcm) && frequencyList.Count > 1;
                        if(selectFrequency) {
                            if(loadScreenPriority == LoadingScreenPriority.Frequency) {
                                additionalDesc = " The frequency can be configured on the next page.";
                            }
                        } else {
                            if(loadScreenPriority == LoadingScreenPriority.Frequency) {
                                additionalDesc = " The frequency is " + frequencyList.First() + "%.";
                            }
                        }

                        // if frequency is not configurable, there are no additional plugin choices
                        // otherwise use flag "loadScreenPriority_" + loadScreenPriority to store the priority choice

                        var loadScreenPriorityOptionYes = new InstallOption(loadScreenPriority.ToString(), loadScreenPriority.ToDescription() + additionalDesc);
                        if(!selectFrequency) {
                            loadScreenPriorityOptionYes.AddFolder(Path.Combine("" + release, "messages", "" + loadScreenPriority, "" + frequencyList.First()), "");
                        } else {
                            loadScreenPriorityOptionYes.AddFlag("loadScreenPriority_" + loadScreenPriority, "true");
                        }
                        chooseloadScreenPrioritiesYes.AddOption(loadScreenPriorityOptionYes);

                        var loadScreenPriorityOptionNo = new InstallOption(loadScreenPriority.ToString(), loadScreenPriority.ToDescription() + additionalDesc);
                        if(!selectFrequency) {
                            loadScreenPriorityOptionNo.AddFolder(Path.Combine("" + release, "no_messages", "" + loadScreenPriority, "" + frequencyList.First()), "");
                        } else {
                            loadScreenPriorityOptionNo.AddFlag("loadScreenPriority_" + loadScreenPriority, "true");
                        }
                        chooseloadScreenPrioritiesNo.AddOption(loadScreenPriorityOptionNo);

                        // set the priority default
                        // again twice for messages and no messages
                        if(loadScreenPriority == Program.Settings.authorSettings.prioritySettings.defaultPrioritySetting) {
                            loadScreenPriorityOptionYes.SetDefault();
                            loadScreenPriorityOptionNo.SetDefault();
                        }
                    }

                    // now the existing two steps for messages and no messages are added

                    // if messages are optional, the two steps are both added and conditionally selected based on the flag
                    if(loadingScreenText == LoadingScreenText.Optional) {
                        chooseloadScreenPrioritiesYes.RequireFlag("loading_screen_messages", "true");
                        chooseloadScreenPrioritiesNo.RequireFlag("loading_screen_messages", "false");
                        fomod.AddInstallStep(chooseloadScreenPrioritiesYes);
                        fomod.AddInstallStep(chooseloadScreenPrioritiesNo);
                    }
                    // if messages are fixed, the corresponding step is added
                    if(loadingScreenText == LoadingScreenText.Always) {
                        fomod.AddInstallStep(chooseloadScreenPrioritiesYes);
                    }
                    if(loadingScreenText == LoadingScreenText.Never) {
                        fomod.AddInstallStep(chooseloadScreenPrioritiesNo);
                    }

                    // now the frequency choice is added
                    if(frequencyList.Count > 1) {
                        // only consider Frequency and Mcm
                        var validOptions = new HashSet<LoadingScreenPriority>() { LoadingScreenPriority.Frequency, LoadingScreenPriority.Mcm };
                        validOptions.IntersectWith(loadScreenPriorities);
                        foreach(var loadScreenPriority in validOptions) {
                            // do everything for messages and no messages and also for all loading screen priorities
                            var chooseFrequencyYes = new InstallStep("Loading Screen Frequency");
                            var chooseFrequencyNo = new InstallStep("Loading Screen Frequency");


                            foreach(var frequency in frequencyList) {
                                var desc = "Controls how often the loading screens appear. With a frequency of 100%, loading screens from vanilla and vanilla compatible loading screen mods will no longer be used.";
                                if(loadScreenPriority == LoadingScreenPriority.Mcm) {
                                    desc += " This is only the default frequency. It can also be configured in the Mod Configuration Menu";
                                }

                                var freqOptionYes = new InstallOption("" + frequency + '%', desc);
                                freqOptionYes.AddFolder(Path.Combine("" + release, "messages", "" + loadScreenPriority, "" + frequency), "");
                                chooseFrequencyYes.AddOption(freqOptionYes);

                                var freqOptionNo = new InstallOption("" + frequency + '%', desc);
                                freqOptionNo.AddFolder(Path.Combine("" + release, "no_messages", "" + loadScreenPriority, "" + frequency), "");
                                chooseFrequencyNo.AddOption(freqOptionNo);

                                if(frequency == defaultFrequency) {
                                    freqOptionYes.SetDefault();
                                    freqOptionNo.SetDefault();
                                }
                            }


                            // add condition on the loading screen priority
                            chooseFrequencyYes.RequireFlag("loadScreenPriority_" + loadScreenPriority, "true");
                            chooseFrequencyNo.RequireFlag("loadScreenPriority_" + loadScreenPriority, "true");
                            // the existing two steps for messages and no messages are added
                            // if messages are optional, the two steps are both added and conditionally selected based on the flag
                            if(loadingScreenText == LoadingScreenText.Optional) {
                                chooseFrequencyYes.RequireFlag("loading_screen_messages", "true");
                                chooseFrequencyNo.RequireFlag("loading_screen_messages", "false");
                                fomod.AddInstallStep(chooseFrequencyYes);
                                fomod.AddInstallStep(chooseFrequencyNo);
                            }
                            // if messages are fixed, the corresponding step is added
                            if(loadingScreenText == LoadingScreenText.Always) {
                                fomod.AddInstallStep(chooseFrequencyYes);
                            }
                            if(loadingScreenText == LoadingScreenText.Never) {
                                fomod.AddInstallStep(chooseFrequencyNo);
                            }
                        }
                    }
                }
                // so far we considered:
                // no priority + no frequency choices
                // priority choices + (no) frequency choices
                // so only one left is:
                // no priority choices + frequency choices

                // this is almost the same as before
                // the only difference is that loadScreenPriorities.First() is used, since there is only one choice
                if(loadScreenPriorities.Count == 1 && frequencyList.Count > 1) {
                    var validOptions = new HashSet<LoadingScreenPriority>() { LoadingScreenPriority.Frequency, LoadingScreenPriority.Mcm };
                    validOptions.IntersectWith(loadScreenPriorities);
                    var choose_frequency_yes = new InstallStep("Loading Screen Frequency");
                    var choose_frequency_no = new InstallStep("Loading Screen Frequency");


                    foreach(var frequency in frequencyList) {
                        var desc = "Controls how often the loading screens appear. With a frequency of 100%, loading screens from vanilla and vanilla compatible loading screen mods will no longer be used.";
                        if(loadScreenPriorities.First() == LoadingScreenPriority.Mcm) {
                            desc += " This is only the default frequency. It can be configured in the Mod Configuration Menu";
                        }

                        var freq_option_yes = new InstallOption("" + frequency + '%', desc);
                        freq_option_yes.AddFolder(Path.Combine("" + release, "messages", "" + loadScreenPriorities.First(), "" + frequency), "");
                        choose_frequency_yes.AddOption(freq_option_yes);

                        var freq_option_no = new InstallOption("" + frequency + '%', desc);
                        freq_option_no.AddFolder(Path.Combine("" + release, "no_messages", "" + loadScreenPriorities.First(), "" + frequency), "");
                        choose_frequency_no.AddOption(freq_option_no);

                        if(frequency == defaultFrequency) {
                            freq_option_yes.SetDefault();
                            freq_option_no.SetDefault();
                        }
                    }
                    if(loadingScreenText == LoadingScreenText.Optional) {
                        choose_frequency_yes.RequireFlag("loading_screen_messages", "true");
                        choose_frequency_no.RequireFlag("loading_screen_messages", "false");
                        fomod.AddInstallStep(choose_frequency_yes);
                        fomod.AddInstallStep(choose_frequency_no);
                    }
                    if(loadingScreenText == LoadingScreenText.Always) {
                        fomod.AddInstallStep(choose_frequency_yes);
                    }
                    if(loadingScreenText == LoadingScreenText.Never) {
                        fomod.AddInstallStep(choose_frequency_no);
                    }

                }

                fomod.WriteFile();
                fomod.Save(Path.Combine(fomodSubDir, "ModuleConfig.xml"));

                Directory.CreateDirectory(Path.Combine(fomodDir, "" + release));
                Directory.Move(Path.Combine(rootDir, "" + release, "meshes"), Path.Combine(fomodDir, "" + release, "meshes"));
                var messagesDir = Path.Combine(fomodDir, "" + release, "messages");
                var noMessagesDir = Path.Combine(fomodDir, "" + release, "no_messages");
                foreach(var loadScreenPriority in loadScreenPriorities) {
                    foreach(var iFrequency in frequencyList) {
                        var frequency = iFrequency;
                        bool breakAfter = false;
                        if(loadScreenPriority != LoadingScreenPriority.Frequency && loadScreenPriority != LoadingScreenPriority.Mcm) {
                            breakAfter = true;
                        }
                        if(loadingScreenText != LoadingScreenText.Never) {
                            var dir = Path.Combine(messagesDir, "" + loadScreenPriority, "" + frequency);
                            Directory.CreateDirectory(dir);
                            File.Move(Path.Combine(rootDir, Program.GetPluginName(release, frequency, true, loadScreenPriority)), Path.Combine(dir, Program.Settings.authorSettings.PluginName));
                        }
                        if(loadingScreenText != LoadingScreenText.Always) {
                            var dir = Path.Combine(noMessagesDir, "" + loadScreenPriority, "" + frequency);
                            Directory.CreateDirectory(dir);
                            File.Move(Path.Combine(rootDir, Program.GetPluginName(release, frequency, false, loadScreenPriority)), Path.Combine(dir, Program.Settings.authorSettings.PluginName));
                        }
                        if(breakAfter) {
                            break;
                        }
                    }
                }


                var archiveName = Program.Settings.authorSettings.ModName + "_" + Program.Settings.authorSettings.ModVersion + "_" + release;

                var zipPath7z = archiveName + ".7z";
                var zipPathZip = archiveName + ".zip";
                zipPath7z = Path.Combine(rootDir, zipPath7z);
                zipPathZip = Path.Combine(rootDir, zipPathZip);
                if(use7z) {
                    var cwd = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(fomodDir);
                    TextureGen.ShellExecuteWait("7z", "a \"" + zipPath7z + "\" .\\* -mx=5");
                    Directory.SetCurrentDirectory(cwd);
                    File.Move(zipPath7z, Path.Combine(Program.Settings.authorSettings.OutputDirectory, archiveName + ".7z"), true);
                } else {
                    ZipFile.CreateFromDirectory(fomodDir, zipPathZip, CompressionLevel.Optimal, false);
                    File.Move(zipPathZip, Path.Combine(Program.Settings.authorSettings.OutputDirectory, archiveName + ".zip"), true);
                }

                stopWatch.Stop();
                Logger.LogTime("[Fomod] main archive", stopWatch.Elapsed);

                Directory.Delete(Path.Combine(fomodDir, "" + release), true);
            }

            foreach(var imageRes in imageResolution) {
                if(imageRes == 2048) { continue; }
                stopWatch.Restart();
                
                foreach(var release in skyrimReleases) {
                    var textureResolutionDir = (imageRes / 1024) + "K";
                    var archiveName = imageRes + "_" + Program.Settings.authorSettings.ModVersion;
                    if(releaseSpecificTextures) {
                        textureResolutionDir = Path.Combine(release.ToString(), textureResolutionDir);
                        archiveName = release.ToString() + "_" + archiveName;
                    }
                    textureResolutionDir = Path.Combine(rootDir, "textures", textureResolutionDir);
                    archiveName = Program.Settings.authorSettings.ModName + "_Textures_" + archiveName;


                    var zipPath7z = archiveName + ".7z";
                    var zipPathZip = archiveName + ".zip";
                    zipPath7z = Path.Combine(rootDir, zipPath7z);
                    zipPathZip = Path.Combine(rootDir, zipPathZip);
                    if(use7z) {
                        var cwd = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(textureResolutionDir);
                        TextureGen.ShellExecuteWait("7z", "a \"" + zipPath7z + "\" .\\* -mx=5");
                        Directory.SetCurrentDirectory(cwd);
                        File.Move(zipPath7z, Path.Combine(Program.Settings.authorSettings.OutputDirectory, archiveName + ".7z"), true);
                    } else {
                        ZipFile.CreateFromDirectory(fomodDir, zipPathZip, CompressionLevel.Optimal, false);
                        File.Move(zipPathZip, Path.Combine(Program.Settings.authorSettings.OutputDirectory, archiveName + ".zip"), true);
                    }
                    if(!releaseSpecificTextures) {
                        break;
                    }
                }
                stopWatch.Stop();
                Logger.LogTime("[Fomod] texutre archive " + imageRes, stopWatch.Elapsed);
            }

        }
    }

}