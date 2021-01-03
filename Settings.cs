using System.Windows.Forms;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace VmsHelper
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

        [Menu("Debug")]
        public ToggleNode Debug { get; set; } = new ToggleNode(true);

        #region VMS

        [Menu("Use VMS")]
        public ToggleNode UseVms { get; set; } = new ToggleNode(false);
        
        [Menu("VMS key")]
        public HotkeyNode VmsKey { get; set; } = new HotkeyNode(Keys.R);

        [Menu("Min HP to activate VMS (percent, set to 0 to disable)")]
        public RangeNode<int> VmsMinHpPercentThreshold { get; set; } = new RangeNode<int>(90, 0, 101);
        
        [Menu("Min ES to activate VMS (percent, set to 0 to disable)")]
        public RangeNode<int> VmsMinEsPercentThreshold { get; set; } = new RangeNode<int>(0, 0, 101);

        #endregion

        #region MS

        [Menu("Use MS")]
        public ToggleNode UseMs { get; set; } = new ToggleNode(false);
        
        [Menu("MS key")]
        public HotkeyNode MsKey { get; set; } = new HotkeyNode(Keys.T);

        [Menu("Min HP to activate MS (percent, set to 0 to disable)")]
        public RangeNode<int> MsMinHpPercentThreshold { get; set; } = new RangeNode<int>(99, 0, 101);
        
        [Menu("Min ES to activate MS (percent, set to 0 to disable)")]
        public RangeNode<int> MsMinEsPercentThreshold { get; set; } = new RangeNode<int>(0, 0, 101);

        #endregion

        #region Vaal Haste

        [Menu("Use Vaal Haste")]
        public ToggleNode UseVaalHaste { get; set; } = new ToggleNode(false);
        
        [Menu("Vaal Haste key")]
        public HotkeyNode VaalHasteKey { get; set; } = new HotkeyNode(Keys.T);

        #endregion
        
        #region Vaal Grace

        [Menu("Use Vaal Grace")]
        public ToggleNode UseVaalGrace { get; set; } = new ToggleNode(false);
        
        [Menu("Vaal Grace key")]
        public HotkeyNode VaalGraceKey { get; set; } = new HotkeyNode(Keys.T);

        [Menu("Min HP to activate Vaal Grace (percent, set to 0 to disable)")]
        public RangeNode<int> VaalGraceMinHpPercentThreshold { get; set; } = new RangeNode<int>(95, 0, 100);
        
        [Menu("Min ES to activate Vaal Grace (percent, set to 0 to disable)")]
        public RangeNode<int> VaalGraceMinEsPercentThreshold { get; set; } = new RangeNode<int>(95, 0, 100);

        #endregion
        
        #region Vaal D

        [Menu("Use Vaal Discipline")]
        public ToggleNode UseVaalD { get; set; } = new ToggleNode(false);
        
        [Menu("Vaal Discipline key")]
        public HotkeyNode VaalDKey { get; set; } = new HotkeyNode(Keys.W);

        [Menu("Min ES to activate Vaal Discipline (percent, set to 0 to disable)")]
        public RangeNode<int> VaalDMinEsPercentThreshold { get; set; } = new RangeNode<int>(40, 0, 100);

        #endregion
        
        #region Flasks

        [Menu("Granite flask enabled")]
        public ToggleNode GraniteFlaskEnabled { get; set; } = new ToggleNode(true);
 
        [Menu("Granite flask key")]
        public HotkeyNode GraniteFlaskKey { get; set; } = new HotkeyNode(Keys.D3);
        
        [Menu("Soul catcher enabled")]
        public ToggleNode SoulCatcherEnabled { get; set; } = new ToggleNode(true);

        [Menu("Soul catcher key")]
        public HotkeyNode SoulCatcherKey { get; set; } = new HotkeyNode(Keys.D4);
        
        [Menu("Min mana to activate soul catcher (absolute, set to 0 to disable)")]
        public RangeNode<int> MinManaSoulCatcherThreshold { get; set; } = new RangeNode<int>(90, 0, 500);
        
        [Menu("Soul ripper enabled")]
        public ToggleNode SoulRipperEnabled { get; set; } = new ToggleNode(false);

        [Menu("Soul ripper key")]
        public HotkeyNode SoulRipperKey { get; set; } = new HotkeyNode(Keys.D5);
        
        #endregion
    }
}