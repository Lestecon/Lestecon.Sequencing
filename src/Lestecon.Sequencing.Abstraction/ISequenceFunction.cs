namespace Lestecon.Sequencing.Abstraction;

public interface ISequenceFunction<in TSequenceContext, in TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    ValueTask<IFunctionResult> Invoke(TSequenceContext sequenceContext, TSequenceData sequenceData);
}
