using Fluxor;
using MuensterData.Domain.Common.Ports;
using MuensterData.Domain.Politics.FederalElections2025.Actions;
using MuensterData.Domain.Politics.FederalElections2025.States;
using System.Reflection;
using System.Text.Json;

namespace MuensterData.Domain.Politics.FederalElections2025.Effects;

public class FederalElection2025Effects
{
    private readonly IState<FederalElection2025State> _state;
    private readonly ICsvReader _csvReader;

    public FederalElection2025Effects(IState<FederalElection2025State> state, ICsvReader csvReader)
    {
        _state = state;
        _csvReader = csvReader;
    }

    [EffectMethod(typeof(LoadPageAction))]
    public async Task HandleLoadPageAction(IDispatcher dispatcher)
    {
        if (_state.Value.ConstituencyPolygonMap is not null)
            return;

        var executionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var filePath = Path.Combine(executionDirectory!, "data", "stimmbezirk.geojson");
        var serializedMap = await File.ReadAllTextAsync(filePath);
        var constituencyPolygonMap = JsonSerializer.Deserialize<object>(serializedMap)!;
        dispatcher.Dispatch(new ConstituencyPolygonMapLoadedAction(constituencyPolygonMap));

        var electionResults = await _csvReader.LoadFederalElectionResults2025Async();
        dispatcher.Dispatch(new ElectionResultsLoadedAction(electionResults));
    }
}
