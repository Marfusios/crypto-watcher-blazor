# Repository Guidelines

## Project Structure & Module Organization
- `CryptoWatcher.Blazor/` hosts the Blazor WebAssembly app; `Program.cs` wires root components and DI.
- `Features/` groups Razor pages by feature (`L2`, `L3`, `Prices`, `Shared`); each `.razor` may have a co-located `.razor.scss`.
- `Styles/crypto-watcher.scss` compiles (via `compilerconfig.json`) into `wwwroot/crypto-watcher.css`; keep static assets under `wwwroot/`.
- `.github/workflows/` defines CI builds/tests and the GitHub Pages deploy pipeline.

## Build, Test, and Development Commands
- `dotnet restore CryptoWatcher.sln` - restore NuGet packages.
- `dotnet build CryptoWatcher.sln -c Release` - compile and validate the app.
- `dotnet watch run --project CryptoWatcher.Blazor/CryptoWatcher.Blazor.csproj` - run the WASM app locally with hot reload at `http://localhost:5020`.
- `dotnet publish CryptoWatcher.Blazor/CryptoWatcher.Blazor.csproj -c Release -o build` - mirror the GitHub Pages publish output (adds assets to `build/wwwroot`).

## Coding Style & Naming Conventions
- Use the default C# formatting (4 spaces, braces on new lines); keep top-level statements where already used.
- Name components and classes in PascalCase, private fields `_camelCase`, and keep async methods suffixed with `Async`.
- Razor components should keep markup, `@code` block, and CSS in the same directory; shared helpers belong in `Features/Shared/`.

## Testing Guidelines
- CI runs `dotnet test --no-restore`; ensure new work keeps this green even if the solution currently lacks test projects.
- Introduce automated tests alongside new features (xUnit or similar) under a future `Tests/` folder and include them in `CryptoWatcher.sln`.
- When adding tests, name files `*Tests.cs` and align namespaces with source folders.
- Use MCP playwright server to verify UI changes where applicable

## Commit & Pull Request Guidelines
- Follow the existing short, descriptive subject style (`Fix average price`, `Implement L2 order book`).
- Keep commits focused; document breaking changes or migrations in the body.
- PRs should explain the change, link related issues, attach UI screenshots or GIFs, and note local `dotnet build`/`dotnet test` results.

## Deployment & Runtime Notes
- GitHub Actions publishes the static site to `gh-pages`; use the publish command above before touching deployment assets.
- Local debugging uses the profile in `CryptoWatcher.Blazor/Properties/launchSettings.json`; adjust environment variables there when needed.
