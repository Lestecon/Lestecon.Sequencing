using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing;

internal class SequenceBranch<TSequenceContext, TSequenceData>
    : ISequenceFunction<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    private readonly Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function;
    private readonly string branchName;

    public SequenceBranch(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function,
        string? branchName = null)
    {
        this.function = function;
        this.branchName = branchName ?? Guid.NewGuid().ToString();
    }

    public SequenceBranch<TSequenceContext, TSequenceData>? OnTrueFunction { get; set; }
    public SequenceBranch<TSequenceContext, TSequenceData>? OnFalseFunction { get; set; }
    public SequenceBranch<TSequenceContext, TSequenceData>? OnAbortFunction { get; set; }
    public SequenceBranch<TSequenceContext, TSequenceData>? OnAnyFunction { get; set; }
    public Dictionary<Func<object, bool>, SequenceBranch<TSequenceContext, TSequenceData>> OnValueFunctions { get; set; } = [];

    public string GetFunctionName() => branchName;

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

    private SequenceBranch<TSequenceContext, TSequenceData>? GetOnValueContinuation(object result)
    {
        foreach (var onValueContinuations in OnValueFunctions)
        {
            if (onValueContinuations.Key(result))
            {
                return onValueContinuations.Value;
            }
        }

        return null;
    }
}
