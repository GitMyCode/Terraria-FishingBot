using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FishingBot.Core
{
    public interface IScreenCapture
    {
        Bitmap GetSnapshot();
    }
}
