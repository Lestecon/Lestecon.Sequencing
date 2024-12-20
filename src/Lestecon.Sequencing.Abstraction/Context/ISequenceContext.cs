using Microsoft.Extensions.Logging;

namespace Lestecon.Sequencing.Abstraction.Context;

public interface ISequenceContext
{
    string ExecutingAssemblyName { get; }

    ILogger Logger { get; }
}
