namespace CryptoWatcher.Blazor.Features.Shared;

/// <summary>
/// Order book streams can emit hundreds of updates per second,
/// pages render the latest state at a fixed rate instead of on every update
/// </summary>
public static class Throttling
{
    /// <summary>At most 10 renders per second, faster changes can't be read anyway</summary>
    public static readonly TimeSpan RenderInterval = TimeSpan.FromMilliseconds(100);
}
