using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DiscordBotNumber23.Services;

// Builds the "ship" image: two avatars placed side by side.
public sealed class ShipService
{
    private const int Size = 256; // each avatar, px

    private static readonly HttpClient Http = new();

    public async Task<MemoryStream> CreateAsync(string avatarUrl1, string avatarUrl2, CancellationToken cancellationToken = default)
    {
        var bytes1 = await Http.GetByteArrayAsync(avatarUrl1, cancellationToken);
        var bytes2 = await Http.GetByteArrayAsync(avatarUrl2, cancellationToken);

        using var left = Image.Load<Rgba32>(bytes1);
        using var right = Image.Load<Rgba32>(bytes2);
        left.Mutate(c => c.Resize(Size, Size));
        right.Mutate(c => c.Resize(Size, Size));

        using var canvas = new Image<Rgba32>(Size * 2, Size);
        canvas.Mutate(c => c
            .DrawImage(left, new Point(0, 0), 1f)
            .DrawImage(right, new Point(Size, 0), 1f));

        var stream = new MemoryStream();
        await canvas.SaveAsPngAsync(stream, cancellationToken);
        stream.Position = 0;
        return stream;
    }
}
