using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private ActorVaalSkill? UpdateVms()
        {
            var actorVaalSkills = GameController?.Player?.GetComponent<Actor>()?.ActorVaalSkills;
            if (actorVaalSkills == null || actorVaalSkills.Count < 1)
            {
                return null;
            }

            return actorVaalSkills.FirstOrDefault(s => s.VaalSkillInternalName == "vaal_molten_shell");
        }

        private List<Buff>? UpdatePlayerBuffs()
        {
            return GameController?.Player?.GetComponent<Life>()?.Buffs;
        }
        
        private IEnumerable<Entity> UpdateEnemies()
        {
            var enemies = GameController.Entities
                .Where(x =>
                    x != null &&
                    x.IsAlive &&
                    x.IsHostile);

            return enemies;
        }
    }
}
