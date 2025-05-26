namespace Lestecon.Sequencing.Abstraction;

public interface ISequenceFunction<in TSequenceContext, in TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    string FunctionName { get; }

    ValueTask<FunctionResult> Invoke(TSequenceContext sequenceContext, TSequenceData sequenceData);
}
