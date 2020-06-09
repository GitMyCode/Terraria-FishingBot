using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.Core.Infrastructure
{
    public static class Helper
    {
        public static async Task<T> WithStopWatch<T>(Func<Task<T>> func, string prefixText = "")
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await func();
            stopwatch.Stop();
            Console.WriteLine($"{prefixText} (Time in ms) {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }
    }
}
