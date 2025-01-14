using System.Diagnostics;

namespace Lestecon.Sequencing.Benchmark;
internal class BenchmarkSequenceData : ISequenceData
{
    public string CorrelationKey { get; set; }

    public Stopwatch Stopwatch { get; set; }

    public double TestValue { get; set; } = 0;
}
