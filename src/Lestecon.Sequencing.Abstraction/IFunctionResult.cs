using FluentResults;

namespace Lestecon.Sequencing;

public interface IFunctionResult : IResult<object>
{
    FunctionResultType Type { get; }
}
