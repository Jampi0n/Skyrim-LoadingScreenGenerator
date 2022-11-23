using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimLE;
using Mutagen.Bethesda.Plugins;
using System.IO;

namespace LoadScreenGen {


    class PluginStandalone : PluginGen {
        protected override void Setup() { }
        protected override void ProcessLscr(Image image, LoadScreen lscr, int counter) { }
    }

    class PluginReplacer : PluginGen {
        protected override void Setup() { }
        protected override void ProcessLscr(Image image, LoadScreen lscr, int counter) {
            var cond = new ConditionFloat();
            var condData = new FunctionConditionData();
            cond.Data = condData;
            condData.Function = Condition.Function.GetRandomPercent;
            cond.ComparisonValue = 100;
            cond.CompareOperator = CompareOperator.LessThanOrEqualTo;
            lscr.Conditions.Add(cond);
        }
    }

    class PluginDebug : PluginGen {
        GlobalFloat? debugGlobal = null;
        protected override void Setup() {
            debugGlobal = mod!.Globals.AddNewFloat(prefix + "currentLoadingScreenIndex");
            debugGlobal.EditorID = prefix + "currentLoadingScreenIndex";
        }
        protected override void ProcessLscr(Image image, LoadScreen lscr, int counter) {
            var cond = new ConditionFloat();
            var condData = new FunctionConditionData();
            cond.Data = condData;
            condData.Function = Condition.Function.GetGlobalValue;
            condData.ParameterOneRecord = debugGlobal!.ToNullableLink();
            cond.ComparisonValue = counter;
            cond.CompareOperator = CompareOperator.EqualTo;
            lscr.Conditions.Add(cond);
        }
    }

    abstract class PluginFrequency : PluginGen {
        protected GlobalFloat? syncRandomVar;
        protected GlobalFloat? configFrequencyVar;
        protected PlacedObject? marker;
        protected Spell? cellStalkerSpell;
        protected abstract void InitQuest();
        protected override void Setup() {
            syncRandomVar = mod!.Globals.AddNewFloat(prefix + "syncRandomVar");
            syncRandomVar.EditorID = prefix + "syncRandomVar";
            configFrequencyVar = mod.Globals.AddNewFloat(prefix + "configFrequencyVar");
            configFrequencyVar.EditorID = prefix + "configFrequencyVar";
            configFrequencyVar.RawFloat = frequency;
            ScriptEntry script;
            if(Skyrim.Cell.AAADeleteWhenDoneTestJeremy.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(Program.state!.LinkCache, out var context)) {
                var cellStalkerCell = context.GetOrAddAsOverride(mod);
                marker = new PlacedObject(mod) {
                    MajorRecordFlagsRaw = (int)PlacedObject.StaticMajorFlag.Persistent,
                };
                marker.Base.SetTo(Skyrim.Static.XMarker);

                script = marker.Vmad().AddLocalScript("JLS_XMarkerReferenceScript");
                script.AddPlayerProperty("PlayerRef");
                script.AddObjectProperty("RandomVar", syncRandomVar);

                cellStalkerCell.Persistent.Add(marker);

                var cellStalkerEffect = mod.MagicEffects.AddNew(prefix + "CellStalkerEffect");
                cellStalkerEffect.Name = prefix + "CellStalkerEffect";
                cellStalkerEffect.SetScriptEffect();
                cellStalkerEffect.CastType = CastType.ConstantEffect;
                cellStalkerEffect.TargetType = TargetType.Self;
                script = cellStalkerEffect.Vmad().AddLocalScript("JLS_TrackInSameCell");
                script.AddPlayerProperty("PlayerRef");
                script.AddObjectProperty("MarkerRef", marker);
                script.AddObjectProperty("RandomVar", syncRandomVar);


                cellStalkerSpell = mod.Spells.AddNew(prefix + "CellStalkerSpell");
                cellStalkerSpell.Name = prefix + "CellStalkerSpell";
                cellStalkerSpell.CastType = CastType.ConstantEffect;
                cellStalkerSpell.TargetType = TargetType.Self;
                cellStalkerSpell.Type = SpellType.Ability;
                var effect = new Effect();
                effect.BaseEffect.SetTo(cellStalkerEffect);
                var effectData = new EffectData {
                    Area = 0,
                    Magnitude = 0,
                    Duration = 0
                };
                effect.Data = effectData;
                var cond = new ConditionFloat();
                cond.ComparisonValue = 0f;
                var conditionData = new FunctionConditionData();
                conditionData.Function = Condition.Function.GetInSameCell;
                conditionData.ParameterOneRecord.SetTo(marker);
                cond.Data = conditionData;
                effect.Conditions.Add(cond);
                cellStalkerSpell.Effects.Add(effect);
                InitQuest();
            }
        }
        protected override void ProcessLscr(Image image, LoadScreen lscr, int counter) {
            var cond = new ConditionGlobal();
            var condData = new FunctionConditionData();
            cond.Data = condData;
            cond.Flags = Condition.Flag.UseGlobal;
            condData.Function = Condition.Function.GetGlobalValue;
            condData.ParameterOneRecord = syncRandomVar!.ToNullableLink();
            cond.ComparisonValue = configFrequencyVar!.ToNullableLink();
            cond.CompareOperator = CompareOperator.LessThanOrEqualTo;
            lscr.Conditions.Add(cond);
        }
    }
    class PluginMcm : PluginFrequency {
        protected override void InitQuest() {
            Quest questObject = mod!.Quests.AddNew();
            ScriptedQuestPlayerAlias quest;

            quest = new McmQuest(questObject, prefix + "Quest_MCM");

            quest.GetAlias().Spells.Add(cellStalkerSpell!);

            var script = quest.Vmad().AddLocalScript("JLS_MCM_Quest_Script");
            script.AddStringProperty("ModName", Program.Settings.authorSettings.McmName);
            script.AddObjectProperty("FrequencyProperty", configFrequencyVar!);
        }
    }
    class PluginFixed : PluginFrequency {
        protected override void InitQuest() {
            Quest questObject = mod!.Quests.AddNew();
            ScriptedQuestPlayerAlias quest;

            quest = new ScriptedQuestPlayerAlias(questObject, prefix + "Quest");
            questObject.VirtualMachineAdapter = null;

            quest.GetAlias().Spells.Add(cellStalkerSpell!);

        }
    }

    public abstract class PluginGen {
        protected ISkyrimMod? mod = null;
        protected string prefix = "";
        protected string meshPath = "";
        protected bool includeMessages = false;
        protected int frequency = 0;
        protected void Init(ISkyrimMod mod, string prefix, string meshPath, bool includeMessages, int frequency) {
            this.mod = mod;
            this.prefix = prefix;
            this.meshPath = meshPath;
            this.includeMessages = includeMessages;
            this.frequency = frequency;
            this.Setup();
        }
        protected void CreateLscr(Image image, int counter) {
            var stat = mod!.Statics.AddNew(prefix + "STAT_" + counter);
            stat.Model = new Model() {
                File = Path.Combine(meshPath, image.skyrimPath + ".nif")
            };
            stat.MaxAngle = 90;

            var lscr = mod.LoadScreens.AddNew(prefix + "LSCR_" + counter);
            lscr.LoadingScreenNif.SetTo(stat);
            lscr.InitialScale = 2;
            lscr.InitialRotation = new Noggog.P3Int16(-90, 0, 0);
            lscr.InitialTranslationOffset = new Noggog.P3Float(-45, 0, 0);
            if(includeMessages) {
                lscr.Description = image.text;
            }
            ProcessLscr(image, lscr, counter);
        }
        protected abstract void Setup();
        protected abstract void ProcessLscr(Image image, LoadScreen lscr, int counter);
        public static void CreateEsp(ISkyrimMod mod, Image[] imageArray, string meshPath, string prefix, bool includeMessages, int frequency, LoadingScreenPriority loadingScreenPriority) {
            Logger.DebugMsg("CreateEsp(" + mod + ", Array<Image>(" + imageArray.Length + "), " + meshPath + ", " + prefix + ", " + includeMessages + ", " + frequency + ", " + loadingScreenPriority + ");");
            PluginGen? pluginGen = null;
            if(Program.Settings.authorSettings.EnableAuthorMode) {
                mod.ModHeader.Author = Program.Settings.authorSettings.ModAuthor;
                mod.ModHeader.Description = Program.Settings.authorSettings.ModDescription;
            }
            if(imageArray.Length < 900 && mod.SkyrimRelease == SkyrimRelease.SkyrimSE) {
                mod.ModHeader.Flags |= SkyrimModHeader.HeaderFlag.LightMaster;
            }
            switch(loadingScreenPriority) {
                case LoadingScreenPriority.Standalone:
                    pluginGen = new PluginStandalone();
                    break;
                case LoadingScreenPriority.Replacer:
                    pluginGen = new PluginReplacer();
                    break;
                case LoadingScreenPriority.Mcm:
                    pluginGen = new PluginMcm();
                    break;
                case LoadingScreenPriority.Frequency:
                    pluginGen = new PluginFixed();
                    break;
                case LoadingScreenPriority.Debug:
                    pluginGen = new PluginDebug();
                    break;
                default:
                    break;
            }
            if(pluginGen != null) {
                pluginGen.Init(mod, prefix, meshPath, includeMessages, frequency);
                for(int i = 0; i < imageArray.Length; ++i) {
                    pluginGen.CreateLscr(imageArray[i], i);
                }
            }
        }

        public static ISkyrimMod CreateNewEsp(string pluginPath, SkyrimRelease release, string dataPath) {
            var skyrimEsmPath = Path.Combine(dataPath, "Skyrim.esm");
            using ISkyrimModDisposableGetter skyrimESM = SkyrimMod.CreateFromBinaryOverlay(skyrimEsmPath, release);


            return new SkyrimMod(ModKey.FromNameAndExtension(Path.GetFileName(pluginPath)), release);
        }

        public static void WriteNewEsp(ISkyrimMod mod, string pluginPath) {
            mod.WriteToBinaryParallel(pluginPath);
        }
    }
}