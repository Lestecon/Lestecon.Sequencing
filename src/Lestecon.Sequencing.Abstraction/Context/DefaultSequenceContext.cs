using Microsoft.Extensions.Logging;

namespace Lestecon.Sequencing;

public class DefaultSequenceContext : ISequenceContext
{
    public DefaultSequenceContext(ILogger logger)
    {
        Logger = logger;
    }

    public ILogger Logger { get; }
}
