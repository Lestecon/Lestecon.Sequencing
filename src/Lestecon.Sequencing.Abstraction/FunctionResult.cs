using FluentResults;

namespace Lestecon.Sequencing;

public class FunctionResult : Result<object>, IFunctionResult
{
    public FunctionResult(FunctionResultType type)
    {
        Type = type;
    }

    public FunctionResultType Type { get; }

    public new FunctionResult WithValue(object value)
    {
        base.WithValue(value);
        return this;
    }

    public static FunctionResult Abort() => new(FunctionResultType.Abort);

    public static FunctionResult False() => new(FunctionResultType.False);

    public static implicit operator ValueTask<IFunctionResult>(FunctionResult functionResult) => new(functionResult);

    public static FunctionResult True() => new(FunctionResultType.True);

    public static FunctionResult Ambivalent() => new(FunctionResultType.Ambivalent);
}
