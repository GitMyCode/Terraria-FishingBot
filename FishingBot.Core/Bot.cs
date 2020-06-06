using System;
using System.Threading.Tasks;

using FishingBot.Core;
using FishingBot.Core.Infrastructure;
using FishingBot.Core.SearchAlgos;

public class Bot
{
    private readonly IClicker m_Clicker;
    private readonly IScreenCapture m_ScreenCapture;

    private readonly FishingMachine m_machine;
    private readonly SearchWithDeltaEColorCompare m_searchWithDelta;

    public Bot(IClicker clicker, IScreenCapture screenCapture)
    {
        this.m_Clicker = clicker;
        this.m_ScreenCapture = screenCapture;

        this.m_searchWithDelta = new SearchWithDeltaEColorCompare(RodHooks.SitingDuckHook);
        this.m_machine = new FishingMachine(this.m_Clicker, this.m_ScreenCapture, this.m_searchWithDelta);
    }

    public bool TogglePause { get; set; }  = false;

    public async Task Run()
    {
        while (true)
        {
            if (TogglePause)
            {
                await Loop();
            }
            await Task.Delay(TimeSpan.FromMilliseconds(70));
        }
    }

    public async Task Loop()
    {
        await this.m_machine.Fish();
    }
} 