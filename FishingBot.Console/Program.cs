//using System;
//using System.Threading.Tasks;
//using System.Drawing;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;
//using System.Drawing.Imaging;
//using System.IO;
//using InputManager;
//using System.Linq;
//using System.ServiceModel.Syndication;

//using FishingBot.Core.SearchAlgos;

//namespace FishingBot
//{
//    public class Program
//    {
//        public static bool ToggleBot = false;
//        public static bool ToggleScreenshotRunner = false;

//        [STAThread]
//        static async Task Main(string[] args)
//        {
//            Console.WriteLine("Fishing Bot");
//            KeyShorcuts registredShorcurts = new KeyShorcuts();
//            var kbh = new LowLevelKeyboardHook();
//            kbh.OnKeyPressed += registredShorcurts.KeyDown;
//            kbh.OnKeyUnpressed += registredShorcurts.KeyUp;
//            kbh.HookKeyboard();
//            Task.Run(async () => await Run());
//            Task.Run(async () => await ScreenshotsRunner());
//            Application.Run();
//            kbh.UnHookKeyboard();
//        }

//        public static async Task ScreenshotsRunner()
//        {
//            try
//            {
//                var screenshotDirPath = @$"D:\\DEV\\code-shed\\FishingBot\\screenshot_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}";
//                var screenshotDir = System.IO.Directory.CreateDirectory(screenshotDirPath);
//                var count = 0;
//                while (true)
//                {
//                    if (ToggleScreenshotRunner)
//                    {
//                        var snapshot = GetSnapshot();
//                        snapshot.Save(@$"{Path.Combine(screenshotDirPath, "screen-" + count + ".png")}");
//                        count++;
//                    }

//                    await Task.Delay(800);
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                throw;
//            }

//        }

//        public static async Task Run()
//        {
//            var bot = new Bot(new WindowsClicker(), new WindowsScreenCapture());
//            try
//            {
//                await bot.Run();
//            }
//            catch (Exception exception)
//            {
//                Console.WriteLine(exception);
//                throw;
//            }
//        }


//        public static async Task Click()
//        {
//            Mouse.ButtonDown(Mouse.MouseKeys.Left);
//            await Task.Delay(35);
//            Mouse.ButtonUp(Mouse.MouseKeys.Left);
//        }

//        public static Bitmap GetSnapshot()
//        {
//            var width = Screen.PrimaryScreen.Bounds.Width;
//            var height = Screen.PrimaryScreen.Bounds.Height;

//            var diviseScreen = 5.0;
//            var widthRegion = 3;
//            var xStart = Convert.ToInt32(width * (2 / 5.0));
//            var xEnd = Convert.ToInt32(width * (3 / 5.0));
//            var yStart = Convert.ToInt32(height * (2.6 / 5.0));
//            var yEnd = Convert.ToInt32(height * (2.8 / 5.0));
//            var snapshot = CaptureScreen(xStart, yStart, xEnd, yEnd, new Size((xEnd - xStart), (yEnd - yStart)));
//            return snapshot;
//        }

//        public static Bitmap CaptureScreen(int sourceX, int sourceY, int destX, int destY,
//            Size regionSize)
//        {
//            Bitmap bmp = new Bitmap(regionSize.Width, regionSize.Height);
//            Graphics g = Graphics.FromImage(bmp);
//            g.CopyFromScreen(sourceX, sourceY, 0, 0, regionSize);
//            return bmp;
//        }

//        public static async Task<T> WithStopWatch<T>(Func<Task<T>> func)
//        {
//            var stopwatch = Stopwatch.StartNew();
//            var result = await func();
//            stopwatch.Stop();
//            Console.WriteLine($"Time: {stopwatch.Elapsed} |\t| (Time in ms) {stopwatch.ElapsedMilliseconds} ms.");
//            return result;
//        }
//    }

//    public class KeyShorcuts
//    {
//        IDictionary<IEnumerable<Keys>, Action> Shortcuts = new Dictionary<IEnumerable<Keys>, Action>()
//        {
//            {
//                new Keys[]{Keys.LMenu, Keys.F7}, () =>
//                {
//                    if(Program.ToggleBot)
//                        Console.WriteLine("Turn off");
//                    else
//                        Console.WriteLine("Turn On");
//                    Program.ToggleBot = !Program.ToggleBot;
//                 }
//            },
//             {
//                new Keys[]{Keys.LMenu, Keys.F6}, () =>
//                {
//                    if(Program.ToggleScreenshotRunner)
//                        Console.WriteLine("Turn off ToggleScreenshotRunner");
//                    else
//                        Console.WriteLine("Turn On ToggleScreenshotRunner");
//                    Program.ToggleScreenshotRunner = !Program.ToggleScreenshotRunner;
//                 }
//            }
//        };

//        ISet<Keys> pressedKeys = new HashSet<Keys>();

//        public void KeyDown(object sender, Keys vkCode)
//        {
//            pressedKeys.Add(vkCode);
//            CheckShortcut();
//        }

//        public void KeyUp(object sender, Keys vkCode)
//        {
//            pressedKeys.Remove(vkCode);
//        }

//        void CheckShortcut()
//        {
//            foreach (var shortcut in Shortcuts)
//            {
//                if (shortcut.Key.SequenceEqual(pressedKeys))
//                {
//                    shortcut.Value();
//                }
//            }
//        }
//    }
//}
