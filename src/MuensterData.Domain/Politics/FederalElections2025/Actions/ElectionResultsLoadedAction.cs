using MuensterData.Domain.Politics.States;

namespace MuensterData.Domain.Politics.FederalElections2025.Actions;

public record ElectionResultsLoadedAction(
    IReadOnlyCollection<ConstituencyElectionResult> Results,
    IReadOnlyCollection<PartyElectionResults> ResultByParty,
    OverallResult OverallResult,
    Turnout OverallTurnout);