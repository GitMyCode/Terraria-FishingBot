using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FishingBot.WindowsUI
{
    public class ConsoleLogWriter : TextWriter
    {
        private TextBox _textbox;
        private readonly TaskScheduler _uiScheduler;
        private readonly ScrollViewer _viewer;
        private StringBuilder _log = new StringBuilder();
        private const int MaxCharCount = 3000;
        private const int ChunkSize = 500;
        private readonly TransformBlock<string, string> _updateUiLogBlock;

        public ConsoleLogWriter(TextBox textbox, ScrollViewer viewer)
        {
            this._textbox = textbox;
            this._viewer = viewer;
            this._uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            this._updateUiLogBlock = new TransformBlock<string, string>(
                value =>
                {
                    this._log.Append(value);
                    if (this._log.Length > MaxCharCount)
                    {
                        this._log = this._log.Remove(0, ChunkSize);
                    }

                    return this._log.ToString();
                }, new ExecutionDataflowBlockOptions
            {
                TaskScheduler = TaskScheduler.Default
            });
            var updateLogUi = new ActionBlock<string>(text => { this._textbox.Text = text; this._viewer.ScrollToBottom(); }, new ExecutionDataflowBlockOptions
            {
                TaskScheduler = this._uiScheduler
            });
            this._updateUiLogBlock.LinkTo(updateLogUi, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public override void Write(char value)
        {
            this._updateUiLogBlock.Post(value.ToString());
        }

        public override void Write(string value)
        {
            this._updateUiLogBlock.Post(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
