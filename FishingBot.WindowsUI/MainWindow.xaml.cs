using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FishingBot.Core;

namespace FishingBot.WindowsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ToggleBot = false;
        private Bot bot;
        private BotViewScanner botViewScanner;

        public MainWindow()
        {
            InitializeComponent();
            ConsoleScrollViewer.ScrollToBottom();

            this.bot = new Bot(new WindowsClicker(), new WindowsScreenCapture());
            this.botViewScanner = new BotViewScanner(this.BotViewImage, TaskScheduler.FromCurrentSynchronizationContext());
            Console.SetOut(new ControlWriter(this.OutputBox, ConsoleScrollViewer));
            Console.WriteLine("Allo");

            Task.Run(async () => await Run());
            Task.Run(async () => await this.botViewScanner.Run());
        }

        void OnToggleBotClick(object sender, RoutedEventArgs e)
        {
            this.ToggleBot = !this.ToggleBot;
            this.bot.TogglePause = this.ToggleBot;
            this.botViewScanner.ToggleBot = this.ToggleBot;
            btn_ToggleBot.Content = this.ToggleBot ? "Pause" : "Start";
            Console.WriteLine(this.ToggleBot ? "Start" : "Pause");
        }

        public async Task Run()
        {
            try
            {
                await bot.Run();
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
