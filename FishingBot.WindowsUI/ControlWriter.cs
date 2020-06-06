using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FishingBot.WindowsUI
{
    public class ControlWriter : TextWriter
    {
        private TextBlock textbox;
        private TaskScheduler uiScheduler;
        private ScrollViewer viewer;

        public ControlWriter(TextBlock textbox, ScrollViewer viewer)
        {
            this.textbox = textbox;
            this.viewer = viewer;
            this.uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public override void Write(char value)
        {
            new TaskFactory(this.uiScheduler).StartNew(() => textbox.Text += value);
            //System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new ThreadStart(() => textbox.Text += value + "\n"));
        }

        public override void Write(string value)
        {
            new TaskFactory(this.uiScheduler).StartNew(() =>
            {
                this.textbox.Text += value;
                viewer.ScrollToBottom();
            });

            
         //   System.Windows.Threading.Dispatcher.CurreDispatcher.BeginInvoke(new ThreadStart();
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
