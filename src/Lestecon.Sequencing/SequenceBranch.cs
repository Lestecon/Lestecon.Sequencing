using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing;

public class SequenceBranch<TSequenceContext, TSequenceData> :
    ISequence<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    internal SequenceBranch(Sequence<TSequenceContext, TSequenceData> sequence)
    {
        Sequence = sequence;
    }

    public string Name => Sequence.Name;

    internal TSequenceContext SequenceContext => Sequence.SequenceContext;

    private string? OnAbortFunctionName { get; set; }

    private string? OnAnyFunctionName { get; set; }

    private string? OnFalseFunctionName { get; set; }

    private string? OnTrueFunctionName { get; set; }

    private Dictionary<Func<object, bool>, string> OnValueFunctionNames { get; } = [];

    private Sequence<TSequenceContext, TSequenceData> Sequence { get; }

    public ValueTask<IFunctionResult> Invoke(
        TSequenceContext sequenceContext,
        TSequenceData sequenceData) =>
        Sequence.InvokeSequence(sequenceContext, sequenceData);

    public ValueTask<IFunctionResult> Invoke(TSequenceData sequenceData) =>
        Sequence.Invoke(sequenceData);

    internal string? GetContinuationFunctionName(IFunctionResult result)
    {
        return result == null ? null : result.Type switch
        {
            FunctionResultType.True => OnTrueFunctionName ?? OnAnyFunctionName,
            FunctionResultType.False => OnFalseFunctionName ?? OnAnyFunctionName,
            FunctionResultType.Abort => OnAbortFunctionName ?? OnAnyFunctionName,
            FunctionResultType.Ambivalent => GetValueFunctionName(result.Value) ?? OnAnyFunctionName,
            _ => OnAnyFunctionName,
        };
    }

    #region When

    /// <summary>
    /// Adds a function to the sequence using any properly defined method.
    /// The Method creates a sequence branch that will only be invoked when specified as a reaction to a result of another sequence function.
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> When(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function) =>
        Sequence.Trigger(function);

    public SequenceBranch<TSequenceContext, TSequenceData> When(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName)
    {
        ArgumentNullException.ThrowIfNull(function);

        return Sequence.Trigger(function, function.Method.Name + functionName);
    }

    /// <summary>
    /// Adds a function to the sequence by defining a sequence function class to be instantiated.
    /// The Method creates a sequence branch that will only be invoked when specified as a reaction to a result of another sequence function.
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> When<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        Sequence.Trigger<TSequenceFunction>(functionName);

    /// <summary>Adds a sequence using the specified sequence creator.</summary>
    public SequenceBranch<TSequenceContext, TSequenceData> When<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);

        return Sequence.Trigger(sequence.Invoke, sequence.Name + (functionName ?? string.Empty));
    }

    /// <summary>Adds the specified sequence.</summary>
    public SequenceBranch<TSequenceContext, TSequenceData> When(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        return sequence == null
            ? throw new ArgumentNullException(nameof(sequence))
            : Sequence.Trigger(sequence.Invoke, sequence.Name + (functionName ?? string.Empty));
    }

    #endregion When

    #region On True

    public SequenceBranch<TSequenceContext, TSequenceData> OnTrue(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnTrueFunctionName = function.Method.Name + (functionName ?? string.Empty);
        Sequence.Trigger(function, OnTrueFunctionName);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "True".
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnTrue<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        Sequence.Trigger<TSequenceFunction>(functionName);
        OnTrueFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnTrue<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);
        OnTrueFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnTrueFunctionName);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnTrue(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnTrueFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnTrueFunctionName);
        return this;
    }

    #endregion On True

    #region On False

    public SequenceBranch<TSequenceContext, TSequenceData> OnFalse(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnFalseFunctionName = function.Method.Name + (functionName ?? string.Empty);
        Sequence.Trigger(function, OnFalseFunctionName);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "False".
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnFalse<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        Sequence.Trigger<TSequenceFunction>(functionName);
        OnFalseFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnFalse<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);
        OnFalseFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnFalseFunctionName);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnFalse(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnFalseFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnFalseFunctionName);
        return this;
    }

    #endregion On False

    #region On Abort

    public SequenceBranch<TSequenceContext, TSequenceData> OnAbort(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnAbortFunctionName = function.Method.Name + (functionName ?? string.Empty);
        Sequence.Trigger(function, OnAbortFunctionName);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "Abort".
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnAbort<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        Sequence.Trigger<TSequenceFunction>(functionName);
        OnAbortFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnAbort<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);
        OnAbortFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnAbortFunctionName);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnAbort(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnAbortFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnAbortFunctionName);
        return this;
    }

    #endregion On Abort

    #region On Value

    /// <summary>
    /// Adds a reaction function to the sequence branch using any properly defined method.
    /// The specified function will be called only if the parent function's result type is "Value".
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnValue(
        Func<object, bool> predicate,
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnValueFunctionNames[predicate] = functionName ?? function.Method.Name;
        Sequence.Trigger(function, OnValueFunctionNames[predicate]);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnValue<TSequence>(
        Func<object, bool> predicate,
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);
        OnValueFunctionNames[predicate] = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnValueFunctionNames[predicate]);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "Value".
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnValue<TSequenceFunction>(
        Func<object, bool> predicate,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        Sequence.Trigger<TSequenceFunction>(functionName);
        OnValueFunctionNames[predicate] = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnValueElse<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        OnValue<TSequenceFunction>(x => true, functionName);

    public SequenceBranch<TSequenceContext, TSequenceData> OnValueElse(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null) =>
        OnValue(x => true, function, functionName);

    public SequenceBranch<TSequenceContext, TSequenceData> OnValueElse<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData> =>
        OnValue(x => true, sequenceCreator, functionName);

    #endregion On Value

    #region On Any

    /// <summary>
    /// Adds a reaction function to the sequence branch using any properly defined method.
    /// The specified function will be called on any parent function result type.
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnAny(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function)
    {
        Sequence.Trigger(function);
        OnAnyFunctionName = function.Method.Name;
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnAny(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string functionName)
    {
        Sequence.Trigger(function, functionName);
        OnAnyFunctionName = function.Method.Name + (functionName ?? string.Empty);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called on any parent function result type.
    /// </summary>
    public SequenceBranch<TSequenceContext, TSequenceData> OnAny<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        Sequence.Trigger<TSequenceFunction>(functionName);
        OnAnyFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnAny<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);
        OnAnyFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnAnyFunctionName);
        return this;
    }

    public SequenceBranch<TSequenceContext, TSequenceData> OnAny(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnAnyFunctionName = sequence.Name + (functionName ?? string.Empty);
        Sequence.Trigger(sequence.Invoke, OnAnyFunctionName);
        return this;
    }

    #endregion On Any

    private string? GetValueFunctionName(object body)
    {
        foreach (var onValueFunctionName in OnValueFunctionNames)
        {
            if (onValueFunctionName.Key(body))
            {
                return onValueFunctionName.Value;
            }
        }

        return null;
    }
}
