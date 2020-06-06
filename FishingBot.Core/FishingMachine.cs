using System;
using System.Threading.Tasks;

using FishingBot.Core;
using FishingBot.Core.Infrastructure;
using FishingBot.Core.SearchAlgos;

using Stateless;

public class FishingMachine
{

    public enum State { IsFishing, IsCatching, LookingForHook, Paused, IsThrowingHook }

    public enum Trigger { Fish, ThrowFishingHook, FoundHook, HookNotFound }

    public readonly StateMachine<State, Trigger> _stateMachine;

    StateMachine<State, Trigger>.TriggerWithParameters<TeraPixel> _setFoundHookTrigger;

    private TeraPixel _placeOfHook;

    private readonly IClicker m_Clicker;
    private readonly IScreenCapture m_ScreenCapture;
    private readonly ISearchStrategy m_SearchStrategy;

    public FishingMachine(IClicker clicker, IScreenCapture screenCapture, ISearchStrategy searchStrategy)
    {
        this.m_Clicker = clicker;
        this.m_ScreenCapture = screenCapture;
        this.m_SearchStrategy = searchStrategy;

        this._stateMachine = new StateMachine<State, Trigger>(State.LookingForHook);
        ConfigureStateMachine();
    }

    public FishingMachine()
    {
        this._stateMachine = new StateMachine<State, Trigger>(State.LookingForHook);
        ConfigureStateMachine();
    }

    public void ConfigureStateMachine()
    {
        _setFoundHookTrigger = _stateMachine.SetTriggerParameters<TeraPixel>(Trigger.FoundHook);

        this._stateMachine.Configure(State.Paused)
            .Permit(Trigger.ThrowFishingHook, State.LookingForHook);

        this._stateMachine.Configure(State.LookingForHook)
            .PermitReentry(Trigger.Fish)
            .OnEntryAsync(async t => await OnLookingForHook())
            .Permit(Trigger.FoundHook, State.IsFishing);

        this._stateMachine.Configure(State.IsFishing)
            .PermitReentry(Trigger.Fish)
            .OnEntryAsync(async t => await OnIsFishing())
            .OnEntryFrom(Trigger.FoundHook, () => Console.WriteLine("Is Fishing: Wait for a catch!"))
            .Permit(Trigger.HookNotFound, State.IsCatching)
            .OnEntryFromAsync(_setFoundHookTrigger, async pixel => { _placeOfHook = pixel; });

        this._stateMachine.Configure(State.IsCatching)
            .OnEntryAsync(async t => await OnIsCatching())
            .Permit(Trigger.ThrowFishingHook, State.IsThrowingHook);

        this._stateMachine.Configure(State.IsThrowingHook)
            .OnEntryAsync(async t => await OnThrowHook())
            .Permit(Trigger.Fish, State.LookingForHook);

    }

    public async Task ThowsHook()
    {
        await _stateMachine.FireAsync(Trigger.ThrowFishingHook);
    }
    public async Task Fish()
    {
        await _stateMachine.FireAsync(Trigger.Fish);
    }

    public async Task FoundHook(TeraPixel pixel)
    {
        Console.WriteLine($"FoundHook : {pixel.X} {pixel.Y}");
        this._placeOfHook = pixel;
        await _stateMachine.FireAsync(_setFoundHookTrigger, pixel);
    }

    public async Task HookNotFound()
    {
        await _stateMachine.FireAsync(Trigger.HookNotFound);
    }

    public async Task OnThrowHook()
    {
        await m_Clicker.Click();
        await Task.Delay(1000);
        await Fish();
    }

    public async Task OnIsCatching()
    {
        Console.WriteLine("OnIsCatching: Found Catch!");
        await m_Clicker.Click();
        await Task.Delay(1000);
        await ThowsHook();
    }

    public async Task OnLookingForHook()
    {
        var searchResult = await Helper.WithStopWatch(() => this.m_SearchStrategy.Search(this.m_ScreenCapture.GetSnapshot()));
        if (searchResult.IsFound)
        {
            await FoundHook(searchResult.Pixel);
        }
    }

    public async Task OnIsFishing()
    {
        try
        {
            var searchResult = await this.m_SearchStrategy.Search(this.m_ScreenCapture.GetSnapshot(), _placeOfHook);
            if (!searchResult.IsFound)
            {
                await HookNotFound();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
       
    }
}