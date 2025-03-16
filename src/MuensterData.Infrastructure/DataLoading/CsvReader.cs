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

    public IEnumerable<Accident> LoadAccidents()
    {
        var rows = GetFileContent("unfaelle-muenster.csv");

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

            yield return new Accident(new(longitude, latitude), year, lightCondition, withBicycle, withCar,
                withPedestrian, withMotorcycle, withTruck, withOther);
        }
    }

    public IEnumerable<ConstituencyElectionResult> LoadFederalElectionResults2025()
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

        var rows = GetFileContent("Open-Data-05515000-Wahl-zum-Deutschen-Bundestag-Wahlbezirk.csv");

        var headlineRow = rows[0].Split(';');

        var idRowIndex = Array.IndexOf(headlineRow, "gebiet-nr");
        var constituencyNameRowIndex = Array.IndexOf(headlineRow, "gebiet-name");

        foreach (var row in rows.Skip(1))
        {
            var columns = row.Split(';');

            var id = columns[idRowIndex];
            var constituencyName = columns[constituencyNameRowIndex];

            List<PartyElectionResult> firstVotes = [];
            List<PartyElectionResult> secondVotes = [];
            for (int col = 0; col < headlineRow.Length; col++)
            {
                var content = headlineRow[col];
                if (Regex.IsMatch(content, @"^D\d+$"))
                {
                    var partyName = partyList[content];
                    var votes = int.Parse(columns[col]);
                    var firstVoteResults = new PartyElectionResult(partyName, votes);
                    firstVotes.Add(firstVoteResults);
                }
                else if (Regex.IsMatch(content, @"^F\d+$"))
                {
                    var partyName = partyList[content];
                    var votes = int.Parse(columns[col]);
                    var secondVoteResults = new PartyElectionResult(partyName, votes);
                    secondVotes.Add(secondVoteResults);
                }
            }

            yield return new ConstituencyElectionResult(id, constituencyName, firstVotes, secondVotes);
        }
    }

    private static string[] GetFileContent(string fileName)
    {
        var executionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var filePath = Path.Combine(executionDirectory!, "data", fileName);
        return File.ReadAllLines(filePath, Encoding.UTF8);
    }
}
