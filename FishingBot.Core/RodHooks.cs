using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

public static class RodHooks
{

    public class HookLoader
    {
        public HookLoader()
        {
        }

        public IList<TeraPixel> Load(string hookResourceName)
        {
            var result = new List<TeraPixel>();
            using (var stream = Assembly.GetAssembly(typeof(HookLoader)).GetManifestResourceStream(hookResourceName))
            {
                var bmp = new Bitmap(stream);

                // skip the bottom since it is under water
                for (int y = 0; y < bmp.Height - 15; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        var p = bmp.GetPixel(x, y);
                        int a = p.A;

                        if (a != 0) // transparent
                        {
                            result.Add(new TeraPixel(x, y, p));
                        }
                    }
                }
            }

            return result;
        }
    }

    public class HookBuilder
    {

        List<(int xStart, int yStart, int xEnd, int yEnd, string hexColor)> Regions { get; set; } = new List<(int xStart, int yStart, int xEnd, int yEnd, string hexColor)>();

        public List<TeraPixel> Rod = new List<TeraPixel>();

        public HookBuilder WithRegion(int xStart, int yStart, int xEnd, int yEnd, string hexColor)
        {
            Regions.Add((xStart, yStart, xEnd, yEnd, hexColor));
            return this;
        }

        public IEnumerable<(int x, int y)> ToRegions(int xStart, int yStart, int xEnd, int yEnd)
        {
            return Enumerable.Range(xStart, (xEnd - xStart) + 1).Select(x => Enumerable.Range(yStart, (yEnd - yStart) + 1).Select(y => (x, y))).SelectMany(x => x);
        }

        public HookBuilder Build()
        {
            foreach (var region in Regions)
            {
                try
                {
                    var allPoints = ToRegions(region.xStart, region.yStart, region.xEnd, region.yEnd);
                    Rod.AddRange(allPoints.Select(x => BuildPixelWithComputedDiff(x.x, x.y, region.hexColor)));

                }
                catch (System.ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine($"Invalid region: {region.xStart}, {region.yStart},{region.xEnd}, {region.yEnd}");
                    throw;
                }
            }
            return this;
        }

        public TeraPixel BuildPixelWithComputedDiff(int x, int y, string hexColor)
        {
            var pixel = new TeraPixel(x, y, hexColor);

            var ratioBToR = pixel.Color.B/pixel.Color.R;
            var ratioGToR = pixel.Color.G/pixel.Color.R;

            for (var i = 1; i < 256; i++)
            {
                //var ratio = (double)i/ (double)pixel.Color.R ;

                var c1RatioG = i * ratioGToR;
                var c1RatioB = i * ratioBToR;

                pixel.precomputed.Add((byte)i, ((c1RatioG), (c1RatioB)));
            }

            return pixel;
        }
    }

    public static IList<TeraPixel> SitingDuckHook = new HookLoader().Load("FishingBot.Core.fishingpoles.Bobber_(Sitting_Duck's).png");

    public static List<TeraPixel> SitingDuckHook2 =
        new HookBuilder()

        .WithRegion(0, 8, 4, 10, "#3b3829")
        .WithRegion(0, 11, 2, 12, "#3b3829")
        .WithRegion(5, 11, 7, 12, "#3b3829")
        .WithRegion(3, 13, 4, 15, "#3b3829")
        .WithRegion(8, 3, 10, 10, "#3b3829")
        .WithRegion(8, 13, 10, 15, "#3b3829")
        .WithRegion(11, 0, 12, 2, "#3b3829")
        .WithRegion(13, 3, 15, 10, "#3b3829")
        .WithRegion(13, 13, 15, 15, "#3b3829")
        .WithRegion(16, 11, 18, 12, "#3b3829")
        .WithRegion(19, 8, 23, 10, "#3b3829")
        .WithRegion(21, 11, 23, 12, "#3b3829")
        .WithRegion(19, 13, 20, 15, "#3b3829")

        .WithRegion(3, 11, 4, 12, "#850a09")
        .WithRegion(11, 5, 12, 7, "#850a09")
        .WithRegion(19, 11, 20, 12, "#850a09")

        .WithRegion(5, 13, 7, 15, "#bbb08b")
        .WithRegion(11, 11, 12, 12, "#bbb08b")
        .WithRegion(16, 13, 18, 15, "#bbb08b")

        .WithRegion(8, 11, 10, 12, "#898165")
        .WithRegion(13, 11, 15, 12, "#898165")
        .WithRegion(11, 13, 12, 15, "#898165")

        .WithRegion(11, 3, 12, 4, "#b44316")

        .WithRegion(11, 8, 12, 10, "#a05d4a")

        .Build().Rod;
}