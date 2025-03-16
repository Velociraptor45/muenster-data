using Fluxor;
using MuensterData.Domain.Politics.FederalElections2025.Actions;
using MuensterData.Domain.Politics.FederalElections2025.States;

namespace MuensterData.Domain.Politics.FederalElections2025.Reducers;

public static class FederalElection2025Reducer
{
    [ReducerMethod]
    public static FederalElection2025State OnConstituencyPolygonMapLoaded(FederalElection2025State state,
        ConstituencyPolygonMapLoadedAction action)
    {
        return state with
        {
            ConstituencyPolygonMap = action.ConstituencyPolygonMap
        };
    }

    [ReducerMethod]
    public static FederalElection2025State OnElectionResultsLoaded(FederalElection2025State state,
        ElectionResultsLoadedAction action)
    {
        return state with
        {
            Results = action.Results
        };
    }
}
