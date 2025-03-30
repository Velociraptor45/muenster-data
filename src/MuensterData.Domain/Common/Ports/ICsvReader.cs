using MuensterData.Domain.Politics.States;
using MuensterData.Domain.Traffic.States;

namespace MuensterData.Domain.Common.Ports;
public interface ICsvReader
{
    Task<List<Accident>> LoadAccidentsAsync();
    Task<(List<ConstituencyElectionResult>, Turnout)> LoadFederalElectionResults2025Async();
}
