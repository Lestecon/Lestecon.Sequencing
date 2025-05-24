
using Lestecon.Result;

namespace Lestecon.Sequencing;

public class FunctionResult : Result<object>
{
    public FunctionResult(FunctionResultType type, bool isSuccess)
        : base(isSuccess)
    {
        Type = type;
    }

    public FunctionResult(FunctionResultType type, object value)
        : base(value)
    {
        Type = type;
    }

    public FunctionResult(FunctionResultType type, Error error)
        : base(error)
    {
        Type = type;
    }

    public FunctionResultType Type { get; init; }

    public static FunctionResult Abort() => new(FunctionResultType.Abort, isSuccess: false);
    public static FunctionResult Abort(object value) => new(FunctionResultType.Abort, value);
    public static FunctionResult Abort(Error error) => new(FunctionResultType.Abort, error);
    public static FunctionResult False() => new(FunctionResultType.False, isSuccess: true);
    public static FunctionResult False(object value) => new(FunctionResultType.False, value);
    public static FunctionResult True() => new(FunctionResultType.True, isSuccess: true);
    public static FunctionResult True(object value) => new(FunctionResultType.True, value);
    public static FunctionResult Indeterminate() => new(FunctionResultType.Indeterminate, isSuccess: true);
    public static FunctionResult Indeterminate(object value) => new(FunctionResultType.Indeterminate, value);

    public static implicit operator ValueTask<FunctionResult>(FunctionResult functionResult) => new(functionResult);

}
