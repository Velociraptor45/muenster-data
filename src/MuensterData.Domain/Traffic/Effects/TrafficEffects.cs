using Fluxor;
using Microsoft.Extensions.Caching.Memory;
using MuensterData.Domain.Common.Ports;
using MuensterData.Domain.Traffic.Actions;
using MuensterData.Domain.Traffic.Actions.Accidents;
using MuensterData.Domain.Traffic.States;

namespace MuensterData.Domain.Traffic.Effects;

public class TrafficEffects
{
    private readonly ICsvReader _csvReader;
    private readonly IMemoryCache _cache;

    public TrafficEffects(IState<TrafficState> state, ICsvReader csvReader, IMemoryCache cache)
    {
        _csvReader = csvReader;
        _cache = cache;
    }

    [EffectMethod(typeof(TrafficPageEnteredAction))]
    public async Task HandleTrafficPageEnteredAction(IDispatcher dispatcher)
    {
        if (_cache.TryGetValue("AccidentsMap", out AllAccidentsLoadedAction? cachedMapAction))
        {
            dispatcher.Dispatch(cachedMapAction);
            return;
        }

        var accidents = await _csvReader.LoadAccidentsAsync();
        var action = new AllAccidentsLoadedAction(accidents);

        _cache.Set("AccidentsMap", action);
        dispatcher.Dispatch(action);
    }

}
