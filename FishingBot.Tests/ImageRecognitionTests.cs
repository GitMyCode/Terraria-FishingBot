using System;
using System.Threading.Tasks;
using Xunit;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit.Abstractions;
using FishingBot.Core.SearchAlgos;

namespace FishingBot.Tests
{
    public class Setup : IDisposable
    {
        public Setup()
        {
            // setup code
            Directory.CreateDirectory("./TestData/Passed");
            Directory.CreateDirectory("./TestData/Failed");

            foreach (var item in Directory.GetFiles("./TestData/Passed"))
            {
                File.Delete(item);
            }
            foreach (var item in Directory.GetFiles("./TestData/Failed"))
            {
                File.Delete(item);
            }
        }

        public void Dispose()
        {
            // clean-up code
        }
    }
    public class ImageRecognitionTests : IClassFixture<Setup>
    {

        private Setup classwideFixture;
        private readonly ITestOutputHelper output;
        public ImageRecognitionTests(Setup fixture, ITestOutputHelper output)
        {
            this.classwideFixture = fixture;
            this.output = output;
        }


        [Fact]
        public void PrintHook()
        {
            var hookBitmap = new Bitmap(100, 100);
            var hook = RodHooks.SitingDuckHook;
            foreach (var pixel in hook)
            {
                hookBitmap.SetPixel(pixel.X, pixel.Y, pixel.Color);
            }

            hookBitmap.Save($"{Path.Combine(Environment.CurrentDirectory, "hook.png")}");

        }
        
        [Theory]
        [InlineData("oasis-golden-01.png")]
        [InlineData("oasis-golden-02.png")]
        public async Task FindGoldenHook(string fileName)
        {
            var filePath = $"./TestData/{fileName}";
            var snapshot = new Bitmap(filePath);
            
            var searchAlgo = new SearchWithDeltaEColorCompare(RodHooks.Golden);
            var result = await WithStopWatch(() => searchAlgo.Search(snapshot));

            if (result.IsFound)
            {
                ShowResult(result.Pixel, filePath, fileName);
            }
            else
            {
                snapshot.Save(Path.Combine("./TestData/Failed/", fileName));
            }

            Assert.True(result.IsFound);
        }


        [Theory]
        //[InlineData("screen-20.png")]
        //[InlineData("screen-21.png")]
        //[InlineData("screen-23.png")]
        //[InlineData("screen-24.png")]
        //[InlineData("screen-26.png")]
        ////[InlineData("screen-28.png")]
        //[InlineData("screen-32.png")]
        //[InlineData("screen-38.png")]
        //[InlineData("screen-40.png")]
        //[InlineData("screen-41.png")]
        //[InlineData("screen-42.png")]
        //[InlineData("screen-44.png")]
        //[InlineData("screen-46.png")]
        //[InlineData("screen-47.png")]
        //[InlineData("screen-62.png")]
        [InlineData("phil-1.png")]
        [InlineData("phil-2.png")]
        [InlineData("oasis-5.png")]
        [InlineData("oasis-borderless-1080-1.png")]
        [InlineData("oasis-borderless-1080-2.png")]
        public async Task FindHookInScreenshot(string fileName)
        {
            var filePath = $"./TestData/{fileName}";
            var snapshot = new Bitmap(filePath);
            
            var searchAlgo = new SearchWithDeltaEColorCompare(RodHooks.SitingDuckHook);
            var result = await WithStopWatch(() => searchAlgo.Search(snapshot));

            if (result.IsFound)
            {
                ShowResult(result.Pixel, filePath, fileName);
            }
            else
            {
                snapshot.Save(Path.Combine("./TestData/Failed/", fileName));
            }

            Assert.True(result.IsFound);
        }

        [Theory]
        //[InlineData("ocean-1.png")]
        //[InlineData("ocean-2.png")]
        //[InlineData("ocean-3.png")]
        //[InlineData("ocean-4.png")]
        //[InlineData("corruption-1.png")]
        //[InlineData("corruption-2.png")]
        //[InlineData("oasis-1.png")]
        //[InlineData("oasis-2.png")]
        //[InlineData("oasis-3.png")]
        [InlineData("oasis-4.png")]
        public async Task FindHookInScreenshotSpecialBiomes(string fileName)
        {
            var filePath = $"./TestData/{fileName}";
            var snapshot = new Bitmap(filePath);
            var searchAlgo = new SearchWithDeltaEColorCompare(RodHooks.SitingDuckHook);
            var result = await WithStopWatch(() => searchAlgo.Search(snapshot));

            if (result.IsFound)
            {
                ShowResult(result.Pixel, filePath, fileName);
            }
            else
            {
                snapshot.Save(Path.Combine("./TestData/Failed/", fileName));
            }

            Assert.True(result.IsFound);
        }


        [Theory]
        [InlineData("oasis-4.png", 57, 0)]
      //  [InlineData("oasis-4.png", 291, 15)]
       // [InlineData("screen-40.png", 664, 68)]
        public async Task FindHookInImage_WithSpecificPixel(string fileName, int x, int y)
        {
            var filePath = $"./TestData/{fileName}";
            var snapshot = new Bitmap(filePath);

            var searchAlgo = new SearchWithDeltaEColorCompare(RodHooks.SitingDuckHook);
            var result = await WithStopWatch(() => searchAlgo.Search(snapshot, new TeraPixel(x, y)));

            if (result.IsFound)
            {
                ShowResult(result.Pixel, filePath, fileName);
            }
            else
            {
                snapshot.Save(Path.Combine("./TestData/Failed/", fileName));
            }

            Assert.True(result.IsFound);
        }

        public async Task<T> WithStopWatch<T>(Func<Task<T>> func)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await func();
            stopwatch.Stop();
            this.output.WriteLine($"Time: {stopwatch.Elapsed} |\t| (Time in ms) {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }

        public void ShowResult(TeraPixel foundPoint, string screenFile, string fileName)
        {
            Bitmap foundBitmap = new Bitmap(screenFile);
            var resMin = RodHooks.SitingDuckHook.Aggregate((prev, next) => {
                var min =  prev.X + prev.Y >= next.X + next.Y ? next : prev;
                return min;
            });
            var resMax = RodHooks.SitingDuckHook.Aggregate((prev, next) => {
                var max =  prev.X + prev.Y <= next.X + next.Y ? next : prev;
                return max;
            });

            var rectangle = Enumerable.Range(resMin.X, resMax.X - resMin.X).Select(x => (x: x, y: resMin.Y)).ToList();
            rectangle.AddRange(Enumerable.Range(resMin.X, resMax.X - resMin.X).Select(x => (x, resMax.Y)));
            rectangle.AddRange(Enumerable.Range(resMin.Y, resMax.Y - resMin.Y).Select(y => (resMin.X, y)));
            rectangle.AddRange(Enumerable.Range(resMin.Y, resMax.Y - resMin.Y).Select(y => (resMax.X, y)));

            foreach (var p in rectangle)
            {
                foundBitmap.SetPixel(foundPoint.X + p.x, foundPoint.Y + p.y, ColorTranslator.FromHtml("#ff0000"));
            }

            foundBitmap.Save(Path.Combine("./TestData/Passed/", fileName));
        }
    }
}
