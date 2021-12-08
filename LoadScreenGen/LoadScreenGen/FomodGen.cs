using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LoadScreenGen.Settings;
using System.IO.Compression;
using System.Diagnostics;

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
            image = Path.Combine("fomod", path);
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
            CopyImage("black.png", dest);
            CopyImage("crop.png", dest);
            CopyImage("fullheight.png", dest);
            CopyImage("fullwidth.png", dest);
            CopyImage("stretch.png", dest);
        }

        public static void CreateFomod(Image[] imageArray, HashSet<AspectRatio> aspectRatios, HashSet<BorderOption> borderOptions, HashSet<LoadScreenChoice> loadScreenChoices, List<int> frequencyList, int defaultFrequency, List<int> imageResolution, List<string> textureDirectory) {
            //Directory.GetCurrentDirectory()
            var rootDir = Program.fomodTmpPath;
            var fomodDir = Path.Combine(rootDir, "fomod");
            var mainDir = Path.Combine(fomodDir, "main");
            Directory.CreateDirectory(mainDir);
            bool useScripts = Program.Settings.authorSettings.choiceSettings.includeMcm || Program.Settings.authorSettings.choiceSettings.includeFrequency;
            if(useScripts) {
                CopyScripts(Path.Combine(fomodDir, "main", "scripts"));
            }
            var loadingScreenText = Program.Settings.authorSettings.loadingScreenText;
            var messagesDir = Path.Combine(fomodDir, "messages");
            var noMessagesDir = Path.Combine(fomodDir, "no_messages");
            var fomodSubDir = Path.Combine(fomodDir, "fomod");
            Directory.Move(Path.Combine(rootDir, "textures", "2K", "textures"), Path.Combine(mainDir, "textures"));
            Directory.Move(Path.Combine(rootDir, "meshes"), Path.Combine(fomodDir, "meshes"));
            CopyImages(Path.Combine(fomodSubDir, "images"));

            foreach(var loadScreenChoice in loadScreenChoices) {
                foreach(var iFrequency in frequencyList) {
                    var frequency = iFrequency;
                    bool breakAfter = false;
                    if(loadScreenChoice != LoadScreenChoice.Frequency && loadScreenChoice != LoadScreenChoice.Mcm) {
                        frequency = 0;
                        breakAfter = true;
                    }
                    if(loadingScreenText != LoadingScreenText.Never) {
                        var dir = Path.Combine(messagesDir, "" + loadScreenChoice, "" + frequency);
                        Directory.CreateDirectory(dir);
                        File.Move(Path.Combine(rootDir, Program.GetPluginName(frequency, true, loadScreenChoice)), Path.Combine(dir, Program.Settings.authorSettings.pluginName));
                    }
                    if(loadingScreenText != LoadingScreenText.Always) {
                        var dir = Path.Combine(noMessagesDir, "" + loadScreenChoice, "" + frequency);
                        Directory.CreateDirectory(dir);
                        File.Move(Path.Combine(rootDir, Program.GetPluginName(frequency, false, loadScreenChoice)), Path.Combine(dir, Program.Settings.authorSettings.pluginName));
                    }
                    if(breakAfter) {
                        break;
                    }
                }
            }

            File.WriteAllLines(Path.Combine(fomodSubDir, "info.xml"), new string[] {
                "<fomod>",
                "   <Name>" + Program.Settings.authorSettings.modName + "</Name>",
                "   <Author>" + Program.Settings.authorSettings.modAuthor + "</Author>",
                "   <Version>" + Program.Settings.authorSettings.modVersion + "</Version>",
                "   <Website>" + Program.Settings.authorSettings.modLink + "</Website>",
                "   <Description>" + Program.Settings.authorSettings.modDescription + "</Description>",
                "</fomod>"
            });

            var fomod = new Fomod(Program.Settings.authorSettings.modName);

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
                            borderInstallOption.AddFolder(Path.Combine("meshes", aspectRatio.ToString(), borderOption.ToString()), "");
                            if(Program.Settings.authorSettings.borderSettings.defaultBorderOption == borderOption) {
                                borderInstallOption.SetDefault();
                            }
                            chooseBorderOption.AddOption(borderInstallOption);
                        }
                        chooseBorderOption.RequireFlag("aspect_ratio_" + aspectRatio, "true");
                        fomod.AddInstallStep(chooseBorderOption);
                    } else {
                        ratioOption.AddFolder(Path.Combine("meshes", aspectRatio.ToString(), borderOptions.First().ToString()), "");
                    }
                    chooseAspectRatio.AddOption(ratioOption);
                }
            } else {
                if(borderOptions.Count > 1) {
                    var chooseBorderOption = new InstallStep("Border Options");
                    foreach(var borderOption in borderOptions) {
                        var borderInstallOption = new InstallOption(borderOption.ToString(), borderOption.ToDescription());
                        borderInstallOption.AddFolder(Path.Combine("meshes", aspectRatios.First().ToString(), borderOption.ToString()), "");
                        if(Program.Settings.authorSettings.borderSettings.defaultBorderOption == borderOption) {
                            borderInstallOption.SetDefault();
                        }
                        chooseBorderOption.AddOption(borderInstallOption);
                    }
                } else {
                    fomod.AddRequiredFolder(Path.Combine("meshes", aspectRatios.First().ToString(), borderOptions.First().ToString()), "");
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
                if(loadScreenChoices.Count == 1 && frequencyList.Count == 1) {
                    yes.AddFolder(Path.Combine("messages", loadScreenChoices.First().ToString(), frequencyList.First().ToString()), "");
                    no.AddFolder(Path.Combine("no_messages", loadScreenChoices.First().ToString(), frequencyList.First().ToString()), "");
                }
            } else if(loadingScreenText == LoadingScreenText.Always && loadScreenChoices.Count == 1 && frequencyList.Count == 1) {
                // no other plugin choices
                fomod.AddRequiredFolder(Path.Combine("messages", loadScreenChoices.First().ToString(), frequencyList.First().ToString()), "");
            } else if(loadingScreenText == LoadingScreenText.Never && loadScreenChoices.Count == 1 && frequencyList.Count == 1) {
                // no other plugin choices
                fomod.AddRequiredFolder(Path.Combine("no_messages", loadScreenChoices.First().ToString(), frequencyList.First().ToString()), "");
            }

            // from now on create two of everything to include the previous choice of loading screen text
            // yes = messages
            // no = no messages

            // has multiple loading screen priorities
            if(loadScreenChoices.Count > 1) {
                
                var chooseLoadScreenChoicesYes = new InstallStep("Loading Screen Priority");
                var chooseLoadScreenChoicesNo = new InstallStep("Loading Screen Priority");

                // loop all available priorities
                foreach(var loadScreenChoice in loadScreenChoices) {
                    var additionalDesc = "";
                    // whether there will be an option to select the frequency in the following page
                    bool selectFrequency = (loadScreenChoice == LoadScreenChoice.Frequency || loadScreenChoice == LoadScreenChoice.Mcm) && frequencyList.Count > 1;
                    if(selectFrequency) {
                        if(loadScreenChoice == LoadScreenChoice.Frequency) {
                            additionalDesc = " The frequency can be configured on the next page.";
                        }
                    } else {
                        if(loadScreenChoice == LoadScreenChoice.Frequency) {
                            additionalDesc = " The frequency is " + frequencyList.First() + "%.";
                        }
                    }

                    // if frequency is not configurable, there are no additional plugin choices
                    // otherwise use flag "loadScreenChoice_" + loadScreenChoice to store the priority choice

                    var loadScreenChoiceOptionYes = new InstallOption(loadScreenChoice.ToString(), loadScreenChoice.ToDescription() + additionalDesc);
                    if(!selectFrequency) {
                        loadScreenChoiceOptionYes.AddFolder(Path.Combine("messages", "" + loadScreenChoice, "" + frequencyList.First()), "");
                    } else {
                        loadScreenChoiceOptionYes.AddFlag("loadScreenChoice_" + loadScreenChoice, "true");
                    }
                    chooseLoadScreenChoicesYes.AddOption(loadScreenChoiceOptionYes);

                    var loadScreenChoiceOptionNo = new InstallOption(loadScreenChoice.ToString(), loadScreenChoice.ToDescription() + additionalDesc);
                    if(!selectFrequency) {
                        loadScreenChoiceOptionYes.AddFolder(Path.Combine("no_messages", "" + loadScreenChoice, "" + frequencyList.First()), "");
                    } else {
                        loadScreenChoiceOptionYes.AddFlag("loadScreenChoice_" + loadScreenChoice, "true");
                    }
                    chooseLoadScreenChoicesNo.AddOption(loadScreenChoiceOptionNo);

                    // set the priority default
                    // again twice for messages and no messages
                    if(loadScreenChoice == Program.Settings.authorSettings.choiceSettings.defaultChoiceSetting) {
                        loadScreenChoiceOptionYes.SetDefault();
                        loadScreenChoiceOptionNo.SetDefault();
                    }
                }

                // now the existing two steps for messages and no messages are added

                // if messages are optional, the two steps are both added and conditionally selected based on the flag
                if(loadingScreenText == LoadingScreenText.Optional) {
                    chooseLoadScreenChoicesYes.RequireFlag("loading_screen_messages", "true");
                    chooseLoadScreenChoicesNo.RequireFlag("loading_screen_messages", "false");
                    fomod.AddInstallStep(chooseLoadScreenChoicesYes);
                    fomod.AddInstallStep(chooseLoadScreenChoicesNo);
                }
                // if messages are fixed, the corresponding step is added
                if(loadingScreenText == LoadingScreenText.Always) {
                    fomod.AddInstallStep(chooseLoadScreenChoicesYes);
                }
                if(loadingScreenText == LoadingScreenText.Never) {
                    fomod.AddInstallStep(chooseLoadScreenChoicesNo);
                }

                // now the frequency choice is added
                if(frequencyList.Count > 1) {
                    // only consider Frequency and Mcm
                    var validOptions = new HashSet<LoadScreenChoice>() { LoadScreenChoice.Frequency, LoadScreenChoice.Mcm };
                    validOptions.IntersectWith(loadScreenChoices);
                    foreach(var loadScreenChoice in validOptions) {
                        // do everything for messages and no messages and also for all loading screen priorities
                        var chooseFrequencyYes = new InstallStep("Loading Screen Frequency");
                        var chooseFrequencyNo = new InstallStep("Loading Screen Frequency");


                        foreach(var frequency in frequencyList) {
                            var desc = "Controls how often the loading screens appear. With a frequency of 100%, loading screens from vanilla and vanilla compatible loading screen mods will no longer be used.";
                            if(loadScreenChoice == LoadScreenChoice.Mcm) {
                                desc += " This is only the default frequency. It can also be configured in the Mod Configuration Menu";
                            }

                            var freqOptionYes = new InstallOption("" + frequency + '%', desc);
                            freqOptionYes.AddFolder(Path.Combine("messages", "" + frequency), "");
                            chooseFrequencyYes.AddOption(freqOptionYes);

                            var freqOptionNo = new InstallOption("" + frequency + '%', desc);
                            freqOptionNo.AddFolder(Path.Combine("no_messages", "" + frequency), "");
                            chooseFrequencyNo.AddOption(freqOptionNo);

                            if(frequency == defaultFrequency) {
                                freqOptionYes.SetDefault();
                                freqOptionNo.SetDefault();
                            }
                        }
                        

                        // add condition on the loading screen priority
                        chooseFrequencyYes.RequireFlag("loadScreenChoice_" + loadScreenChoice, "true");
                        chooseFrequencyNo.RequireFlag("loadScreenChoice_" + loadScreenChoice, "true");
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
            // the only difference is that loadScreenChoices.First() is used, since there is only one choice
            if(loadScreenChoices.Count == 1 && frequencyList.Count > 1) {
                var validOptions = new HashSet<LoadScreenChoice>() { LoadScreenChoice.Frequency, LoadScreenChoice.Mcm };
                validOptions.IntersectWith(loadScreenChoices);
                var choose_frequency_yes = new InstallStep("Loading Screen Frequency");
                var choose_frequency_no = new InstallStep("Loading Screen Frequency");


                foreach(var frequency in frequencyList) {
                    var desc = "Controls how often the loading screens appear. With a frequency of 100%, loading screens from vanilla and vanilla compatible loading screen mods will no longer be used.";
                    if(loadScreenChoices.First() == LoadScreenChoice.Mcm) {
                        desc += " This is only the default frequency. It can be configured in the Mod Configuration Menu";
                    }

                    var freq_option_yes = new InstallOption("" + frequency + '%', desc);
                    freq_option_yes.AddFolder(Path.Combine("messages", "" + frequency), "");
                    choose_frequency_yes.AddOption(freq_option_yes);

                    var freq_option_no = new InstallOption("" + frequency + '%', desc);
                    freq_option_no.AddFolder(Path.Combine("no_messages", "" + frequency), "");
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


            bool use7z = false;

            try {
                if(TextureGen.ShellExecuteWait("7z", "i").Contains("7z")) {
                    use7z = true;
                }
            } catch(Exception) { }

            var zipPath7z = Program.Settings.authorSettings.modName + "_" + Program.Settings.authorSettings.modVersion + ".7z";
            var zipPathZip = Program.Settings.authorSettings.modName + "_" + Program.Settings.authorSettings.modVersion + ".zip";
            zipPath7z = Path.Combine(rootDir, zipPath7z);
            zipPathZip = Path.Combine(rootDir, zipPathZip);
            if(use7z) {
                var cwd = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(fomodDir);
                TextureGen.ShellExecuteWait("7z", "a \"" + zipPath7z + "\" .\\* -mx");
                Directory.SetCurrentDirectory(cwd);
            }
            ZipFile.CreateFromDirectory(fomodDir, zipPathZip);


        }
    }

}