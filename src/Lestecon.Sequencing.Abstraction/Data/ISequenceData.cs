using System.Diagnostics;

namespace Lestecon.Sequencing;

public interface ISequenceData
{
    string CorrelationKey { get; set; }

    Stopwatch Stopwatch { get; set; }
}
