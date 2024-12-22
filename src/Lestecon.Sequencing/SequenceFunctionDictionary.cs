namespace Lestecon.Sequencing;

internal class SequenceFunctionDictionary<TSequenceContext, TSequenceData> :
    Dictionary<string, Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>>>
{
    public void AddIfMissing(
        string functionName,
        Func<TSequenceContext, TSequenceData, ValueTask<IFunctionResult>> function)
    {
        if (!ContainsKey(functionName)) this[functionName] = function;
    }

    public void SetInitiationFunctionName(string functionName, ref string? initiationFunctionName)
    {
        if (Count == 0) initiationFunctionName = functionName;
    }
}
