using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.Shared.Cache;

namespace VmsHelper
{
    public partial class VmsHelperCore : BaseSettingsPlugin<Settings>
    {
        TimeCache<Element> chatUi;

        private IEnumerable<Element> GetVisibleUi()
        {
            var elements = GameController?.IngameState?.IngameUi?.Children?.Where(
                x =>
                    x?.IsValid == true &&
                    x?.IsVisible == true &&
                    x?.IsVisibleLocal == true);
            return elements ?? new List<Element>();
        }

        private Element GetChatUi()
        {
            var value = GetVisibleUi().FirstOrDefault(
                x => 
                    x?.ChildCount == 5 &&
                    x?.Children?[0]?.ChildCount == 2 &&
                    x?.Children?[1]?.ChildCount == 3 &&
                    x?.Children?[2]?.ChildCount == 4);
            return value;
        }

        private bool ChatIsOpened()
        {
            return chatUi.Value?.Children?[3]?.IsVisible == true;
        }
    }
}