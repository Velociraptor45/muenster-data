using Fluxor;
using MuensterData.Domain.Politics.States;

namespace MuensterData.Domain.Politics.FederalElections2025.States;

public record FederalElection2025State(
    object? ConstituencyPolygonMap,
    OverallResult OverallResult,
    Turnout OverallTurnout,
    IReadOnlyCollection<ConstituencyElectionResult> Results,
    IReadOnlyCollection<PartyElectionResults> ResultByParty,
    ElectionSettings Settings,
    ConstituencyElectionResult? SelectedPollingStation,
    ConstituencyElectionResult? SelectedPostalDistrict,
    PartyElectionResults? SelectedParty)
{
    public IReadOnlyCollection<ConstituencyElectionResult> PostalResults => Results.Where(r => r.IsPostal).ToArray();
    public IReadOnlyCollection<ConstituencyElectionResult> PollingStationResults => Results.Where(r => !r.IsPostal).ToArray();
}

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
            new([], []),
            new(1, 1),
            [],
            [],
            new ElectionSettings(false, true, false),
            null,
            null,
            null);
    }
}