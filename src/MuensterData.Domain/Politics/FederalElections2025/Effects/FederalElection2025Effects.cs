using Fluxor;
using MuensterData.Domain.Common.Ports;
using MuensterData.Domain.Politics.FederalElections2025.Actions;
using MuensterData.Domain.Politics.FederalElections2025.States;
using MuensterData.Domain.Politics.States;
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

        var (electionResults, overallTurnout) = await _csvReader.LoadFederalElectionResults2025Async();

        var dict = new Dictionary<string, (List<DistrictPartyResult>, List<DistrictPartyResult>)>();
        var postalDict = new Dictionary<string, (List<DistrictPartyResult>, List<DistrictPartyResult>)>();
        foreach (var result in electionResults)
        {
            foreach (var firstVoteResult in result.FirstVote)
            {
                if (!dict.ContainsKey(firstVoteResult.Name))
                {
                    dict[firstVoteResult.Name] = ([], []);
                    postalDict[firstVoteResult.Name] = ([], []);
                }

                var districtResult = new DistrictPartyResult(result.Name, firstVoteResult.Votes, firstVoteResult.Percentage);
                if (result.IsPostal)
                    postalDict[firstVoteResult.Name].Item1.Add(districtResult);
                else
                    dict[firstVoteResult.Name].Item1.Add(districtResult);
            }
            foreach (var secondVoteResult in result.SecondVote)
            {
                if (!dict.ContainsKey(secondVoteResult.Name))
                {
                    dict[secondVoteResult.Name] = ([], []);
                    postalDict[secondVoteResult.Name] = ([], []);
                }

                var districtResult = new DistrictPartyResult(result.Name, secondVoteResult.Votes, secondVoteResult.Percentage);
                if (result.IsPostal)
                    postalDict[secondVoteResult.Name].Item2.Add(districtResult);
                else
                    dict[secondVoteResult.Name].Item2.Add(districtResult);
            }
        }

        var resultsByParty = new List<PartyElectionResults>();
        foreach (var partyName in dict.Keys)
        {
            var firstVotes = dict[partyName].Item1.OrderByDescending(x => x.Votes).ThenBy(x => x.DistrictName).ToArray();
            var secondVotes = dict[partyName].Item2.OrderByDescending(x => x.Votes).ThenBy(x => x.DistrictName).ToArray();
            var postalFirstVotes = postalDict[partyName].Item1.OrderByDescending(x => x.Votes).ThenBy(x => x.DistrictName).ToArray();
            var postalSecondVotes = postalDict[partyName].Item2.OrderByDescending(x => x.Votes).ThenBy(x => x.DistrictName).ToArray();
            resultsByParty.Add(new PartyElectionResults(partyName, firstVotes, secondVotes, postalFirstVotes, postalSecondVotes));
        }

        var totalFirstVotes = electionResults.Sum(x => x.FirstVote.Sum(y => y.Votes));
        var totalSecondVotes = electionResults.Sum(x => x.SecondVote.Sum(y => y.Votes));

        var overallPartyResultsFirstVote = new List<OverallPartyResult>();
        var overallPartyResultsSecondVote = new List<OverallPartyResult>();
        foreach (var partyName in dict.Keys)
        {
            var partyFirstVotes = dict[partyName].Item1.Sum(x => x.Votes) + postalDict[partyName].Item1.Sum(x => x.Votes);
            var percentageFirstVotes = (decimal)partyFirstVotes / totalFirstVotes * 100;
            overallPartyResultsFirstVote.Add(new OverallPartyResult(partyName, percentageFirstVotes));

            var partySecondVotes = dict[partyName].Item2.Sum(x => x.Votes) + postalDict[partyName].Item2.Sum(x => x.Votes);
            var percentageSecondVotes = (decimal)partySecondVotes / totalSecondVotes * 100;
            overallPartyResultsSecondVote.Add(new OverallPartyResult(partyName, percentageSecondVotes));
        }

        var overallResult = new OverallResult(overallPartyResultsFirstVote, overallPartyResultsSecondVote);

        dispatcher.Dispatch(new ElectionResultsLoadedAction(electionResults, resultsByParty, overallResult, overallTurnout));
    }
}
