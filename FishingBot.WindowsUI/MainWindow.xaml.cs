using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        private Thread botThread;


        public static Dictionary<string, IList<TeraPixel>> RodDictionary = new Dictionary<string, IList<TeraPixel>>()
        {
            { "Bloody (Not tested)", RodHooks.Bloody },
            { "Fiberglass (Not tested)", RodHooks.Fiberglass },
            { "Fisher of Souls (Not tested)", RodHooks.FisherOfSouls },
            { "FleshCatcher's (Not tested)", RodHooks.Fleshcatcher },
            { "Golden (Not tested)", RodHooks.Golden },
            { "Hotline's (Not tested)", RodHooks.Hotline },
            { "Mechanic's (Not tested)", RodHooks.Mechanic },
            { "Scarab (Not tested)", RodHooks.Scarab },
            { "Sitting Duck's", RodHooks.SitingDuckHook },
            { "Wooden (Not tested)", RodHooks.Wooden }
        };

        private static string m_selectedRod = "Sitting Duck's";

        public MainWindow()
        {
            InitializeComponent();
            this.RodSelector.ItemsSource = RodDictionary.Keys;
            this.RodSelector.SelectedIndex = RodDictionary.Keys.ToList().IndexOf(m_selectedRod);
        }

        void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.botViewScanner = new BotViewScanner(this.BotViewImage, TaskScheduler.FromCurrentSynchronizationContext());
            Console.SetOut(new ConsoleLogWriter(this.OutputBox, ConsoleScrollViewer));
            Console.WriteLine("Allo");

            Task.Run(async () => await this.botViewScanner.Run());
        }

        void OnToggleBotClick(object sender, RoutedEventArgs e)
        {
            this.ToggleBot = !this.ToggleBot;
            this.botViewScanner.ToggleBot = this.ToggleBot;
            btn_ToggleBot.Content = this.ToggleBot ? "Stop" : "Start";
            Task.Run(
                () =>
                {
                    if (this.ToggleBot)
                    {
                        StartBot();
                    }
                    else
                    {
                        StopBot();
                    }
                    Console.WriteLine(this.ToggleBot ? "Start" : "Stop");
                });
        }

        void OnRodSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_selectedRod = e.AddedItems[0] as string;
            Console.WriteLine($"Selected rod: {m_selectedRod}");
        }

        public void StartBot()
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            botThread = new Thread(() => Run(this.cancellationTokenSource.Token));
            this.botThread.Start();
        }

        public void StopBot()
        {
            try
            {
                this.cancellationTokenSource.Cancel();
                this.botThread.Interrupt();
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        public async Task Run(CancellationToken token = default)
        {
            try
            {
                var bot = new Bot(new WindowsClicker(), new WindowsScreenCapture(), RodDictionary[m_selectedRod]);
                await bot.Run(token);
            }
            catch (ThreadInterruptedException) { }
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
    }
}
