using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing;

public static class SequenceBuilder
{
    public static SequenceBuilder<TSequenceContext, TSequenceData> Create<TSequenceContext, TSequenceData>()
        where TSequenceContext : ISequenceContext
        where TSequenceData : ISequenceData => new();
}

public class SequenceBuilder<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    private readonly Dictionary<string, SequenceBuilderBranch<TSequenceContext, TSequenceData>> branches = [];
    private readonly Dictionary<string, Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>>> functions = [];

    internal SequenceBuilder()
    {
    }

    private string? initialFunctionName;

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> StartWith<TSequenceFunction>(
        TSequenceFunction sequenceFunction,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData> =>
        Register(sequenceFunction, functionName);

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> StartWith<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        Register<TSequenceFunction>(functionName);

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> StartWith(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function) =>
        Register(function);

    public SequenceBuilderBranch<TSequenceContext, TSequenceData> StartWith(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function,
        string functionName) =>
        Register(function, functionName);

    internal SequenceBuilderBranch<TSequenceContext, TSequenceData> Register<TSequenceFunction>(
        TSequenceFunction sequenceFunction,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData> =>
        Register(sequenceFunction.Invoke, typeof(TSequenceFunction).Name + (functionName ?? string.Empty));

    internal SequenceBuilderBranch<TSequenceContext, TSequenceData> Register<TSequenceFunction>(
        string? functionName)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        Register(new TSequenceFunction().Invoke, typeof(TSequenceFunction).Name + (functionName ?? string.Empty));

    internal SequenceBuilderBranch<TSequenceContext, TSequenceData> Register(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function)
    {
        ArgumentNullException.ThrowIfNull(function);

        return Register(function, function.Method.Name);
    }

    internal SequenceBuilderBranch<TSequenceContext, TSequenceData> Register(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function,
        string functionName)
    {
        ArgumentNullException.ThrowIfNull(function);

        if (functions.Count == 0)
        {
            initialFunctionName = functionName;
        }

        functions.TryAdd(functionName, function);

        if (!branches.TryGetValue(functionName, out var branch))
        {
            branch = new SequenceBuilderBranch<TSequenceContext, TSequenceData>(this);
            branches.Add(functionName, branch);
        }

        return branch;
    }

    public ISequenceFunction<TSequenceContext, TSequenceData> Build()
    {
        if (string.IsNullOrWhiteSpace(initialFunctionName)
            || !functions.TryGetValue(initialFunctionName, out var initialFunction))
        {
            throw new InvalidOperationException("No initial function present");
        }

        return BuildTree(initialFunctionName, initialFunction);
    }

    private SequenceTree<TSequenceContext, TSequenceData> BuildTree(string functionName, Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function)
    {
        var tree = new SequenceTree<TSequenceContext, TSequenceData>(function);

        if (branches.TryGetValue(functionName, out var sequenceBuilderBranch))
        {
            tree.OnTrueFunction = GetFunctionAndBuildTree(sequenceBuilderBranch.OnTrueFunctionName);
            tree.OnFalseFunction = GetFunctionAndBuildTree(sequenceBuilderBranch.OnFalseFunctionName);
            tree.OnAbortFunction = GetFunctionAndBuildTree(sequenceBuilderBranch.OnAbortFunctionName);
            tree.OnAnyFunction = GetFunctionAndBuildTree(sequenceBuilderBranch.OnAnyFunctionName);

            foreach (var keyValuePair in sequenceBuilderBranch.OnValueFunctionNames)
            {
                var onValueFunction = GetFunctionAndBuildTree(keyValuePair.Value);

                if (onValueFunction != null)
                {
                    tree.OnValueFunctions.Add(keyValuePair.Key, onValueFunction);
                }
            }
        }

        return tree;
    }

    private SequenceTree<TSequenceContext, TSequenceData>? GetFunctionAndBuildTree(string? functionName)
    {
        if (!string.IsNullOrWhiteSpace(functionName)
            && functions.TryGetValue(functionName, out var function))
        {
            return BuildTree(functionName, function);
        }

        return null;
    }
}
