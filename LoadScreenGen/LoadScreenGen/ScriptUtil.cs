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

namespace LoadScreenGen {
    public static class ScriptUtil {
        public static VirtualMachineAdapter Vmad(this IScripted scripted) {
            if(scripted.VirtualMachineAdapter == null) {
                scripted.VirtualMachineAdapter = new VirtualMachineAdapter {
                    Version = 5,
                    ObjectFormat = 2,
                };
            }
            return scripted.VirtualMachineAdapter;
        }
        public static ScriptEntry AddLocalScript(this AVirtualMachineAdapter vmad, string name) {
            var script = new ScriptEntry {
                Name = name,
                Flags = ScriptEntry.Flag.Local,
            };
            vmad.Scripts.Add(script);
            return script;
        }

        public static ScriptObjectProperty AddObjectProperty(this ScriptEntry script, string name, IFormLink<ISkyrimMajorRecordGetter> obj) {
            var prop = new ScriptObjectProperty {
                Name = name,
                Flags = ScriptProperty.Flag.Edited,
                Object = obj,
                Alias = -1
            };
            script.Properties.Add(prop);
            return prop;
        }

        public static ScriptObjectProperty AddObjectProperty(this ScriptEntry script, string name, ISkyrimMajorRecordGetter obj) {
            var prop = new ScriptObjectProperty {
                Name = name,
                Flags = ScriptProperty.Flag.Edited,
                Object = obj.AsNullableLink(),
                Alias = -1
            };
            script.Properties.Add(prop);
            return prop;
        }

        public static ScriptObjectProperty AddPlayerProperty(this ScriptEntry script, string name) {
            return script.AddObjectProperty(name, Constants.Player.AsNullable());
        }

        public static ScriptIntProperty AddIntProperty(this ScriptEntry script, string name, int value) {
            var prop = new ScriptIntProperty {
                Name = name,
                Flags = ScriptProperty.Flag.Edited,
                Data = value
            };
            script.Properties.Add(prop);
            return prop;
        }

        public static ScriptFloatProperty AddFloatProperty(this ScriptEntry script, string name, float value) {
            var prop = new ScriptFloatProperty {
                Name = name,
                Flags = ScriptProperty.Flag.Edited,
                Data = value
            };
            script.Properties.Add(prop);
            return prop;
        }

        public static ScriptBoolProperty AddBoolProperty(this ScriptEntry script, string name, bool value) {
            var prop = new ScriptBoolProperty {
                Name = name,
                Flags = ScriptProperty.Flag.Edited,
                Data = value
            };
            script.Properties.Add(prop);
            return prop;
        }

        public static ScriptStringProperty AddStringProperty(this ScriptEntry script, string name, string value) {
            var prop = new ScriptStringProperty {
                Name = name,
                Flags = ScriptProperty.Flag.Edited,
                Data = value
            };
            script.Properties.Add(prop);
            return prop;
        }
    }

    public class ScriptedQuest {
        protected readonly Quest quest;
        public ScriptedQuest(Quest quest, string questName) {
            this.quest = quest;
            quest.EditorID = questName;
            quest.Name = questName;
            quest.Flags = Quest.Flag.RunOnce | Quest.Flag.StartGameEnabled;
            if(quest.VirtualMachineAdapter == null) {
                quest.VirtualMachineAdapter = new QuestAdapter();
            }
        }
        public QuestAdapter Vmad() {
            return quest.VirtualMachineAdapter!;
        }
    }

    public class ScriptedQuestPlayerAlias : ScriptedQuest {
        protected readonly QuestAlias playerAlias;
        public ScriptedQuestPlayerAlias(Quest quest, string questName, string aliasName = "PlayerAlias") : base(quest, questName) {
            playerAlias = new QuestAlias {
                Name = aliasName,
                Flags = new QuestAlias.Flag(),
                VoiceTypes = FormKey.Null.AsNullableLink<IAliasVoiceTypeGetter>()
            };
            playerAlias.ForcedReference.SetTo(Constants.Player.Cast<IPlacedGetter>());
            quest.Aliases.Add(playerAlias);   
        }

        public QuestFragmentAlias AddQuestFragmentAlias() {
            var playerAliasFragment = new QuestFragmentAlias();
            playerAliasFragment.Property.Object = quest.AsNullableLink();
            playerAliasFragment.Property.Alias = 0;
            quest.VirtualMachineAdapter!.Aliases.Add(playerAliasFragment);
            quest.VirtualMachineAdapter.Unknown = 2;
            return playerAliasFragment;
        }

        public QuestAlias GetAlias() {
            return playerAlias;
        }

    }
    public class McmQuest : ScriptedQuestPlayerAlias {
        public McmQuest(Quest quest, string questName) : base(quest, questName) {
            var playerAliasFragment = AddQuestFragmentAlias();
            var playerLoadScript = new ScriptEntry {
                Name = "SKI_PlayerLoadGameAlias",
                Flags = ScriptEntry.Flag.Local
            };
            playerAliasFragment.Scripts.Add(playerLoadScript);
        }

    }
}
