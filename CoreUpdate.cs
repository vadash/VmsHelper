using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
// ReSharper disable ConstantConditionalAccessQualifier

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private ActorVaalSkill UpdateVms()
        {
            return GetVaalSkill("vaal_molten_shell");
        }
       
        private ActorVaalSkill UpdateVaalHaste()
        {
            return GetVaalSkill("vaal_haste");
        }

        private ActorVaalSkill UpdateVaalGrace()
        {
            return GetVaalSkill("vaal_grace");
        }
        
        private Life UpdateLifeComponent() => GameController?.Player?.GetComponent<Life>();
        
        private ActorVaalSkill GetVaalSkill(string internalName)
        {
            var actorSkill = GameController
                ?.Player
                ?.GetComponent<Actor>()
                ?.ActorSkills
                ?.FirstOrDefault(s =>
                    s?.InternalName == internalName);
            if (actorSkill?.IsOnSkillBar != true) return null;
            var actorVaalSkill = GameController
                ?.Player
                ?.GetComponent<Actor>()
                ?.ActorVaalSkills
                ?.FirstOrDefault(s =>
                    s?.VaalSkillInternalName == internalName);
            return actorVaalSkill;
        }
    }
}
