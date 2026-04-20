using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfUI.Helpers;

public sealed class ImagePathToBitmapSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => TryLoad(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;

    /// <summary>
    /// Loads the image at <paramref name="path"/> as a frozen <see cref="BitmapSource"/>.
    /// Returns <c>null</c> for missing files, blank paths, or any decode failure (e.g. unsupported
    /// format, missing codec such as HEIF Image Extensions, corrupt file, file currently in use).
    /// Defensive by design so a broken file never crashes a binding.
    /// </summary>
    public static BitmapSource? TryLoad(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);

            BitmapDecoder decoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            BitmapFrame frame = decoder.Frames[0];
            BitmapSource oriented = ApplyExifOrientationFromFrame(frame);

            oriented.Freeze();
            return oriented;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (FileFormatException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    private static BitmapSource ApplyExifOrientationFromFrame(BitmapFrame frame)
    {
        const string orientationQuery = "/app1/ifd/{ushort=274}";

        if (frame.Metadata is not BitmapMetadata metadata)
            return frame;

        if (!metadata.ContainsQuery(orientationQuery))
            return frame;

        object raw = metadata.GetQuery(orientationQuery);

        if (raw is not ushort orientation)
            return frame;

        return orientation switch
        {
            3 => new TransformedBitmap(frame, new RotateTransform(180)),
            6 => new TransformedBitmap(frame, new RotateTransform(90)),
            8 => new TransformedBitmap(frame, new RotateTransform(270)),
            2 => new TransformedBitmap(frame, new ScaleTransform(-1, 1, 0.5, 0.5)),
            4 => new TransformedBitmap(frame, new ScaleTransform(1, -1, 0.5, 0.5)),
            5 => new TransformedBitmap(frame, new TransformGroup { Children = { new ScaleTransform(-1, 1), new RotateTransform(90) } }),
            7 => new TransformedBitmap(frame, new TransformGroup { Children = { new ScaleTransform(-1, 1), new RotateTransform(270) } }),
            _ => frame
        };
    }
}
