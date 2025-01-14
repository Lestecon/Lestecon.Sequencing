using BenchmarkDotNet.Attributes;
using Lestecon.Sequencing.Abstraction;
using Microsoft.Extensions.Logging;

namespace Lestecon.Sequencing.Benchmark;

[SimpleJob(iterationCount: 40)]
[MemoryDiagnoser]
public class SequencingBenchmark
{
    private SequenceContext context;
    private BenchmarkSequenceData sequenceData;

    private ISequence<ISequenceContext, BenchmarkSequenceData> classSequence;
    private ISequence<ISequenceContext, BenchmarkSequenceData> methodSequence;

    [GlobalSetup]
    public void Setup()
    {
        context = new SequenceContext(new LoggerFactory().CreateLogger("test"));
        sequenceData = new BenchmarkSequenceData();

        classSequence = new Sequence<ISequenceContext, BenchmarkSequenceData>(context, "test")
            .Trigger<SequencingClasses.WorkClass1>()
                .OnTrue<SequencingClasses.DelayClass1>()
            .When<SequencingClasses.DelayClass1>()
                .OnTrue<SequencingClasses.WorkClass2>()
            .When<SequencingClasses.WorkClass2>()
                .OnTrue<SequencingClasses.DelayClass2>()
            .When<SequencingClasses.DelayClass2>()
                .OnTrue<SequencingClasses.WorkClass3>()
            .When<SequencingClasses.WorkClass3>()
                .OnTrue<SequencingClasses.DelayClass3>()
            .When<SequencingClasses.DelayClass3>()
                .OnTrue<SequencingClasses.WorkClass4>()
            .When<SequencingClasses.WorkClass4>()
                .OnTrue<SequencingClasses.DelayClass4>()
            .When<SequencingClasses.DelayClass4>()
                .OnTrue<SequencingClasses.WorkClass5>()
            .When<SequencingClasses.WorkClass5>()
                .OnTrue<SequencingClasses.DelayClass5>();

        methodSequence = new Sequence<ISequenceContext, BenchmarkSequenceData>(context, "test")
            .Trigger(SequencingMethods.Work1)
                .OnTrue(SequencingMethods.Delay1)
            .When(SequencingMethods.Delay1)
                .OnTrue(SequencingMethods.Work2)
            .When(SequencingMethods.Work2)
                .OnTrue(SequencingMethods.Delay2)
            .When(SequencingMethods.Delay2)
                .OnTrue(SequencingMethods.Work3)
            .When(SequencingMethods.Work3)
                .OnTrue(SequencingMethods.Delay3)
            .When(SequencingMethods.Delay3)
                .OnTrue(SequencingMethods.Work4)
            .When(SequencingMethods.Work4)
                .OnTrue(SequencingMethods.Delay4)
            .When(SequencingMethods.Delay4)
                .OnTrue(SequencingMethods.Work5)
            .When(SequencingMethods.Work5)
                .OnTrue(SequencingMethods.Delay5);
    }

    [Benchmark]
    public async Task Simple()
    {
        _ = await SimpleMethods.WorkMethod(context, sequenceData);
        _ = await SimpleMethods.DelayMethod(context, sequenceData);
        _ = await SimpleMethods.WorkMethod(context, sequenceData);
        _ = await SimpleMethods.DelayMethod(context, sequenceData);
        _ = await SimpleMethods.WorkMethod(context, sequenceData);
        _ = await SimpleMethods.DelayMethod(context, sequenceData);
        _ = await SimpleMethods.WorkMethod(context, sequenceData);
        _ = await SimpleMethods.DelayMethod(context, sequenceData);
        _ = await SimpleMethods.WorkMethod(context, sequenceData);
        _ = await SimpleMethods.DelayMethod(context, sequenceData);
    }

    [Benchmark]
    public async Task TestSequencingClasses() => _ = await classSequence.Invoke(sequenceData);

    [Benchmark]
    public async Task TestSequencingMethods() => _ = await methodSequence.Invoke(sequenceData);

    //[Benchmark]
    //public async Task SequencingClasses2()
    //{

    //}

    //[Benchmark]
    //public async Task SequencingMethods2()
    //{

    //}
}
