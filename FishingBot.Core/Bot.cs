using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FishingBot.Core;
using FishingBot.Core.Infrastructure;
using FishingBot.Core.SearchAlgos;

public class Bot
{
    private readonly IClicker m_Clicker;
    private readonly IScreenCapture m_ScreenCapture;

    private FishingMachine m_machine;
    private readonly SearchWithDeltaEColorCompare m_searchWithDelta;

    private readonly IList<TeraPixel> m_rod;

    public Bot(IClicker clicker, IScreenCapture screenCapture, IList<TeraPixel> rod)
    {
        this.m_Clicker = clicker;
        this.m_ScreenCapture = screenCapture;
        this.m_rod = rod;
        this.m_searchWithDelta = new SearchWithDeltaEColorCompare(this.m_rod);
    }

    public async Task Run(CancellationToken token)
    {
        this.m_machine = new FishingMachine(this.m_Clicker, this.m_ScreenCapture, this.m_searchWithDelta, token);

        while (!token.IsCancellationRequested)
        {

            await this.m_machine.Fish();
            await Task.Delay(TimeSpan.FromMilliseconds(70), token);
        }
    }
}