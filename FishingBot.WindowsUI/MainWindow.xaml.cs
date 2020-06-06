using System;
using System.Collections.Generic;
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

namespace FishingBot.WindowsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ToggleBot = false;
        private Bot bot;
        public MainWindow()
        {
            InitializeComponent();
            ConsoleScrollViewer.ScrollToBottom();

            bot = new Bot(new WindowsClicker(), new WindowsScreenCapture());

            Console.SetOut(new ControlWriter(this.OutputBox, ConsoleScrollViewer));
            Console.WriteLine("Allo");

            Task.Run(async () => await Run());
        }

        void OnToggleBotClick(object sender, RoutedEventArgs e)
        {
            this.ToggleBot = !this.ToggleBot;
            this.bot.TogglePause = this.ToggleBot;
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
    }
}
