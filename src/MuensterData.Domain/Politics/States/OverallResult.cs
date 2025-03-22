namespace MuensterData.Domain.Politics.States;

public record OverallResult(
    IReadOnlyCollection<OverallPartyResult> FirstVoteResults,
    IReadOnlyCollection<OverallPartyResult> SecondVoteResults);