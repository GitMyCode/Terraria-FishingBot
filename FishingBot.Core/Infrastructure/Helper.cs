using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.Core.Infrastructure
{
    public static class Helper
    {
        public static async Task<T> WithStopWatch<T>(Func<Task<T>> func)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await func();
            stopwatch.Stop();
            Console.WriteLine($"Time: {stopwatch.Elapsed} |\t| (Time in ms) {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }
    }
}
