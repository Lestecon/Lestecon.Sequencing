namespace Lestecon.Sequencing.Benchmark;
internal static class SequencingMethods
{
    private static ValueTask<IFunctionResult> Work(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData)
    {
        //sequenceData.TestValue = (sequenceData.TestValue + 1.23) % 100;
        Thread.Sleep(1);

        return FunctionResult.True();
    }

    private static async ValueTask<IFunctionResult> Dekay(ISequenceContext sequenceContext, ISequenceData sequenceData)
    {
        await Task.Delay(1);

        return FunctionResult.True();
    }

    internal static ValueTask<IFunctionResult> Work1(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Work(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Work2(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Work(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Work3(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Work(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Work4(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Work(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Work5(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Work(sequenceContext, sequenceData);

    internal static ValueTask<IFunctionResult> Delay1(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Dekay(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Delay2(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Dekay(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Delay3(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Dekay(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Delay4(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Dekay(sequenceContext, sequenceData);
    internal static ValueTask<IFunctionResult> Delay5(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData) =>
        Dekay(sequenceContext, sequenceData);

}
