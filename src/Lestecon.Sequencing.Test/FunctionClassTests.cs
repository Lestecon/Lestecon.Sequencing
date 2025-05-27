using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Diagnostics;

namespace Lestecon.Sequencing.Test;

public class FunctionClassTests
{
    [Fact]
    public async Task TrueResultEmpty()
    {
        var context = new DefaultSequenceContext(Substitute.For<ILogger>());

        var sequence = SequenceBuilder.Create<DefaultSequenceContext, TestSequenceData>()
            .StartWith<TrueFunctionEmpty>()
                .IfTrueRun<FalseFunctionEmpty>()
            .After<FalseFunctionEmpty10ms>()
                .IfFalseRun<TrueFunctionEmpty10ms>()
            .After<FalseFunctionEmpty>()
                .IfFalseRun<FalseFunctionEmpty10ms>()
            .Build();

        var result = await sequence.Invoke(context, new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

        Assert.Equal(FunctionResultType.True, result.Type);
        Assert.Null(result.Value);
    }

    //[Fact]
    //public async Task TrueResultWithValue()
    //{
    //    var context = new SequenceContext(Substitute.For<ILogger>());
    //    var sequence = new SequenceOld<SequenceContext, TestSequenceData>(context, "TestSequence")
    //        .Run<TrueFunctionEmpty>()
    //            .IfTrueRun<FalseFunctionEmpty>()
    //        .After<FalseFunctionEmpty>()
    //            .IfFalseRun<FalseFunctionEmpty10ms>()
    //        .After<FalseFunctionEmpty10ms>()
    //            .IfFalseRun<TrueFunctionWithValue>();

    //    var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

    //    Assert.Equal(FunctionResultType.True, result.Type);
    //    Assert.NotNull(result.Value);
    //}

    //[Fact]
    //public async Task FalseResultEmpty()
    //{
    //    var context = new SequenceContext(Substitute.For<ILogger>());
    //    var sequence = new SequenceOld<SequenceContext, TestSequenceData>(context, "TestSequence")
    //        .Run<TrueFunctionEmpty>()
    //            .IfTrueRun<FalseFunctionEmpty>()
    //        .After<FalseFunctionEmpty>()
    //            .IfFalseRun<TrueFunctionEmpty10ms>()
    //        .After<TrueFunctionEmpty10ms>()
    //            .IfTrueRun<FalseFunctionEmpty10ms>();

    //    var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

    //    Assert.Equal(FunctionResultType.False, result.Type);
    //    Assert.Null(result.Value);
    //}

    //[Fact]
    //public async Task FalseResultWithValue()
    //{
    //    var context = new SequenceContext(Substitute.For<ILogger>());
    //    var sequence = new SequenceOld<SequenceContext, TestSequenceData>(context, "TestSequence")
    //        .Run<TrueFunctionEmpty>()
    //            .IfTrueRun<FalseFunctionEmpty>()
    //        .After<FalseFunctionEmpty>()
    //            .IfFalseRun<FalseFunctionEmpty10ms>()
    //        .After<FalseFunctionEmpty10ms>()
    //            .IfTrueRun<FalseFunctionWithValue>();

    //    var result = await sequence.Invoke(new TestSequenceData(Guid.NewGuid().ToString(), Stopwatch.StartNew()));

    //    Assert.Equal(FunctionResultType.False, result.Type);
    //    Assert.NotNull(result.Value);
    //}
}
