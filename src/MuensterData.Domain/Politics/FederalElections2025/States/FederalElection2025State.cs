using Fluxor;
using MuensterData.Domain.Politics.States;

namespace MuensterData.Domain.Politics.FederalElections2025.States;

public record FederalElection2025State(
    object? ConstituencyPolygonMap,
    IReadOnlyCollection<ConstituencyElectionResult> Results,
    ConstituencyMapSettings ConstituencyMapSettings);

public class FederalElection2025FeatureState : Feature<FederalElection2025State>
{
    public override string GetName()
    {
        return nameof(FederalElection2025State);
    }
    protected override FederalElection2025State GetInitialState()
    {
        return new FederalElection2025State(
            null,
            [],
            new ConstituencyMapSettings(false));
    }
}