using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media.Imaging;

using FishingBot.Core;

using Image = System.Windows.Controls.Image;

namespace FishingBot.WindowsUI
{

    public class BotViewScanner
    {
        private readonly TaskScheduler _uiScheduler;
        private Image imageController;
        private IScreenCapture screenCapture;
        private readonly TransformBlock<IScreenCapture, BitmapImage> _updateBotViewUiFlow;

        public BotViewScanner(Image imageController, TaskScheduler uiScheduler)
        {
            this.imageController = imageController;
            this._uiScheduler = uiScheduler;
            this.screenCapture = new WindowsScreenCapture();

            this._updateBotViewUiFlow = new TransformBlock<IScreenCapture, BitmapImage>(sc =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (var memStream = new MemoryStream())
                {
                    var btm = sc.GetSnapshot();
                    btm.Save(memStream, ImageFormat.Jpeg);
                    memStream.Position = 0;

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // this is needed to pass to another thread
                }

                return bitmapImage;
            });

            var updateUi = new ActionBlock<BitmapImage>(
                bmp =>
                {
                    this.imageController.Source = bmp;
                }, new ExecutionDataflowBlockOptions
                {
                    TaskScheduler = this._uiScheduler
                });

            this._updateBotViewUiFlow.LinkTo(updateUi);

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
                        this._updateBotViewUiFlow.Post(this.screenCapture);
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
    }
}
