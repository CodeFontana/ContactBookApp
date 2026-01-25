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
    {
        string? path = value as string;

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return null;

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

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;

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