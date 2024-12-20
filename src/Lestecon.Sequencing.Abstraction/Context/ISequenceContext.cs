using Microsoft.Extensions.Logging;

namespace Lestecon.Sequencing;

public interface ISequenceContext
{
    string ExecutingAssemblyName { get; }

    ILogger Logger { get; }
}
