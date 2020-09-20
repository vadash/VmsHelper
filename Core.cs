using System;
using System.Collections;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;

// ReSharper disable IteratorNeverReturns

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private const float MIN_VMS_DURATION = 8.99f;
        private const float MIN_MS_DURATION = 2.99f;
        
        private DateTime NextVaalHaste { get; set; }
        private DateTime NextVaalMoltenShell { get; set; }
        private DateTime NextMoltenShell { get; set; }
        private DateTime NextArmorFlask { get; set; }
        private DateTime NextSoulCatcherFlask { get; set; }
        private TimeCache<ActorVaalSkill> VmsActorVaalSkill { get; set; }
        private TimeCache<ActorVaalSkill> VaalHasteActorVaalSkill { get; set; }
        private TimeCache<Life> PlayerLifeComponent { get; set; }
        
        public override void OnLoad()
        {
            VmsActorVaalSkill = new TimeCache<ActorVaalSkill>(UpdateVms, 5000);
            VaalHasteActorVaalSkill = new TimeCache<ActorVaalSkill>(UpdateVaalHaste, 5000);
            PlayerLifeComponent = new TimeCache<Life>(UpdateLifeComponent, 66);
            Core.MainRunner.Run(new Coroutine(MainCoroutine(), this, "VmsHelperMain"));
            base.OnLoad();
        }

        private IEnumerator MainCoroutine()
        {
            while (true)
            {
                if (!CanRun())
                {
                    yield return new WaitTime(500);
                    continue;
                }

                if (!IsShieldUp())
                {
                    yield return CastVallMoltenShell();
                    yield return CastMoltenShell();               
                }
                yield return CastVaalHaste();
                
                yield return new WaitTime(33); // server tick
            }
        }

        private IEnumerator CastVaalHaste()
        {
            if (!Settings.UseVaalHaste) yield break;
            if (NextVaalHaste > DateTime.Now) yield break;
            
            if (Settings.SoulRipperEnabled &&
                VaalHasteActorVaalSkill?.Value?.CurrVaalSouls < VaalHasteActorVaalSkill?.Value?.VaalSoulsPerUse &&
                CanUseSoulRipperFlask())
            {
                yield return Input.KeyPress(Settings.SoulRipperKey);
                yield return new WaitTime(100);
            }

            if (IsVaalHasteReady())
            {
                if (Settings.SoulCatcherEnabled &&
                    NextSoulCatcherFlask < DateTime.Now &&
                    PlayerLifeComponent?.Value?.CurMana > Settings.MinManaSoulCatcherThreshold)
                {
                    yield return Input.KeyPress(Settings.SoulCatcherKey);
                    NextSoulCatcherFlask = DateTime.Now.AddMilliseconds(4000);
                }
                
                yield return Input.KeyPress(Settings.VaalHasteKey);
                NextVaalHaste = DateTime.Now.AddMilliseconds(1000);
            }
        }
        
        private IEnumerator CastVallMoltenShell()
        {
            if (!Settings.UseVms) yield break;
            if (NextVaalMoltenShell > DateTime.Now) yield break;
            if (!IsVmsReady() && !CanUseSoulRipperFlask()) yield break;

            var playerHpPercent = PlayerLifeComponent?.Value?.HPPercentage;
            var playerEsPercent = PlayerLifeComponent?.Value?.ESPercentage;

            var hpCondition = Settings.VmsMinHpPercentThreshold > 0 &&
                              playerHpPercent - 1 < Settings.VmsMinHpPercentThreshold / 100d;
            
            var esCondition = Settings.VmsMinEsPercentThreshold > 0 &&
                              playerEsPercent - 1 < Settings.VmsMinEsPercentThreshold / 100d;
            
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
                if (Settings.SoulRipperEnabled &&
                    !IsVmsReady() &&
                    CanUseSoulRipperFlask())
                {
                    yield return Input.KeyPress(Settings.SoulRipperKey);
                }

                yield return Input.KeyPress(Settings.VmsKey);
                NextVaalMoltenShell = DateTime.Now.AddMilliseconds(1000);
            }
        }

        private IEnumerator CastMoltenShell()
        {
            if (!Settings.UseMs) yield break;
            if (NextMoltenShell > DateTime.Now) yield break;

            if (PlayerLifeComponent?.Value?.CurMana < 14) yield break;
            var playerHpPercent = PlayerLifeComponent?.Value?.HPPercentage;
            var playerEsPercent = PlayerLifeComponent?.Value?.ESPercentage;

            var hpCondition = Settings.MsMinHpPercentThreshold > 0 &&
                              playerHpPercent - 1 < Settings.MsMinHpPercentThreshold / 100d;
            
            var esCondition = Settings.MsMinEsPercentThreshold > 0 &&
                              playerEsPercent - 1 < Settings.MsMinEsPercentThreshold / 100d;
            
            if (hpCondition || esCondition)
            {
                if (Settings.GraniteFlaskEnabled &&
                    NextArmorFlask < DateTime.Now)
                {
                    yield return Input.KeyPress(Settings.GraniteFlaskKey);
                    NextArmorFlask = DateTime.Now.AddMilliseconds(4800);
                }

                yield return Input.KeyPress(Settings.MsKey);
                NextMoltenShell = DateTime.Now.AddMilliseconds(1000);
            }
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

        private bool IsVmsReady()
        {
            return VmsActorVaalSkill?.Value == null || // this offset is often broken
                   VmsActorVaalSkill?.Value?.CurrVaalSouls >= VmsActorVaalSkill?.Value?.VaalSoulsPerUse;
        }
        
        private bool IsVaalHasteReady()
        {
            return VaalHasteActorVaalSkill?.Value == null || // this offset is often broken
                   VaalHasteActorVaalSkill?.Value?.CurrVaalSouls >= VaalHasteActorVaalSkill?.Value?.VaalSoulsPerUse;
        }

        private bool CanUseSoulRipperFlask()
        {
            var currentFlask = GameController
                ?.Game
                ?.IngameState
                ?.ServerData
                ?.PlayerInventories
                ?.FirstOrDefault(x => 
                    x?.Inventory?.InventType == InventoryTypeE.Flask)?.Inventory?.InventorySlotItems?.FirstOrDefault(x =>
                        x?.Item?.Path == @"Metadata/Items/Flasks/FlaskUtility8" &&
                        x?.Item?.GetComponent<RenderItem>().ResourcePath == @"Art/2DItems/Flasks/SoulRipper.dds")
                ?.Item;
            var flaskChargesStruct = currentFlask?.GetComponent<Charges>();
            var maxCharges = flaskChargesStruct?.ChargesMax + currentFlask?.GetComponent<Mods>()?.ItemMods[1].Value1;
            return flaskChargesStruct?.NumCharges == maxCharges;
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
