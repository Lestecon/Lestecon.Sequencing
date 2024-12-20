namespace Lestecon.Sequencing.Abstraction;

public interface ISequence<in TSequenceContext, in TSequenceData>
    : ISequenceFunction<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    string Name { get; }

    ValueTask<IFunctionResult> Invoke(TSequenceData sequenceData);
}
