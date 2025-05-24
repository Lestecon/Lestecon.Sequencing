using Microsoft.Extensions.Logging;

namespace Lestecon.Sequencing;

public interface ISequenceContext
{
    ILogger Logger { get; }
}
