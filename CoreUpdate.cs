using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private ActorVaalSkill UpdateVms()
        {
            var actorVaalSkills = GameController?.Player?.GetComponent<Actor>()?.ActorVaalSkills;
            return actorVaalSkills?.FirstOrDefault(s => s?.VaalSkillInternalName == "vaal_molten_shell");
        }
        
        private ActorVaalSkill UpdateVaalHaste()
        {
            var actorVaalSkills = GameController?.Player?.GetComponent<Actor>()?.ActorVaalSkills;
            return actorVaalSkills?.FirstOrDefault(s => s?.VaalSkillInternalName == "vaal_haste");
        }

        private ActorVaalSkill UpdateVaalGrace()
        {
            var actorVaalSkills = GameController?.Player?.GetComponent<Actor>()?.ActorVaalSkills;
            return actorVaalSkills?.FirstOrDefault(s => s?.VaalSkillInternalName == "vaal_grace");
        }
        
        private Life UpdateLifeComponent() => GameController?.Player?.GetComponent<Life>();
    }
}
