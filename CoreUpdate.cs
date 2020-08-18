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
                // broken offset PepeHands
                return null;
            }

            return actorVaalSkills.FirstOrDefault(s => s.VaalSkillInternalName == "vaal_molten_shell");
        }

        private double DistToCursor(Entity x)
        {
            return x?.IsValid != true ? double.MaxValue : Math.Sqrt(DistToCursorSqr(x));
        }

        private double DistToCursorSqr(Entity x)
        {
            if (x?.IsValid != true) return double.MaxValue;
            var cursorPosition = Input.MousePosition;
            var xOnScreen = GameController.IngameState.Camera.WorldToScreen(x.Pos);
            var xToCursorSqr = Helpers.DistanceSquared(xOnScreen, cursorPosition);
            return xToCursorSqr;
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
