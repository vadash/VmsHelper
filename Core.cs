using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

// ReSharper disable IteratorNeverReturns

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        private Random Random { get; } = new Random();
        private DateTime NextVallMoltenShell { get; set; }
        private DateTime NextMoltenShell { get; set; }
        private DateTime NextArmorFlask { get; set; }
        private DateTime NextSoulCatcherFlask { get; set; }
        private TimeCache<ActorVaalSkill?>? VmsActorVaalSkill { get; set; }
        //private TimeCache<IEnumerable<Entity>> NearbyEnemies { get; set; }
        
        public override void OnLoad()
        {
            VmsActorVaalSkill = new TimeCache<ActorVaalSkill?>(UpdateVms, 5000);
            //NearbyEnemies = new TimeCache<IEnumerable<Entity>>(UpdateEnemies, 500);
            Core.MainRunner.Run(new Coroutine(MainCoroutine(), this, "VmsHelper1"));
            base.OnLoad();
        }

        private IEnumerator MainCoroutine()
        {
            while (true)
            {
                if (!CanRun() || !Settings.Enable) { yield return new WaitTime(500); continue; }

                yield return CastVallMoltenShell();
                yield return new WaitTime(16); // half server tick
            }
        }

        private IEnumerator CastVallMoltenShell()
        {
            if (!Settings.Enable ||
                !Settings.UseVms ||
                NextVallMoltenShell > DateTime.Now ||
                !IsVmsReady())
                yield break;
            
            var playerHpPercent = GameController?.Player?.GetComponent<Life>()?.HPPercentage;
            var playerEsPercent = GameController?.Player?.GetComponent<Life>()?.ESPercentage;

            var hpCondition = Settings.MinHpPercentThreshold > 0 &&
                              playerHpPercent < Settings.MinHpPercentThreshold / 100d;
            
            var esCondition = Settings.MinEsPercentThreshold > 0 &&
                              playerEsPercent < Settings.MinEsPercentThreshold / 100d;
            
            if (hpCondition || esCondition)
            {
                if (Settings.GraniteFlaskEnabled && NextArmorFlask > DateTime.Now)
                {
                    yield return Input.KeyPress(Settings.GraniteFlaskKey);
                    NextArmorFlask = DateTime.Now.AddMilliseconds(4800);
                }
                if (Settings.SoulCatcherEnabled && NextSoulCatcherFlask > DateTime.Now)
                {
                    yield return Input.KeyPress(Settings.SoulCatcherKey);
                    NextSoulCatcherFlask = DateTime.Now.AddMilliseconds(4000);
                }

                yield return Input.KeyPress(Settings.VmsKey);
                yield return Input.KeyPress(Settings.VmsKey);
                yield return Input.KeyPress(Settings.VmsKey);
            }

            NextVallMoltenShell = DateTime.Now.AddMilliseconds(Random.Next(45, 55));
        }

        private bool IsVmsReady()
        {
            return VmsActorVaalSkill?.Value == null ||
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

        public override void Render()
        {

        }
    }
}
