using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing;

internal class SequenceTree<TSequenceContext, TSequenceData>(
    Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function)
    : ISequenceFunction<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    public SequenceTree<TSequenceContext, TSequenceData>? OnTrueFunction { get; set; }
    public SequenceTree<TSequenceContext, TSequenceData>? OnFalseFunction { get; set; }
    public SequenceTree<TSequenceContext, TSequenceData>? OnAbortFunction { get; set; }
    public SequenceTree<TSequenceContext, TSequenceData>? OnAnyFunction { get; set; }
    public Dictionary<Func<object, bool>, SequenceTree<TSequenceContext, TSequenceData>> OnValueFunctions { get; set; } = [];

    public async ValueTask<FunctionResult> Invoke(TSequenceContext sequenceContext, TSequenceData sequenceData)
    {
        var result = await function.Invoke(sequenceContext, sequenceData);

        var continuationFunction = result.Type switch
        {
            FunctionResultType.True => OnTrueFunction,
            FunctionResultType.False => OnFalseFunction,
            FunctionResultType.Abort => OnAbortFunction,
            FunctionResultType.Indeterminate => GetOnValueContinuation(result),
            _ => null
        };

        continuationFunction ??= OnAnyFunction;

        if (continuationFunction != null)
        {
            return await continuationFunction.Invoke(sequenceContext, sequenceData);
        }
        else
        {
            return result;
        }
    }

    private SequenceTree<TSequenceContext, TSequenceData>? GetOnValueContinuation(object value)
    {
        foreach (var onValueContinuations in OnValueFunctions)
        {
            if (onValueContinuations.Key(value))
            {
                return onValueContinuations.Value;
            }
        }

        return null;
    }
}
