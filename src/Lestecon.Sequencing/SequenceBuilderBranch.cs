using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing;

public class SequenceBuilderBranch<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    private readonly SequenceBuilder<TSequenceContext, TSequenceData> builder;

    internal SequenceBuilderBranch(
        SequenceBuilder<TSequenceContext, TSequenceData> builder)
    {
        this.builder = builder;
    }

    public string? OnAbortFunctionName { get; private set; }
    public string? OnAnyFunctionName { get; private set; }
    public string? OnFalseFunctionName { get; private set; }
    public string? OnTrueFunctionName { get; private set; }
    public Dictionary<Func<object, bool>, string> OnValueFunctionNames { get; } = [];

    public ISequenceFunction<TSequenceContext, TSequenceData> Build() => builder.Build();

    #region After

    /// <summary>
    /// Adds a function to the sequence using any properly defined method.
    /// The Method creates a sequence branch that will only be invoked when specified as a reaction to a result of another sequence function.
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> After(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function) =>
        builder.Register(function);

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> After(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName)
    {
        ArgumentNullException.ThrowIfNull(function);

        return builder.Register(function, function.Method.Name + functionName);
    }

    /// <summary>
    /// Adds a function to the sequence by defining a sequence function class to be instantiated.
    /// The Method creates a sequence branch that will only be invoked when specified as a reaction to a result of another sequence function.
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> After<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        builder.Register<TSequenceFunction>(functionName);

    /// <summary>Adds the specified sequence.</summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> After(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        return sequence == null
            ? throw new ArgumentNullException(nameof(sequence))
            : builder.Register(sequence.Invoke, sequence.Name + (functionName ?? string.Empty));
    }

    #endregion

    #region On True

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfTrueRun(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnTrueFunctionName = function.Method.Name + (functionName ?? string.Empty);
        builder.Register(function, OnTrueFunctionName);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "True".
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfTrueRun<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        builder.Register<TSequenceFunction>(functionName);
        OnTrueFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfTrueRun(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnTrueFunctionName = sequence.Name + (functionName ?? string.Empty);
        builder.Register(sequence.Invoke, OnTrueFunctionName);
        return this;
    }

    #endregion On True

    #region On False

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfFalseRun(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnFalseFunctionName = function.Method.Name + (functionName ?? string.Empty);
        builder.Register(function, OnFalseFunctionName);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "False".
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfFalseRun<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        builder.Register<TSequenceFunction>(functionName);
        OnFalseFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfFalseRun(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnFalseFunctionName = sequence.Name + (functionName ?? string.Empty);
        builder.Register(sequence.Invoke, OnFalseFunctionName);
        return this;
    }

    #endregion On False

    #region On Abort

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAbortRun(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnAbortFunctionName = function.Method.Name + (functionName ?? string.Empty);
        builder.Register(function, OnAbortFunctionName);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "Abort".
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAbortRun<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        builder.Register<TSequenceFunction>(functionName);
        OnAbortFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAbortRun(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnAbortFunctionName = sequence.Name + (functionName ?? string.Empty);
        builder.Register(sequence.Invoke, OnAbortFunctionName);
        return this;
    }

    #endregion On Abort

    #region On Value

    /// <summary>
    /// Adds a reaction function to the sequence branch using any properly defined method.
    /// The specified function will be called only if the parent function's result type is "Value".
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfValueRun(
        Func<object, bool> predicate,
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(function);

        OnValueFunctionNames[predicate] = functionName ?? function.Method.Name;
        builder.Register(function, OnValueFunctionNames[predicate]);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called only if the parent function's result type is "Value".
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfValueRun<TSequenceFunction>(
        Func<object, bool> predicate,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        builder.Register<TSequenceFunction>(functionName);
        OnValueFunctionNames[predicate] = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfValueRun<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        IfValueRun<TSequenceFunction>(x => true, functionName);

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfValueElseRun(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string? functionName = null) =>
        IfValueRun(x => true, function, functionName);

    #endregion On Value

    #region On Any

    /// <summary>
    /// Adds a reaction function to the sequence branch using any properly defined method.
    /// The specified function will be called on any parent function result type.
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAnyRun(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function)
    {
        builder.Register(function);
        OnAnyFunctionName = function.Method.Name;
        return this;
    }

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAnyRun(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string functionName)
    {
        builder.Register(function, functionName);
        OnAnyFunctionName = function.Method.Name + (functionName ?? string.Empty);
        return this;
    }

    /// <summary>
    /// Adds a reaction function to the sequence branch by defining a sequence function class to be instantiated.
    /// The specified function will be called on any parent function result type.
    /// </summary>
    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAnyRun<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        builder.Register<TSequenceFunction>(functionName);
        OnAnyFunctionName = typeof(TSequenceFunction).Name + (functionName ?? string.Empty);
        return this;
    }

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> IfAnyRun(
        ISequence<TSequenceContext, TSequenceData> sequence,
        string? functionName = null)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        OnAnyFunctionName = sequence.Name + (functionName ?? string.Empty);
        builder.Register(sequence.Invoke, OnAnyFunctionName);
        return this;
    }

    #endregion On Any
}
