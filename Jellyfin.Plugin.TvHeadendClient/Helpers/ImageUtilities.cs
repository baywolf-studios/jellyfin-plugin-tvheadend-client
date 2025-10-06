using MediaBrowser.Controller;
using MediaBrowser.Model.Drawing;

namespace Jellyfin.Plugin.TvHeadendClient.Helpers;

public static class ImageUtilities
{
    private static readonly Dictionary<ImageFormat, byte?[][]> SignatureTable = new()
    {
        [ImageFormat.Bmp] =
        [
            [0x42, 0x4D] // BM
        ],
        [ImageFormat.Gif] =
        [
            [0x47, 0x49, 0x46, 0x38, 0x37, 0x61], // GIF87a
            [0x47, 0x49, 0x46, 0x38, 0x39, 0x61] // GIF89a
        ],
        [ImageFormat.Jpg] =
        [
            [0xFF, 0xD8, 0xFF, 0xDB],
            [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01], // JFIF
            [0xFF, 0xD8, 0xFF, 0xEE] // Adobe JPEG
        ],
        [ImageFormat.Png] =
        [
            [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A] // PNG
        ],
        [ImageFormat.Webp] =
        [
            // RIFF....WEBP
            [0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x57, 0x45, 0x42, 0x50]
        ],
        [ImageFormat.Svg] =
        [
            // <?xml
            [0x3C, 0x3F, 0x78, 0x6D, 0x6C],
            // <svg
            // SVG can also start with '<svg' if the XML declaration is omitted.
            // Note: This pattern might be less robust for detection in a generic header check.
            [0x3C, 0x73, 0x76, 0x67],
            // Optional: With UTF-8 BOM, followed by <?xml
            // BOM: 0xEF, 0xBB, 0xBF, followed by <?xml (0x3C, 0x3F, 0x78, 0x6D, 0x6C)
            [0xEF, 0xBB, 0xBF, 0x3C, 0x3F, 0x78, 0x6D, 0x6C]
        ]
    };

    public static bool TryGetImageType(byte[]? imageData, out ImageFormat format)
    {
        ArgumentNullException.ThrowIfNull(imageData);

        foreach (var (candidateFormat, signatures) in SignatureTable)
        foreach (var signature in signatures)
        {
            if (imageData.Length < signature.Length)
            {
                continue;
            }

            var isMatch = true;

            for (var i = 0; i < signature.Length; i++)
            {
                var expectedByte = signature[i];
                if (!expectedByte.HasValue || imageData[i] == expectedByte.Value)
                {
                    continue;
                }

                isMatch = false;
                break;
            }

            if (isMatch)
            {
                format = candidateFormat;
                return true;
            }
        }

        format = ImageFormat.Jpg;
        return false;
    }

    public static (bool HasImage, string? ImageUrl) GetImageInfo(string? imageUrl, IServerApplicationHost appHost)
    {
        var hasImage = !string.IsNullOrEmpty(imageUrl);

        var resolvedImageUrl = !string.IsNullOrEmpty(imageUrl) &&
                               imageUrl.StartsWith("imagecache", StringComparison.OrdinalIgnoreCase)
            ? $"{appHost.GetApiUrlForLocalAccess()}/{PluginInfo.IdString}/{imageUrl}"
            : imageUrl;

        return (hasImage, resolvedImageUrl);
    }
}
