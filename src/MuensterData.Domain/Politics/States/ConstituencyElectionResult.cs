namespace MuensterData.Domain.Politics.States;

public record ConstituencyElectionResult(string Id, string Name, bool IsPostal,
    IReadOnlyCollection<PartyElectionResult> FirstVote, IReadOnlyCollection<PartyElectionResult> SecondVote)
{
    public string WinnerFirstVote => FirstVote.MaxBy(p => p.Votes)!.Name;
    public string WinnerSecondVote => SecondVote.MaxBy(p => p.Votes)!.Name;
}