namespace MuensterData.Domain.Politics.States;

public record PartyElectionResults(
    string PartyName,
    IReadOnlyCollection<DistrictPartyResult> PollingStationFirstVotes,
    IReadOnlyCollection<DistrictPartyResult> PollingStationSecondVotes,
    IReadOnlyCollection<DistrictPartyResult> PostalFirstVotes,
    IReadOnlyCollection<DistrictPartyResult> PostalSecondVotes);
