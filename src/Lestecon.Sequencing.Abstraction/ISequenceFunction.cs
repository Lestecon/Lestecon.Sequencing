using Lestecon.Sequencing.Abstraction.Context;
using Lestecon.Sequencing.Abstraction.Data;

namespace Lestecon.Sequencing.Abstraction;

public interface ISequenceFunction<in TSequenceContext, in TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    ValueTask<IFunctionResult> Invoke(TSequenceContext sequenceContext, TSequenceData sequenceData);
}
