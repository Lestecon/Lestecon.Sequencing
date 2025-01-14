using FluentResults;
using Lestecon.Sequencing.Abstraction;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Lestecon.Sequencing;

public class Sequence<TSequenceContext, TSequenceData> :
    ISequence<TSequenceContext, TSequenceData>
    where TSequenceContext : ISequenceContext
    where TSequenceData : ISequenceData
{
    /// <summary>The initiation function name.</summary>
    private string? initiationFunctionName;

    public Sequence(TSequenceContext sequenceContext, string sequenceName)
    {
        SequenceContext = sequenceContext;
        Logger = SequenceContext.Logger;
        Name = sequenceName;
    }

    /// <summary>Gets the name.</summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>Gets the sequence context.</summary>
    /// <value>The sequence context.</value>
    internal TSequenceContext SequenceContext { get; }

    /// <summary>Gets the logger.</summary>
    /// <value>The logger.</value>
    protected ILogger Logger { get; }

    /// <summary>Gets the sequence branches.</summary>
    /// <value>The sequence branches.</value>
    private Dictionary<string, SequenceBranch<TSequenceContext, TSequenceData>> SequenceBranches { get; } = [];

    /// <summary>Gets the sequence functions.</summary>
    /// <value>The sequence functions.</value>
    private Dictionary<string, Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>>> SequenceFunctions { get; } = [];

    /// <summary>
    /// Invokes the current sequence as a sequence function with the specified sequence context and sequence data.
    /// </summary>
    /// <param name="sequenceContext">The sequence context.</param>
    /// <param name="sequenceData">The sequence data.</param>
    /// <returns></returns>
    public ValueTask<IFunctionResult> Invoke(TSequenceContext sequenceContext, TSequenceData sequenceData) =>
        InvokeSequence(sequenceContext, sequenceData);

    /// <summary>Invokes the current sequence with the specified sequence data.</summary>
    /// <param name="sequenceData">The sequence data.</param>
    /// <returns></returns>
    public ValueTask<IFunctionResult> Invoke(TSequenceData sequenceData) =>
        InvokeSequence(SequenceContext, sequenceData);

    /// <summary>
    /// Adds a function to the sequence using any properly defined method.
    /// Only the first function added by this method will be used.
    /// </summary>
    /// <param name="function">The function.</param>
    /// <returns></returns>
    public SequenceBranch<TSequenceContext, TSequenceData> Trigger(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function)
    {
        ArgumentNullException.ThrowIfNull(function);

        return Trigger(function, function.Method.Name);
    }

    /// <summary>
    /// Adds a function to the sequence by defining a sequence function class to be instantiated.
    /// Only the first function added by this method will be used.
    /// </summary>
    /// <typeparam name="TSequenceFunction">The type of the sequence function.</typeparam>
    /// <param name="functionName">Name of the function.</param>
    /// <returns></returns>
    public SequenceBranch<TSequenceContext, TSequenceData> Trigger<TSequenceFunction>(
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData>, new() =>
        Trigger(new TSequenceFunction().Invoke, typeof(TSequenceFunction).Name + (functionName ?? string.Empty));

    /// <summary>
    /// Adds a function to the sequence using an instantiated sequence function class.
    /// Only the first function added by this method will be used.
    /// </summary>
    /// <typeparam name="TSequenceFunction">The type of the sequence function.</typeparam>
    /// <param name="sequenceFunction">The sequence function.</param>
    /// <param name="functionName">Name of the function.</param>
    /// <returns></returns>
    public SequenceBranch<TSequenceContext, TSequenceData> Trigger<TSequenceFunction>(
        TSequenceFunction sequenceFunction,
        string? functionName = null)
        where TSequenceFunction : ISequenceFunction<TSequenceContext, TSequenceData> =>
        Trigger(sequenceFunction.Invoke, typeof(TSequenceFunction).Name + (functionName ?? string.Empty));

    public SequenceBranch<TSequenceContext, TSequenceData> Trigger<TSequence>(
        Func<TSequenceContext, TSequence> sequenceCreator,
        string? functionName = null)
        where TSequence : ISequence<TSequenceContext, TSequenceData>
    {
        ArgumentNullException.ThrowIfNull(sequenceCreator);

        var sequence = sequenceCreator(SequenceContext);

        return Trigger(sequence.Invoke, sequence.Name + (functionName ?? string.Empty));
    }

    /// <summary>Adds a function to the sequence. Only the first function added by this method will be used.</summary>
    /// <param name="function">The function.</param>
    /// <param name="functionName">Name of the function.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">function.</exception>
    public SequenceBranch<TSequenceContext, TSequenceData> Trigger(
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function,
        string functionName)
    {
        ArgumentNullException.ThrowIfNull(function);

        if (SequenceFunctions.Count == 0)
        {
            initiationFunctionName = functionName;
        }

        if (!SequenceFunctions.ContainsKey(functionName))
        {
            SequenceFunctions[functionName] = function;
        }

        return SequenceBranches[functionName] = new SequenceBranch<TSequenceContext, TSequenceData>(this);
    }

    /// <summary>Invokes the sequence.</summary>
    /// <param name="sequenceContext">The sequence context.</param>
    /// <param name="sequenceData">The sequence data.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">sequenceData
    /// or
    /// ProcessResponse.</exception>
    /// <exception cref="EntryPointNotFoundException"></exception>
    internal async ValueTask<IFunctionResult> InvokeSequence(
        TSequenceContext sequenceContext,
        TSequenceData sequenceData)
    {
        Logger.LogDebug($"Sequence '{Name}' started; {sequenceData.CorrelationKey}");

        if (Equals(sequenceData, default(TSequenceData)))
        {
            Logger.LogError($"Sequence '{Name}' aborted; data is null");
            throw new ArgumentNullException(nameof(sequenceData));
        }

        string? functionName = initiationFunctionName;
        var sequenceStopwatch = new Stopwatch();
        sequenceStopwatch.Start();

        if (functionName == null)
        {
            string message = $"Sequence '{Name}' aborted; no functions present; {sequenceData.CorrelationKey}";
            Logger.LogError(message);
            throw new EntryPointNotFoundException(message);
        }

        try
        {
            IFunctionResult? result = null;

            string lastFunctionName = functionName;

            while (functionName != null)
            {
                if (SequenceFunctions.TryGetValue(functionName, out var function))
                {
                    Logger.LogDebug($"Function '{functionName}' started; sequence '{Name}'; " +
                        $"{sequenceData.CorrelationKey}");

                    var functionStopwatch = new Stopwatch();
                    functionStopwatch.Start();

                    result = await function
                        .Invoke(sequenceContext, sequenceData)
                        .ConfigureAwait(false);

                    Logger.LogDebug($"Function '{functionName}' ended after {functionStopwatch.Elapsed.TotalMilliseconds} ms; " +
                        $"result '{result.Type}'; sequence '{Name}'; {sequenceData.CorrelationKey}");

                    if (SequenceBranches.TryGetValue(functionName, out var branch))
                    {
                        functionName = branch.GetContinuationFunctionName(result);

                        if (functionName != null)
                        {
                            lastFunctionName = functionName;
                        }

                        continue;
                    }
                }
                else
                {
                    Logger.LogError($"Function '{functionName}' not found; sequence '{Name}'; " +
                        $"{sequenceData.CorrelationKey}");
                }

                break;
            }

            if (sequenceStopwatch.Elapsed.TotalMilliseconds > 1000)
            {
                Logger.LogInformation($"Sequence '{Name}' ended after {sequenceStopwatch.Elapsed.TotalMilliseconds} ms; " +
                    $"result '{result.Type}'; last function '{lastFunctionName ?? "none"}'; " +
                    $"{sequenceData.CorrelationKey}");
            }
            else
            {
                Logger.LogDebug($"Sequence '{Name}' ended after {sequenceStopwatch.Elapsed.TotalMilliseconds} ms; " +
                    $"result '{result.Type}'; last function '{lastFunctionName ?? "none"}'; " +
                    $"{sequenceData.CorrelationKey}");
            }

            if (result.Type == FunctionResultType.Abort
                && result.Errors.Any())
            {
                Logger.LogError($"Sequence abort reason: {result.Errors.First().Message}; " +
                    $"last function '{lastFunctionName ?? "none"}'; {sequenceData.CorrelationKey}");
            }

            return result;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, $"Exception thrown in sequence '{Name}'; " +
                $"last function '{functionName}'; {sequenceData.CorrelationKey}");

            var result = FunctionResult.Abort();

            result.WithError(new Error("An unexpected error occurred"));

            return result;
        }
    }
}
