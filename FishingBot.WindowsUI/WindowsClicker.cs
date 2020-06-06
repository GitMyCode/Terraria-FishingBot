using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FishingBot.Core;

using InputManager;

namespace FishingBot.WindowsUI
{
    public class WindowsClicker : IClicker
    {
        public async Task Click()
        {
            Mouse.ButtonDown(Mouse.MouseKeys.Left);
            await Task.Delay(35);
            Mouse.ButtonUp(Mouse.MouseKeys.Left);
        }
    }
}
