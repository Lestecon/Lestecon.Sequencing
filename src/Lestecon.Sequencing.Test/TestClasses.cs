using Lestecon.Sequencing.Abstraction;
using System.Diagnostics;

namespace Lestecon.Sequencing.Test;

#region True

internal class TrueFunctionEmpty10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.True();
    }
}

internal class TrueFunctionEmpty : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.True();
}

internal class TrueFunctionWithValue10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.True(new TestObjectDto());
    }
}

internal class TrueFunctionWithValue : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.True(new TestObjectDto());
}

#endregion

#region False

internal class FalseFunctionEmpty10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.False();
    }
}

internal class FalseFunctionEmpty : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) => FunctionResult.False();
}

internal class FalseFunctionWithValue10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.False(new TestObjectDto());
    }
}

internal class FalseFunctionWithValue : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.False(new TestObjectDto());
}

#endregion

#region Abort

internal class AbortFunctionEmpty10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Abort();
    }
}

internal class AbortFunctionEmpty : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) => FunctionResult.Abort();
}

internal class AbortFunctionWithValue10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Abort(new TestObjectDto());
    }
}

internal class AbortFunctionWithValue : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.Abort(new TestObjectDto());
}

#endregion

#region Indeterminate

internal class IndeterminateFunctionEmpty10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Indeterminate();
    }
}

internal class IndeterminateFunctionEmpty : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) => FunctionResult.Indeterminate();
}

internal class IndeterminateFunctionWithValue10ms : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public async ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData)
    {
        await Task.Delay(10);

        return FunctionResult.Indeterminate(new TestObjectDto());
    }
}

internal class IndeterminateFunctionWithValue : ISequenceFunction<DefaultSequenceContext, TestSequenceData>
{
    public ValueTask<FunctionResult> Invoke(DefaultSequenceContext sequenceContext, TestSequenceData sequenceData) =>
        FunctionResult.Indeterminate(new TestObjectDto());
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
