using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Diagnostics;

namespace Lestecon.Sequencing.Test;

public class FunctionClassTests
{
    [Fact]
    public async Task TrueResultEmpty()
    {
        var context = new SequenceContext(Substitute.For<ILogger>());
        var sequence = new Sequence<SequenceContext, TestSequenceData>(context, "TestSequence")
            .Trigger<TrueFunctionEmpty>()
                .OnTrue<FalseFunctionEmpty>()
            .When<FalseFunctionEmpty>()
                .OnFalse<FalseFunctionEmpty10ms>()
            .When<FalseFunctionEmpty10ms>()
                .OnFalse<TrueFunctionEmpty10ms>();

        var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

        Assert.Equal(FunctionResultType.True, result.Type);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task TrueResultWithValue()
    {
        var context = new SequenceContext(Substitute.For<ILogger>());
        var sequence = new Sequence<SequenceContext, TestSequenceData>(context, "TestSequence")
            .Trigger<TrueFunctionEmpty>()
                .OnTrue<FalseFunctionEmpty>()
            .When<FalseFunctionEmpty>()
                .OnFalse<FalseFunctionEmpty10ms>()
            .When<FalseFunctionEmpty10ms>()
                .OnFalse<TrueFunctionWithValue>();

        var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

        Assert.Equal(FunctionResultType.True, result.Type);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task FalseResultEmpty()
    {
        var context = new SequenceContext(Substitute.For<ILogger>());
        var sequence = new Sequence<SequenceContext, TestSequenceData>(context, "TestSequence")
            .Trigger<TrueFunctionEmpty>()
                .OnTrue<FalseFunctionEmpty>()
            .When<FalseFunctionEmpty>()
                .OnFalse<TrueFunctionEmpty10ms>()
            .When<TrueFunctionEmpty10ms>()
                .OnTrue<FalseFunctionEmpty10ms>();

        var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

        Assert.Equal(FunctionResultType.False, result.Type);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task FalseResultWithValue()
    {
        var context = new SequenceContext(Substitute.For<ILogger>());
        var sequence = new Sequence<SequenceContext, TestSequenceData>(context, "TestSequence")
            .Trigger<TrueFunctionEmpty>()
                .OnTrue<FalseFunctionEmpty>()
            .When<FalseFunctionEmpty>()
                .OnFalse<FalseFunctionEmpty10ms>()
            .When<FalseFunctionEmpty10ms>()
                .OnFalse<FalseFunctionWithValue>();

        var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

        Assert.Equal(FunctionResultType.False, result.Type);
        Assert.NotNull(result.Value);
    }
}
