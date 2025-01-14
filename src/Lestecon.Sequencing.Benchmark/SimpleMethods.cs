namespace Lestecon.Sequencing.Benchmark;
internal static class SimpleMethods
{
    internal static async ValueTask<IFunctionResult> DelayMethod(ISequenceContext sequenceContext, ISequenceData sequenceData)
    {
        await Task.Delay(1);

        return FunctionResult.True();
    }

    internal static ValueTask<IFunctionResult> WorkMethod(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData)
    {
        //sequenceData.TestValue = (sequenceData.TestValue + 1.23) % 100;
        Thread.Sleep(1);

        return FunctionResult.True();
    }
}
