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
    public static class MagicEffectUtil {
        public static void SetScriptEffect(this MagicEffect effect) {
            effect.Flags |= MagicEffect.Flag.HideInUI;
            effect.MagicSkill = ActorValue.None;
            effect.ResistValue = ActorValue.None;
            effect.Archetype.Type = MagicEffectArchetype.TypeEnum.Script;
            effect.Archetype.ActorValue = ActorValue.None;
            effect.CastingSoundLevel = SoundLevel.Silent;
            effect.BaseCost = 0;
        }
    }
}
