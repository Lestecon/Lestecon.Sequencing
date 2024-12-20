using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Lestecon.Sequencing.Abstraction.Context;
public class SequenceContext : ISequenceContext
{
    public SequenceContext(ILogger logger)
    {
        ExecutingAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name
            ?? Assembly.GetExecutingAssembly().GetName().Name
            ?? Assembly.GetCallingAssembly().GetName().Name
            ?? "Unknown";
        Logger = logger;
    }

    public string ExecutingAssemblyName { get; }

    public ILogger Logger { get; }
}
