using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FishingBot.Core;

namespace FishingBot.WindowsUI
{
    public class WindowsScreenCapture : IScreenCapture
    {
        public Bitmap GetSnapshot()
        {
            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;

            var diviseScreen = 5.0;
            var widthRegion = 3;
            var xStart = Convert.ToInt32(width * (2 / 5.0));
            var xEnd = Convert.ToInt32(width * (3 / 5.0));
            var yStart = Convert.ToInt32(height * (2.6 / 5.0));
            var yEnd = Convert.ToInt32(height * (2.8 / 5.0));
            var snapshot = CaptureScreen(xStart, yStart, xEnd, yEnd, new Size((xEnd - xStart), (yEnd - yStart)));
            return snapshot;
        }

        public static Bitmap CaptureScreen(int sourceX, int sourceY, int destX, int destY,
                                           Size regionSize)
        {
            Bitmap bmp = new Bitmap(regionSize.Width, regionSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(sourceX, sourceY, 0, 0, regionSize);
            return bmp;
        }
    }
}
