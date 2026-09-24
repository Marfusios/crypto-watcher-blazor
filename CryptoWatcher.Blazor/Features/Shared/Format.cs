using System.Globalization;

namespace CryptoWatcher.Blazor.Features.Shared;

/// <summary>
/// Number formatting shared by all pages, invariant culture keeps trading notation (84,218.01) independent of the browser locale
/// </summary>
public static class Format
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public static string Price(double? value) => Number(value, 2);

    public static string Size(double? value) => Number(value, 4);

    public static string Number(double? value, int decimals) =>
        value.HasValue && double.IsFinite(value.Value) ? value.Value.ToString("N" + decimals, Culture) : "–";

    /// <summary>Number with explicit sign, zero renders without sign</summary>
    public static string Signed(double value, int decimals = 2)
    {
        var abs = Math.Abs(value).ToString("N" + decimals, Culture);
        if (abs == 0.0.ToString("N" + decimals, Culture))
            return abs;
        return (value > 0 ? "+" : "−") + abs;
    }

    /// <summary>Fraction of the reference price in basis points</summary>
    public static double Bps(double value, double reference) => reference > 0 ? value / reference * 10_000 : 0;

    public static string Percent(double value, int decimals = 1) => value.ToString("N" + decimals, Culture) + "%";

    /// <summary>Compact USD amount, e.g. $6.41M</summary>
    public static string Usd(double value) => Math.Abs(value) switch
    {
        >= 1_000_000_000 => "$" + (value / 1_000_000_000).ToString("N2", Culture) + "B",
        >= 1_000_000 => "$" + (value / 1_000_000).ToString("N2", Culture) + "M",
        >= 1_000 => "$" + (value / 1_000).ToString("N1", Culture) + "K",
        _ => "$" + value.ToString("N0", Culture)
    };

    /// <summary>Display name of an exchange, e.g. binance -> Binance</summary>
    public static string Name(string value) =>
        string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];

    /// <summary>CSS class for a signed value</summary>
    public static string? Tone(double value) => value > 0 ? "up" : value < 0 ? "down" : null;
}
