using System.Diagnostics;

namespace Lestecon.Sequencing.Abstraction.Data;

public interface ISequenceData
{
    string CorrelationKey { get; set; }

    Stopwatch Stopwatch { get; set; }
}
