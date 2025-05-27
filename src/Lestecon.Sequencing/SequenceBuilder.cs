using Lestecon.Sequencing.Abstraction;

namespace Lestecon.Sequencing;

public static class SequenceBuilder
{
    public static SequenceBuilder<TSequenceContext, TSequenceData> Create<TSequenceContext, TSequenceData>(
        string? sequenceName = null)
        where TSequenceContext : ISequenceContext
        where TSequenceData : ISequenceData => new(sequenceName);
}

public class SequenceBuilder<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    private readonly Dictionary<string, SequenceBranchDefinition<TSequenceContext, TSequenceData>> branchDefinitions = [];
    private readonly Dictionary<string, Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>>> functions = [];

    private readonly string? sequenceName;
    private string? initialFunctionName;


    internal SequenceBuilder(string? sequenceName = null)
    {
        this.sequenceName = sequenceName;
    }

    public SequenceBranchDefinition<TSequenceContext, TSequenceData> StartWith<TSequenceFunction>(
        TSequenceFunction sequenceFunction,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData> =>
        Register(sequenceFunction, functionName);

    public SequenceBranchDefinition<TSequenceContext, TSequenceData> StartWith<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        Register<TSequenceFunction>(functionName);

    public SequenceBranchDefinition<TSequenceContext, TSequenceData> StartWith(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function) =>
        Register(function);

    public SequenceBranchDefinition<TSequenceContext, TSequenceData> StartWith(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function,
        string functionName) =>
        Register(function, functionName);

    internal SequenceBranchDefinition<TSequenceContext, TSequenceData> Register<TSequenceFunction>(
        TSequenceFunction sequenceFunction,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData> =>
        Register(
            sequenceFunction.Invoke,
            sequenceFunction.GetFunctionName() + (functionName ?? string.Empty));

    internal SequenceBranchDefinition<TSequenceContext, TSequenceData> Register<TSequenceFunction>(
        string? functionName)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new()
    {
        var sequenceFunction = new TSequenceFunction();
        return Register(
            sequenceFunction.Invoke,
            sequenceFunction.GetFunctionName() + (functionName ?? string.Empty));
    }

    internal SequenceBranchDefinition<TSequenceContext, TSequenceData> Register(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function)
    {
        ArgumentNullException.ThrowIfNull(function);

        return Register(function, function.Method.Name);
    }

    internal SequenceBranchDefinition<TSequenceContext, TSequenceData> Register(
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function,
        string functionName)
    {
        ArgumentNullException.ThrowIfNull(function);

        if (functions.Count == 0)
        {
            initialFunctionName = functionName;
        }

        functions.TryAdd(functionName, function);

        if (!branchDefinitions.TryGetValue(functionName, out var branch))
        {
            branch = new SequenceBranchDefinition<TSequenceContext, TSequenceData>(this);
            branchDefinitions.Add(functionName, branch);
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

        return BuildBranch(initialFunctionName, initialFunction, sequenceName);
    }

    private SequenceBranch<TSequenceContext, TSequenceData> BuildBranch(
        string functionName,
        Func<TSequenceContext, TSequenceData, ValueTask<FunctionResult>> function,
        string? branchName = null)
    {
        var branch = new SequenceBranch<TSequenceContext, TSequenceData>(function, branchName);

        if (branchDefinitions.TryGetValue(functionName, out var sequenceBuilderBranch))
        {
            branch.OnTrueFunction = GetFunctionAndBuildBranch(sequenceBuilderBranch.OnTrueFunctionName);
            branch.OnFalseFunction = GetFunctionAndBuildBranch(sequenceBuilderBranch.OnFalseFunctionName);
            branch.OnAbortFunction = GetFunctionAndBuildBranch(sequenceBuilderBranch.OnAbortFunctionName);
            branch.OnAnyFunction = GetFunctionAndBuildBranch(sequenceBuilderBranch.OnAnyFunctionName);

            foreach (var keyValuePair in sequenceBuilderBranch.OnValueFunctionNames)
            {
                var onValueFunction = GetFunctionAndBuildBranch(keyValuePair.Value);

                if (onValueFunction != null)
                {
                    branch.OnValueFunctions.Add(keyValuePair.Key, onValueFunction);
                }
            }
        }

        return branch;
    }

    private SequenceBranch<TSequenceContext, TSequenceData>? GetFunctionAndBuildBranch(string? functionName)
    {
        if (!string.IsNullOrWhiteSpace(functionName)
            && functions.TryGetValue(functionName, out var function))
        {
            return BuildBranch(functionName, function);
        }

        return null;
    }
}
