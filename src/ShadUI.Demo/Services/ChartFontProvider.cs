using Avalonia.Platform;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.IO;

namespace ShadUI.Demo.Services;

public sealed class ChartFontProvider : IDisposable
{
    private static SKData? _fontData;
    private static SKTypeface? _typeface;
    private static bool _isConfigured;
    private static readonly object _lock = new();
    private static readonly SKTypeface? _fallbackTypeface = SKTypeface.Default;

    public SKTypeface? Typeface => _typeface;

    public ChartFontProvider()
    {
        lock (_lock)
        {
            if (_isConfigured)
                return;

            var fontUri = new Uri("avares://shadui-app/Assets/Fonts/Manrope-Regular.ttf");
            try
            {
                using var fontAsset = AssetLoader.Open(fontUri);
                using var memoryStream = new MemoryStream();
                fontAsset.CopyTo(memoryStream);

                _fontData = SKData.CreateCopy(memoryStream.ToArray());
                _typeface =
                    SKTypeface.FromData(_fontData)
                    ?? throw new InvalidOperationException("Failed to create SKTypeface from font data");
            }
            catch
            {
                _typeface = _fallbackTypeface;
            }

            LiveCharts.Configure(config =>
                config.HasTextSettings(new TextSettings { DefaultTypeface = _typeface })
            );
            _isConfigured = true;
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (!_isConfigured)
                return;

            _typeface?.Dispose();
            _fontData?.Dispose();
            _typeface = null;
            _fontData = null;
            _isConfigured = false;
        }
    }
}
