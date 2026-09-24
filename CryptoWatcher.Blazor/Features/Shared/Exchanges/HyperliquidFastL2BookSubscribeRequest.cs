using Hyperliquid.Client.Websocket.Requests;

namespace CryptoWatcher.Blazor.Features.Shared.Exchanges;

/// <summary>
/// Hyperliquid L2 book subscription in the fast mode (not exposed by the client library yet).
/// Snapshots are pushed every ~0.5s with 5 levels per side, instead of every ~5s with 20 levels.
/// </summary>
public class HyperliquidFastL2BookSubscribeRequest : HyperliquidRequestBase
{
    public HyperliquidFastL2BookSubscribeRequest(string coin)
    {
        Subscription = new { type = "l2Book", coin, fast = true };
    }

    public override object? Subscription { get; }
}
