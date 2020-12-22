using System;
using System.Collections;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;
// ReSharper disable ConstantConditionalAccessQualifier
// ReSharper disable RedundantExtendsListEntry
// ReSharper disable UnusedType.Global
// ReSharper disable ConstantNullCoalescingCondition
// ReSharper disable IteratorNeverReturns

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private const float MIN_VMS_DURATION = 8.99f;
        private const float MIN_MS_DURATION = 2.99f;
        
        private DateTime NextVaalHaste { get; set; }
        private DateTime NextVaalD { get; set; }
        private DateTime NextVaalGrace { get; set; }
        private DateTime NextVaalMoltenShell { get; set; }
        private DateTime NextMoltenShell { get; set; }
        private DateTime NextArmorFlask { get; set; }
        private DateTime NextSoulCatcherFlask { get; set; }
        private DateTime NextSoulRipperFlask { get; set; }
        private TimeCache<ActorVaalSkill> VaalDActorVaalSkill { get; set; }
        private TimeCache<ActorVaalSkill> VmsActorVaalSkill { get; set; }
        private TimeCache<ActorVaalSkill> VaalHasteActorVaalSkill { get; set; }
        private TimeCache<ActorVaalSkill> VaalGraceActorVaalSkill { get; set; }
        private TimeCache<Life> PlayerLifeComponent { get; set; }
        
        public override void OnLoad()
        {
            VaalDActorVaalSkill = new TimeCache<ActorVaalSkill>(UpdateVaalD, 5000);
            VmsActorVaalSkill = new TimeCache<ActorVaalSkill>(UpdateVms, 5000);
            VaalHasteActorVaalSkill = new TimeCache<ActorVaalSkill>(UpdateVaalHaste, 5000);
            VaalGraceActorVaalSkill = new TimeCache<ActorVaalSkill>(UpdateVaalGrace, 5000);
            PlayerLifeComponent = new TimeCache<Life>(UpdateLifeComponent, 66);
            Core.MainRunner.Run(new Coroutine(MainCoroutine(), this, "VmsHelperMain"));
            base.OnLoad();
        }

        private IEnumerator MainCoroutine()
        {
            while (true)
            {
                if (CanRun())
                {
                    yield return CastVallMoltenShell();
                    yield return CastMoltenShell();
                    yield return CastVaalD();
                    yield return CastVaalGrace();
                    if ((VmsActorVaalSkill.Value == null ||
                         VmsActorVaalSkill.Value.CurrVaalSouls == VmsActorVaalSkill.Value.VaalMaxSouls) &&
                        (VaalDActorVaalSkill.Value == null ||
                         VaalDActorVaalSkill.Value.CurrVaalSouls == VaalDActorVaalSkill.Value.VaalMaxSouls) &&
                        (VaalGraceActorVaalSkill.Value == null ||
                         VaalGraceActorVaalSkill.Value.CurrVaalSouls == VaalGraceActorVaalSkill.Value.VaalMaxSouls))
                    {
                        // Cast haste only when other vaal skills are charged
                        yield return CastVaalHaste();
                    }
                }

                yield return new WaitTime(33); // server tick
            }
        }

        private IEnumerator CastVaalD()
        {
            if (!Settings.UseVaalD) yield break;
            if (NextVaalD > DateTime.Now) yield break;
            if (VaalDActorVaalSkill.Value == null) yield break;
            yield return UseSoulRipper();
            var esCondition = Settings.VaalDMinEsPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurES < PlayerLifeComponent?.Value?.MaxES
                              * Settings.VaalDMinEsPercentThreshold / 100d;
            if (!esCondition) yield break;
            if (IsVaalSkillReady(VaalDActorVaalSkill))
            {
                yield return UseSoulCatcher();
                yield return Input.KeyPress(Settings.VaalDKey);
                NextVaalD = DateTime.Now.AddMilliseconds(new Random().Next(900, 1100));
            }
        }

        private IEnumerator CastVaalGrace()
        {
            if (!Settings.UseVaalGrace) yield break;
            if (NextVaalGrace > DateTime.Now) yield break;
            if (VaalGraceActorVaalSkill.Value == null) yield break;
            yield return UseSoulRipper();
            var hpCondition = Settings.VaalGraceMinHpPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurHP < PlayerLifeComponent?.Value?.MaxHP
                              * Settings.VaalGraceMinHpPercentThreshold / 100d;
            var esCondition = Settings.VaalGraceMinEsPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurES < PlayerLifeComponent?.Value?.MaxES
                              * Settings.VaalGraceMinEsPercentThreshold / 100d;
            if (!hpCondition && !esCondition) yield break;
            if (IsVaalSkillReady(VaalGraceActorVaalSkill))
            {
                yield return UseSoulCatcher();
                yield return Input.KeyPress(Settings.VaalGraceKey);
                NextVaalGrace = DateTime.Now.AddMilliseconds(new Random().Next(900, 1100));
            }
        }

        private IEnumerator CastVaalHaste()
        {
            if (!Settings.UseVaalHaste) yield break;
            if (NextVaalHaste > DateTime.Now) yield break;
            if (VaalHasteActorVaalSkill.Value == null) yield break;
            yield return UseSoulRipper();
            if (IsVaalSkillReady(VaalHasteActorVaalSkill))
            {
                yield return UseSoulCatcher();
                yield return Input.KeyPress(Settings.VaalHasteKey);
                NextVaalHaste = DateTime.Now.AddMilliseconds(new Random().Next(900, 1100));
            }
        }

        private IEnumerator CastVallMoltenShell()
        {
            if (!Settings.UseVms) yield break;
            if (NextVaalMoltenShell > DateTime.Now) yield break;
            if (VmsActorVaalSkill.Value == null) yield break;
            if (IsShieldUp()) yield break;
            yield return UseSoulRipper();
            var hpCondition = Settings.VmsMinHpPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurHP < PlayerLifeComponent?.Value?.MaxHP
                              * Settings.VmsMinHpPercentThreshold / 100d;
            var esCondition = Settings.VmsMinEsPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurES < PlayerLifeComponent?.Value?.MaxES
                              * Settings.VmsMinEsPercentThreshold / 100d;
            if (!hpCondition && !esCondition) yield break;
            if (IsVaalSkillReady(VmsActorVaalSkill))
            {
                yield return UseGraniteFlask();
                yield return UseSoulCatcher();
                yield return Input.KeyPress(Settings.VmsKey);
                NextVaalMoltenShell = DateTime.Now.AddMilliseconds(new Random().Next(900, 1100));
            }
        }

        private IEnumerator CastMoltenShell()
        {
            if (!Settings.UseMs) yield break;
            if (NextMoltenShell > DateTime.Now) yield break;
            if (PlayerLifeComponent?.Value?.CurMana < 14) yield break;
            if (IsShieldUp()) yield break;
            var hpCondition = Settings.MsMinHpPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurHP < PlayerLifeComponent?.Value?.MaxHP
                              * Settings.MsMinHpPercentThreshold / 100d;
            var esCondition = Settings.MsMinEsPercentThreshold > 0 &&
                              PlayerLifeComponent?.Value?.CurES < PlayerLifeComponent?.Value?.MaxES
                              * Settings.MsMinEsPercentThreshold / 100d;
            if (!hpCondition && !esCondition) yield break;
            yield return UseGraniteFlask();
            yield return Input.KeyPress(Settings.MsKey);
            NextMoltenShell = DateTime.Now.AddMilliseconds(new Random().Next(900, 1100));
        }
        
        private bool IsShieldUp()
        {
            var shield = PlayerLifeComponent?.Value?.Buffs
                .FirstOrDefault(buff => 
                    buff.Name == "molten_shell_shield" && 
                    buff.Timer > 0);
            
            if (shield?.MaxTime >= MIN_VMS_DURATION)
            {
                NextVaalMoltenShell = DateTime.Now.AddSeconds(shield?.Timer ?? 0.25);
            }
            else if (shield?.MaxTime >= MIN_MS_DURATION)
            {
                NextMoltenShell = DateTime.Now.AddSeconds(4);
            }
            
            return shield != null;
        }

        private IEnumerator UseSoulRipper()
        {
            if (Settings.SoulRipperEnabled &&
                NextSoulRipperFlask < DateTime.Now)
            {
                yield return Input.KeyPress(Settings.SoulRipperKey);
                NextSoulRipperFlask = DateTime.Now.AddMilliseconds(new Random().Next(450, 550));
            }
        }
        
        private IEnumerator UseSoulCatcher()
        {
            if (Settings.SoulCatcherEnabled &&
                NextSoulCatcherFlask < DateTime.Now &&
                PlayerLifeComponent?.Value?.CurMana > Settings.MinManaSoulCatcherThreshold)
            {
                yield return Input.KeyPress(Settings.SoulCatcherKey);
                NextSoulCatcherFlask = DateTime.Now.AddMilliseconds(new Random().Next(3750, 4250));
            }
        }
        
        private IEnumerator UseGraniteFlask()
        {
            if (Settings.GraniteFlaskEnabled &&
                NextArmorFlask < DateTime.Now)
            {
                yield return Input.KeyPress(Settings.GraniteFlaskKey);
                NextArmorFlask = DateTime.Now.AddMilliseconds(new Random().Next(4600, 5000));
            }
        }
        
        private static bool IsVaalSkillReady(CachedValue<ActorVaalSkill> vaalSkill)
        {
            return vaalSkill?.Value == null || // this offset is often broken
                   vaalSkill?.Value?.CurrVaalSouls >= vaalSkill?.Value?.VaalSoulsPerUse;
        }
       
        private bool CanRun()
        {
            if (!Settings.Enable) return false;
            if (GameController?.InGame == false) return false;
            if (GameController?.Area?.CurrentArea?.IsTown == true) return false;
            if (GameController?.Area?.CurrentArea?.IsHideout == true) return false;
            if (MenuWindow.IsOpened) return false;
            if (GameController?.Entities?.Count == 0) return false;
            if (GameController?.IsForeGroundCache == false) return false;
            if (ChatIsOpened()) return false;
            return true;
        }
    }
}
