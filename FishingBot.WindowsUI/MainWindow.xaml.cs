using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using FishingBot.Core;

namespace FishingBot.WindowsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ToggleBot = false;
        private BotViewScanner botViewScanner;
        private CancellationTokenSource cancellationTokenSource;


        public static Dictionary<string, IList<TeraPixel>> RodDictionary = new Dictionary<string, IList<TeraPixel>>()
        {
            { "Bloody", RodHooks.Bloody },
            { "Fiberglass", RodHooks.Fiberglass },
            { "Fisher of Souls", RodHooks.FisherOfSouls },
            { "FleshCatcher's", RodHooks.Fleshcatcher },
            { "Golden", RodHooks.Golden },
            { "Hotline's", RodHooks.Hotline },
            { "Mechanic's", RodHooks.Mechanic },
            { "Reinforced", RodHooks.Reinforced },
            { "Scarab", RodHooks.Scarab },
            { "Sitting Duck's", RodHooks.SitingDuckHook },
            { "Wooden", RodHooks.Wooden }
        };

        private static string m_selectedRod = "Sitting Duck's";

        public MainWindow()
        {
            InitializeComponent();
            ConsoleScrollViewer.ScrollToBottom();

            this.RodSelector.ItemsSource = RodDictionary.Keys;
            this.RodSelector.SelectedIndex = RodDictionary.Keys.ToList().IndexOf(m_selectedRod);

            
            this.botViewScanner = new BotViewScanner(this.BotViewImage, TaskScheduler.FromCurrentSynchronizationContext());
            Console.SetOut(new ControlWriter(this.OutputBox, ConsoleScrollViewer));
            Console.WriteLine("Allo");

            Task.Run(async () => await this.botViewScanner.Run());
        }

        void OnToggleBotClick(object sender, RoutedEventArgs e)
        {
            this.ToggleBot = !this.ToggleBot;
            this.botViewScanner.ToggleBot = this.ToggleBot;

            if (this.ToggleBot)
            {
                StartBot();
            }
            else
            {
                StopBot();
            }

            btn_ToggleBot.Content = this.ToggleBot ? "Stop" : "Start";
            Console.WriteLine(this.ToggleBot ? "Start" : "Stop");
        }

        void OnRodSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_selectedRod = e.AddedItems[0] as string;
            Console.WriteLine($"Selected rod: {m_selectedRod}");
        }

        public void StartBot()
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() => Run(this.cancellationTokenSource.Token), TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        public void StopBot()
        {
            this.cancellationTokenSource.Cancel();
        }

        public async Task Run(CancellationToken token = default)
        {
            try
            {
                var bot = new Bot(new WindowsClicker(), new WindowsScreenCapture(), RodDictionary[m_selectedRod]);
                await bot.Run(token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Bot stopped");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public class BotViewScanner
        {
            private TaskScheduler uiScheduler;
            private Image imageController;
            private IScreenCapture screenCapture;

            public BotViewScanner(Image imageController, TaskScheduler uiScheduler)
            {
                this.imageController = imageController;
                this.uiScheduler = uiScheduler;
                this.screenCapture = new WindowsScreenCapture();
            }

            public bool ToggleBot { get; set; }

            public async Task Run()
            {
                try
                {
                    while (true)
                    {
                        if (this.ToggleBot)
                        {
                            await new TaskFactory(this.uiScheduler).StartNew((() => { this.imageController.Source = this.ScreenToBitmapImage(); }));
                            await Task.Delay(500);
                        }

                        await Task.Delay(200);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }

            private BitmapImage ScreenToBitmapImage()
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (var memStream = new MemoryStream())
                {
                    var btm = screenCapture.GetSnapshot();
                    btm.Save(memStream, ImageFormat.Jpeg);
                    memStream.Position = 0;

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

                return bitmapImage;
            }
        }
    }
}
