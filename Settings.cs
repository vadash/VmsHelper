using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace VmsHelper
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

        [Menu("Debug")]
        public ToggleNode Debug { get; set; } = new ToggleNode(false);

        [Menu("Use VMS")]
        public ToggleNode UseVms { get; set; } = new ToggleNode(true);
        
        [Menu("VMS key")]
        public HotkeyNode VmsKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.R);

        [Menu("MS key")]
        public HotkeyNode MsKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.T);

        [Menu("Soul catcher enabled")]
        public ToggleNode SoulCatcherEnabled { get; set; } = new ToggleNode(true);

        [Menu("Soul catcher key")]
        public HotkeyNode SoulCatcherKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.D5);
        
        [Menu("Granite flask enabled")]
        public ToggleNode GraniteFlaskEnabled { get; set; } = new ToggleNode(true);
 
        [Menu("Granite flask key")]
        public HotkeyNode GraniteFlaskKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.D4);
        
        [Menu("Range to activate (grid units)")]
        public RangeNode<int> ActivateGridRange { get; set; } = new RangeNode<int>(50, 50, 250);
        
        [Menu("Min HP to activate (percent, set to 0 to disable)")]
        public RangeNode<int> MinHpPercentThreshold { get; set; } = new RangeNode<int>(90, 0, 100);
        
        [Menu("Min ES to activate (percent, set to 0 to disable)")]
        public RangeNode<int> MinEsPercentThreshold { get; set; } = new RangeNode<int>(90, 0, 100);
        
        [Menu("Min mana to activate soul catcher (absolute, set to 0 to disable)")]
        public RangeNode<int> MinManaSoulCatcherThreshold { get; set; } = new RangeNode<int>(90, 0, 500);
    }
}