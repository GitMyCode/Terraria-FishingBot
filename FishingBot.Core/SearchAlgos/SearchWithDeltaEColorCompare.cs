using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.Core.SearchAlgos
{
    public class SearchWithDeltaEColorCompare : ISearchStrategy
    {
        public IEnumerable<TeraPixel> hookSchema;
        public int hookPixelCount;
        public int maximumXHook = 0;
        public int maximumYHook = 0;

        public SearchWithDeltaEColorCompare(IEnumerable<TeraPixel> hook)
        {
            this.hookSchema = hook;
            this.hookPixelCount = this.hookSchema.Count();
            this.maximumYHook = hook.Select(p => p.Y).Max();
            this.maximumXHook = hook.Select(p => p.X).Max();
        }

        public Task<SearchResult> Search(Bitmap screen)
        {
            var tolerance = 18;
            float countLoop = 0;
            var pixelArrays = this.GetPixelArray(screen);
            var calculations = this.GetDeltaECalculations(pixelArrays);
            var width = pixelArrays[0].Length;
            var height = pixelArrays.Length;

            var result = new SearchResult();

            Parallel.For(0, height, (y, state) =>
            {
                for (var x = 0; x < pixelArrays[0].Length; x++)
                {
                    var totalDiff = this.SearchFromPixel(x, y, pixelArrays, calculations, out var finishedLoop, width, height);
                    if (finishedLoop && totalDiff / this.hookPixelCount < tolerance)
                    {
                        result = new SearchResult
                        {
                            IsFound = true,
                            Pixel = new TeraPixel(x, y)
                        };
                        state.Stop();
                    }
                }
            });

            return Task.FromResult(result);
        }

        public Task<SearchResult> Search(Bitmap screen, TeraPixel pixel)
        {
            var result = new SearchResult();
            var tolerance = 19;
            var subBitmap = this.GetSubBitmap(screen, pixel);
            var height = subBitmap.Height;
            var width = subBitmap.Width;
            var pixelArrays = this.GetPixelArray(subBitmap);
            var calculations = this.GetDeltaECalculations(pixelArrays);

            Parallel.For(0,6,(y, state) =>
            {
                for (var x = 0; x < 6; x++)
                {
                    var totalDiff = this.SearchFromPixel(x, y, pixelArrays, calculations, out var finishedLoop, width, height);
                    if (finishedLoop && totalDiff / this.hookPixelCount < tolerance)
                    {
                        result = new SearchResult
                        {
                            IsFound = true,
                            Pixel = new TeraPixel(x, y)
                        };

                        state.Stop();
                    }
                }
            });

            return Task.FromResult(result);
        }

        public double SearchFromPixel(int x, int y, int[][] pixelArrays, DeltaECalculation[][] calculations, out bool finishedLoop, int width, int height)
        {
            finishedLoop = false;
            var totalDiff = 0.0;
            var completedLoop = this.hookPixelCount;
            foreach (var hookPixel in this.hookSchema)
            {
                if (x + hookPixel.X >= width)
                    break;
                if (y + hookPixel.Y >= height)
                    break;

                var observedY = y + hookPixel.Y;
                var observedX = x + hookPixel.X;
                var pixelToCheck = BitConverter.GetBytes(pixelArrays[y + hookPixel.Y][x + hookPixel.X]);
                var pixelCalculation = calculations[observedY][observedX];

                var sR = pixelToCheck[2];
                var sG = pixelToCheck[1];
                var sB = pixelToCheck[0];

                if (sR + sG + sB < 70)
                {
                    break;
                }

                var precomputedDiff = hookPixel.Calculation.CompareTo(pixelCalculation);
                totalDiff += precomputedDiff;

                completedLoop--;
            }

            finishedLoop = completedLoop == 0;
            return totalDiff;
        }

        public Bitmap GetSubBitmap(Bitmap btm, TeraPixel pixel)
        {
            var region = 3;
            var xStart = pixel.X - region < 0 ? 0 : pixel.X - region;
            var xEnd = pixel.X + region + this.maximumXHook;
            var yStart = pixel.Y - region < 0 ? 0 : pixel.Y - region;
            var yEnd = pixel.Y + region + this.maximumYHook;
            return btm.Clone(new System.Drawing.Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart), btm.PixelFormat);
        }

        private int[][] GetPixelArray(Bitmap bitmap)
        {
            var result = new int[bitmap.Height][];

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            for (int y = 0; y < bitmapData.Height; ++y)
            {
                result[y] = new int[bitmapData.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }

        private DeltaECalculation[][] GetDeltaECalculations(int[][] pixels)
        {
            var height = pixels.Length;
            var width = pixels[0].Length;
            var calculation = new DeltaECalculation[height][];
            for (var y = 0; y < height; y++)
            {
                calculation[y] = new DeltaECalculation[width];
                for (var x = 0; x < width; x++)
                {
                    var pixelToCheck = BitConverter.GetBytes(pixels[y][x]);

                    var R = pixelToCheck[2];
                    var G = pixelToCheck[1];
                    var B = pixelToCheck[0];
                    calculation[y][x] = new DeltaECalculation(R, G, B);
                }
            }

            return calculation;
        }
    }
}