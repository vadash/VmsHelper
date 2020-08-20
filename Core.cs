using System;
using System.Collections;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;

// ReSharper disable IteratorNeverReturns

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private const float MIN_VMS_DURATION = 8.99f;
        private const float MIN_MS_DURATION = 2.99f; 
        
        private DateTime NextVaalMoltenShell { get; set; }
        private DateTime NextMoltenShell { get; set; }
        private DateTime NextArmorFlask { get; set; }
        private DateTime NextSoulCatcherFlask { get; set; }
        private TimeCache<ActorVaalSkill?>? VmsActorVaalSkill { get; set; }
        private TimeCache<Life?>? PlayerLifeComponent { get; set; }
        
        public override void OnLoad()
        {
            VmsActorVaalSkill = new TimeCache<ActorVaalSkill?>(UpdateVms, 5000);
            PlayerLifeComponent = new TimeCache<Life?>(UpdateLifeComponent, 66);
            Core.MainRunner.Run(new Coroutine(MainCoroutine(), this, "VmsHelper1"));
            base.OnLoad();
        }

        private IEnumerator MainCoroutine()
        {
            while (true)
            {
                if (!CanRun()) { yield return new WaitTime(500); continue; }

                yield return CastVallMoltenShell();
                yield return CastMoltenShell();
                yield return new WaitTime(16); // half server tick
            }
        }

        private IEnumerator CastVallMoltenShell()
        {
            if (!Settings.UseVms ||
                NextVaalMoltenShell > DateTime.Now ||
                IsShieldUp() ||
                !IsVmsReady())
                yield break;

            var playerHpPercent = PlayerLifeComponent?.Value?.HPPercentage;
            var playerEsPercent = PlayerLifeComponent?.Value?.ESPercentage;

            var hpCondition = Settings.VmsMinHpPercentThreshold > 0 &&
                              playerHpPercent < Settings.VmsMinHpPercentThreshold / 100d;
            
            var esCondition = Settings.VmsMinEsPercentThreshold > 0 &&
                              playerEsPercent < Settings.VmsMinEsPercentThreshold / 100d;
            
            if (hpCondition || esCondition)
            {
                if (Settings.GraniteFlaskEnabled &&
                    NextArmorFlask < DateTime.Now)
                {
                    yield return Input.KeyPress(Settings.GraniteFlaskKey);
                    NextArmorFlask = DateTime.Now.AddMilliseconds(4800);
                }
                if (Settings.SoulCatcherEnabled &&
                    NextSoulCatcherFlask < DateTime.Now &&
                    PlayerLifeComponent?.Value?.CurMana > Settings.MinManaSoulCatcherThreshold)
                {
                    yield return Input.KeyPress(Settings.SoulCatcherKey);
                    NextSoulCatcherFlask = DateTime.Now.AddMilliseconds(4000);
                }

                yield return Input.KeyPress(Settings.VmsKey);
            }
        }

        private IEnumerator CastMoltenShell()
        {
            if (!Settings.UseMs ||
                NextMoltenShell > DateTime.Now ||
                IsShieldUp())
                yield break;
            
            if (PlayerLifeComponent?.Value?.CurMana < 14) yield break;
            var playerHpPercent = PlayerLifeComponent?.Value?.HPPercentage;
            var playerEsPercent = PlayerLifeComponent?.Value?.ESPercentage;

            var hpCondition = Settings.MsMinHpPercentThreshold > 0 &&
                              playerHpPercent < Settings.MsMinHpPercentThreshold / 100d;
            
            var esCondition = Settings.MsMinEsPercentThreshold > 0 &&
                              playerEsPercent < Settings.MsMinEsPercentThreshold / 100d;
            
            if (hpCondition || esCondition)
            {
                if (Settings.GraniteFlaskEnabled &&
                    NextArmorFlask < DateTime.Now)
                {
                    yield return Input.KeyPress(Settings.GraniteFlaskKey);
                    NextArmorFlask = DateTime.Now.AddMilliseconds(4800);
                }

                yield return Input.KeyPress(Settings.MsKey);
            }
        }
        
        private bool IsShieldUp()
        {
            var shield = PlayerLifeComponent?.Value?.Buffs
                .FirstOrDefault(buff => 
                    buff.Name == "molten_shell_shield" && 
                    buff.Timer > 0);
            
            if (NextVaalMoltenShell < DateTime.Now &&
                shield?.MaxTime >= MIN_VMS_DURATION)
            {
                NextVaalMoltenShell = DateTime.Now.AddSeconds(shield?.Timer ?? 0.25);
            }
            
            if (NextMoltenShell < DateTime.Now &&
                shield?.MaxTime >= MIN_MS_DURATION &&
                shield?.MaxTime < MIN_VMS_DURATION)
            {
                NextMoltenShell = DateTime.Now.AddSeconds(4);
            }
            
            return shield != null;
        }

        private bool IsVmsReady()
        {
            return VmsActorVaalSkill?.Value == null || // this offset is often broken
                   VmsActorVaalSkill?.Value?.CurrVaalSouls >= VmsActorVaalSkill?.Value?.VaalSoulsPerUse;
        }

        private bool CanRun()
        {
            if (!Settings.Enable) return false;
            if (GameController?.InGame == false) return false;
            if (GameController?.Area?.CurrentArea?.IsTown == true) return false;
            if (MenuWindow.IsOpened) return false;
            if (GameController?.Entities?.Count == 0) return false;
            if (GameController?.IsForeGroundCache == false) return false;
            return true;
        }
    }
}
