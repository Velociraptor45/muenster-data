using MuensterData.Domain.Politics.States;

namespace MuensterData.Domain.Politics.FederalElections2025.Actions;

public record SelectedPollingStationChangedAction(ConstituencyElectionResult PollingStation);