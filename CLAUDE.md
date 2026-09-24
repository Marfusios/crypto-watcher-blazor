# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Blazor WebAssembly application showcasing real-time cryptocurrency market data using the `crypto-websocket-extensions` library. The app displays:
- Real-time price changes across multiple exchanges (< 1ms latency)
- L2 order book data (aggregated price levels)
- L3 order book data (individual order tracking with price/amount updates)

Supported exchanges: Bitfinex, Binance, Bitstamp, Coinbase, Hyperliquid (Bitmex disabled since the BitMEX exchange closed in September 2026)

Exchange feed notes:
- Coinbase subscribes to the public `level2_batch` channel, plain `level2` requires API keys. It streams the full book (~40k levels), so it's used only on the Prices page, `BidLevels`/`AskLevels` materialization on the L2 page can't keep up
- Hyperliquid uses the fast `l2Book` mode (every ~0.5s, 5 levels) on the Prices page, L2 page has a Fast / Deep switch (default Fast, Deep = 20 levels every ~5s), Liquidity page keeps the 20-level feed for depth

## Architecture

### Project Structure
- **Single project solution**: `CryptoWatcher.Blazor` - Blazor WebAssembly app
- **Features-based organization**: `Features/` directory groups Razor components by feature:
  - `Prices/` - Real-time price comparison across exchanges
  - `L2/` - L2 order book visualization (Binance, Bitfinex, Bitstamp, Hyperliquid)
  - `L3/` - L3 order book visualization (Bitfinex)
  - `Liquidity/` - Depth analysis near the mid price across exchanges
  - `Shared/` - Shared components (Layout, Card, PageHeader, Stat, OrderBook ladder, Icon) and `Format` number helpers
- **Styling**: plain CSS, no preprocessor
  - `wwwroot/css/app.css` - design tokens (dark and light theme via `data-theme`), base styles and shared primitives (`.page`, `.stats`, `.grid`, `.seg`, `.btn`, `.chip`, `.balance`)
  - `*.razor.css` next to components - Blazor CSS isolation, bundled by the SDK into `CryptoWatcher.Blazor.styles.css`; use `::deep` for elements rendered by child components (e.g. `NavLink`)
  - Theme and sidebar state live on `<html>` attributes, set by the inline script in `wwwroot/index.html` (`window.cw` helpers, persisted in localStorage)
  - Numbers use `Format` (invariant culture, tabular figures); up/bid = `--up`, down/ask = `--down`
- **Static assets**: `wwwroot/` directory

### Key Technical Patterns
- **WebSocket connections**: Each exchange has dedicated WebSocket client setup in `PricesPage.razor`
- **Order book management**: Uses `CryptoOrderBookL2`/`L3` from `crypto-websocket-extensions` library
- **Reactive streams**: RxJS-style observables for order book updates (`BidAskUpdatedStream`)
- **Price aggregation**: `CombineLatest()` merges streams from all exchanges, calculates average price
- **Component lifecycle**: Connection setup in `OnInitializedAsync()`, cleanup in `Dispose()`

## Development Commands

### Build & Run
```bash
dotnet restore                                                    # Restore NuGet packages
dotnet build -c Release                                          # Build in Release mode
dotnet watch run --project CryptoWatcher.Blazor/CryptoWatcher.Blazor.csproj  # Run with hot reload (http://localhost:5022)
```

### Testing
```bash
dotnet test --no-restore                                         # Run tests (CI command)
```

Note: Test projects don't currently exist but CI expects them. Future tests should use xUnit in a `Tests/` folder.

### Deployment
```bash
dotnet publish CryptoWatcher.Blazor/CryptoWatcher.Blazor.csproj -c Release -o build  # Publish for GitHub Pages
```

CI/CD: GitHub Actions publishes to GitHub Pages via `.github/workflows/gh-pages.yml`

## Code Style
- **Formatting**: Default C# style, 4 spaces, braces on new lines
- **Naming**: PascalCase for components/classes, `_camelCase` for private fields, `Async` suffix for async methods
- **Component structure**: Markup, `@code` block, and `.razor.css` in same directory
- **Commit messages**: Short, descriptive imperative style (e.g., "Fix average price", "Implement L2 order book")

## Adding Exchange Support

When adding a new exchange to `PricesPage.razor`:
1. Add entry to `_targetMarkets` dictionary with exchange name and trading pair
2. Create `Start{Exchange}()` method following existing patterns (Bitmex, Bitfinex, etc.)
3. Set up WebSocket communicator with exchange-specific URL
4. Configure client with subscription requests in `ReconnectionHappened` handler
5. Initialize `OrderBookSourceBase` (exchange-specific implementation)
6. Call `InitOrderBooks()` to wire up order book streams
7. Update `StartExchange()` switch statement with new exchange case
