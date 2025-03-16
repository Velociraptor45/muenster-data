using MuensterData.Domain.Politics.States;
using MuensterData.Domain.Traffic.States;

namespace MuensterData.Domain.Common.Ports;
public interface ICsvReader
{
    IEnumerable<Accident> LoadAccidents();
    IEnumerable<ConstituencyElectionResult> LoadFederalElectionResults2025();
}
