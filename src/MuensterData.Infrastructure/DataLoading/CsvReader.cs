using MuensterData.Domain.Common.Ports;
using MuensterData.Domain.Politics.States;
using MuensterData.Domain.Traffic.States;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MuensterData.Infrastructure.DataLoading;
public class CsvReader : ICsvReader
{
    private static readonly CultureInfo EnglishCulture = new("en-en");
    private const string One = "1";
    private const string PostalVotePattern = @"^\d+ - Brief ";
    private const string RegularVotePattern = @"^\d+ - ";


    public async Task<List<Accident>> LoadAccidentsAsync()
    {
        var rows = await GetFileContentAsync("unfaelle-muenster.csv");

        const int longRowIndex = 20;
        const int latRowIndex = 21;
        const int yearRowIndex = 4;
        const int lightConditionRowIndex = 11;
        const int withBicycleRowIndex = 12;
        const int withCarRowIndex = 13;
        const int withPedestrianRowIndex = 14;
        const int withMotorcycleRowIndex = 15;
        const int withTruckRowIndex = 16;
        const int withOtherRowIndex = 17;

        var list = new List<Accident>();
        foreach (var row in rows.Skip(1))
        {
            var columns = row.Split(',');

            var longitude = double.Parse(columns[longRowIndex], EnglishCulture);
            var latitude = double.Parse(columns[latRowIndex], EnglishCulture);
            var year = int.Parse(columns[yearRowIndex]);
            var lightCondition = int.Parse(columns[lightConditionRowIndex]);
            var withBicycle = columns[withBicycleRowIndex] == One;
            var withCar = columns[withCarRowIndex] == One;
            var withPedestrian = columns[withPedestrianRowIndex] == One;
            var withMotorcycle = columns[withMotorcycleRowIndex] == One;
            var withTruck = columns[withTruckRowIndex] == One;
            var withOther = columns[withOtherRowIndex] == One;

            list.Add(new Accident(new(longitude, latitude), year, lightCondition, withBicycle, withCar,
                withPedestrian, withMotorcycle, withTruck, withOther));
        }

        return list;
    }

    public async Task<(List<ConstituencyElectionResult>, Turnout)> LoadFederalElectionResults2025Async()
    {
        var partyList = new Dictionary<string, string>()
        {
            {"D1", "SPD"},
            {"D2", "CDU"},
            {"D3", "GRÜNE"},
            {"D4", "FDP"},
            {"D5", "AfD"},
            {"D6", "Linke"},
            {"D7", "PARTEI MENSCH UMWELT TIERSCHUTZ"},
            {"D8", "Partei für Arbeit, Rechtsstaat, Tierschutz, Elitenförderung und basisdemokratische Initiative"},
            {"D9", "Basisdemokratische Partei Deutschland"},
            {"D10", "Die Gerechtigkeitspartei – Team Todenhöfer"},
            {"D11", "FREIE WÄHLER"},
            {"D12", "Volt"},
            {"D13", "Marxistisch-Leninistische Partei Deutschlands"},
            {"D14", "Partei des Fortschritts"},
            {"D15", "BÜNDNIS DEUTSCHLAND"},
            {"D16", "BSW"},
            {"D17", "MERA25 - Gemeinsam für Europäische Unabhängigkeit"},
            {"D18", "WerteUnion"},
            {"F1", "SPD"},
            {"F2", "CDU"},
            {"F3", "GRÜNE"},
            {"F4", "FDP"},
            {"F5", "AfD"},
            {"F6", "Linke"},
            {"F7", "PARTEI MENSCH UMWELT TIERSCHUTZ"},
            {"F8", "Partei für Arbeit, Rechtsstaat, Tierschutz, Elitenförderung und basisdemokratische Initiative"},
            {"F9", "Basisdemokratische Partei Deutschland"},
            {"F10", "Die Gerechtigkeitspartei – Team Todenhöfer"},
            {"F11", "FREIE WÄHLER"},
            {"F12", "Volt"},
            {"F13", "Marxistisch-Leninistische Partei Deutschlands"},
            {"F14", "Partei des Fortschritts"},
            {"F15", "BÜNDNIS DEUTSCHLAND"},
            {"F16", "BSW"},
            {"F17", "MERA25 - Gemeinsam für Europäische Unabhängigkeit"},
            {"F18", "WerteUnion"}
        };

        var rows = await GetFileContentAsync("Open-Data-05515000-Wahl-zum-Deutschen-Bundestag-Wahlbezirk.csv");

        var headlineRow = rows[0].Split(';');

        var idRowIndex = Array.IndexOf(headlineRow, "gebiet-nr");
        var constituencyNameRowIndex = Array.IndexOf(headlineRow, "gebiet-name");
        var totalEligibleVotersRowIndex = Array.IndexOf(headlineRow, "A");
        var pollingStationEligibleVotersRowIndex = Array.IndexOf(headlineRow, "A1");
        var totalVotersRowIndex = Array.IndexOf(headlineRow, "B");
        var validFirstVotesCountRowIndex = Array.IndexOf(headlineRow, "D");
        var validSecondVotesCountRowIndex = Array.IndexOf(headlineRow, "F");

        var list = new List<ConstituencyElectionResult>();
        var totalEligibleVoters = 0;
        var totalVoters = 0;
        foreach (var row in rows.Skip(1))
        {
            var columns = row.Split(';');

            var id = columns[idRowIndex];
            var constituencyName = columns[constituencyNameRowIndex];

            var validFirstVotesCount = (decimal)int.Parse(columns[validFirstVotesCountRowIndex]);
            var validSecondVotesCount = (decimal)int.Parse(columns[validSecondVotesCountRowIndex]);

            List<PartyElectionResult> firstVotes = [];
            List<PartyElectionResult> secondVotes = [];
            for (int col = 0; col < headlineRow.Length; col++)
            {
                var content = headlineRow[col];
                if (IsFirstVote(content))
                {
                    var partyName = partyList[content];
                    var votes = int.Parse(columns[col]);
                    var firstVoteResults = new PartyElectionResult(partyName, votes, votes / validFirstVotesCount * 100);
                    firstVotes.Add(firstVoteResults);
                }
                else if (IsSecondVote(content))
                {
                    var partyName = partyList[content];
                    var votes = int.Parse(columns[col]);
                    var secondVoteResults = new PartyElectionResult(partyName, votes, votes / validSecondVotesCount * 100);
                    secondVotes.Add(secondVoteResults);
                }
            }

            bool isPostalVote = false;
            Turnout? turnout = null;
            if (Regex.IsMatch(constituencyName, PostalVotePattern))
            {
                isPostalVote = true;
                constituencyName = Regex.Replace(constituencyName, PostalVotePattern, string.Empty);
                totalVoters += int.Parse(columns[totalVotersRowIndex]);
            }
            else
            {
                totalEligibleVoters += int.Parse(columns[totalEligibleVotersRowIndex]);
                var pollingStationEligibleVoters = int.Parse(columns[pollingStationEligibleVotersRowIndex]);
                var voters = int.Parse(columns[totalVotersRowIndex]);
                totalVoters += voters;
                turnout = new Turnout(pollingStationEligibleVoters, voters);

                constituencyName = Regex.Replace(constituencyName, RegularVotePattern, string.Empty);
            }

            list.Add(new ConstituencyElectionResult(id, constituencyName, isPostalVote, firstVotes, secondVotes, turnout));
        }
        return (list, new Turnout(totalEligibleVoters, totalVoters));

        bool IsFirstVote(string content) => Regex.IsMatch(content, @"^D\d+$");
        bool IsSecondVote(string content) => Regex.IsMatch(content, @"^F\d+$");
    }

    private static async Task<string[]> GetFileContentAsync(string fileName)
    {
        var executionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var filePath = Path.Combine(executionDirectory!, "data", fileName);
        return await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
    }
}
