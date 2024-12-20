using FluentResults;

namespace Lestecon.Sequencing.Abstraction;
public interface IFunctionResult : IResult<object>
{
    FunctionResultType Type { get; }
}
