using Lestecon.Sequencing.Abstraction;
using System.Diagnostics;

namespace Lestecon.Sequencing.Test;

#region True

internal class TrueFunctionEmpty10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.True();
    }
}

internal class TrueFunctionEmpty : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.True();
}

internal class TrueFunctionWithValue10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.True().WithValue(new TestObjectDto());
    }
}

internal class TrueFunctionWithValue : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.True().WithValue(new TestObjectDto());
}

#endregion

#region False

internal class FalseFunctionEmpty10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.False();
    }
}

internal class FalseFunctionEmpty : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) => FunctionResult.False();
}

internal class FalseFunctionWithValue10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.False().WithValue(new TestObjectDto());
    }
}

internal class FalseFunctionWithValue : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.False().WithValue(new TestObjectDto());
}

#endregion

#region Abort

internal class AbortFunctionEmpty10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Abort();
    }
}

internal class AbortFunctionEmpty : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) => FunctionResult.Abort();
}

internal class AbortFunctionWithValue10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Abort().WithValue(new TestObjectDto());
    }
}

internal class AbortFunctionWithValue : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.Abort().WithValue(new TestObjectDto());
}

#endregion

#region Ambivalent

internal class AmbivalentFunctionEmpty10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Ambivalent();
    }
}

internal class AmbivalentFunctionEmpty : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) => FunctionResult.Ambivalent();
}

internal class AmbivalentFunctionWithValue10ms : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public async ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Ambivalent().WithValue(new TestObjectDto());
    }
}

internal class AmbivalentFunctionWithValue : ISequenceFunction<SequenceContext, TestSequenceData>
{
    public ValueTask<IFunctionResult> Invoke(SequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.Ambivalent().WithValue(new TestObjectDto());
}

#endregion

internal class TestSequenceData : ISequenceData
{
    public TestSequenceData(
        string correlationKey,
        Stopwatch stopwatch)
    {
        CorrelationKey = correlationKey;
        Stopwatch = stopwatch;
    }

    public string CorrelationKey { get; set; }
    public Stopwatch Stopwatch { get; set; }
}

internal class TestObjectDto
{
    public TestObjectDto()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public string? Message { get; set; }
}
