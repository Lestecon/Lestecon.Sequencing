using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing.Benchmark;
internal static class SequencingClasses
{
    internal abstract class WorkClass : ISequenceFunction<ISequenceContext, BenchmarkSequenceData>
    {
        public ValueTask<IFunctionResult> Invoke(ISequenceContext sequenceContext, BenchmarkSequenceData sequenceData)
        {
            //sequenceData.TestValue = (sequenceData.TestValue + 1.23) % 100;
            Thread.Sleep(1);

            return FunctionResult.True();
        }
    }

    internal abstract class DelayClass : ISequenceFunction<ISequenceContext, ISequenceData>
    {
        public async ValueTask<IFunctionResult> Invoke(ISequenceContext sequenceContext, ISequenceData sequenceData)
        {
            await Task.Delay(1);

            return FunctionResult.True();
        }
    }

    internal class WorkClass1 : WorkClass { }
    internal class WorkClass2 : WorkClass { }
    internal class WorkClass3 : WorkClass { }
    internal class WorkClass4 : WorkClass { }
    internal class WorkClass5 : WorkClass { }

    internal class DelayClass1 : DelayClass { }
    internal class DelayClass2 : DelayClass { }
    internal class DelayClass3 : DelayClass { }
    internal class DelayClass4 : DelayClass { }
    internal class DelayClass5 : DelayClass { }
}
