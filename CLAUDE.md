# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Blazor WebAssembly application showcasing real-time cryptocurrency market data using the `crypto-websocket-extensions` library. The app displays:
- Real-time price changes across multiple exchanges (< 1ms latency)
- L2 order book data (aggregated price levels)
- L3 order book data (individual order tracking with price/amount updates)

Supported exchanges: Bitmex, Bitfinex, Binance, Bitstamp, Hyperliquid (Coinbase currently disabled)

## Architecture

### Project Structure
- **Single project solution**: `CryptoWatcher.Blazor` - Blazor WebAssembly app
- **Features-based organization**: `Features/` directory groups Razor components by feature:
  - `Prices/` - Real-time price comparison across exchanges
  - `L2/` - L2 order book visualization (Binance)
  - `L3/` - L3 order book visualization (Bitfinex)
  - `Shared/` - Shared components (Card, Layout, Navigation)
  - `Stats/` - Statistics features
- **Styling**: SCSS files co-located with components; `Styles/crypto-watcher.scss` compiles to `wwwroot/crypto-watcher.css` via `compilerconfig.json`
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
- **Component structure**: Markup, `@code` block, and SCSS in same directory
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
