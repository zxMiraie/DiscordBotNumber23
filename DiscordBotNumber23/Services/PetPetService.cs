using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DiscordBotNumber23.Services;

// Generates the classic "petpet" animation: a hand squishes the target's avatar.
// Algorithm ported from the pet-pet-gif npm package (10 frames, bounce easing).
public sealed class PetPetService : IDisposable
{
    private const int Frames = 10;
    private const int Resolution = 128;
    private const int FrameDelay = 2; // centiseconds (~20ms per frame)

    private static readonly HttpClient Http = new();

    // Hand sprites, preloaded once and scaled to the output resolution.
    private readonly Image<Rgba32>[] _hands = new Image<Rgba32>[Frames];

    public PetPetService()
    {
        var handDir = Path.Combine(AppContext.BaseDirectory, "Assets", "petpet");
        for (var i = 0; i < Frames; i++)
        {
            var hand = Image.Load<Rgba32>(Path.Combine(handDir, $"pet{i}.gif"));
            hand.Mutate(c => c.Resize(Resolution, Resolution));
            _hands[i] = hand;
        }
    }

    public async Task<MemoryStream> CreateAsync(string avatarUrl, CancellationToken cancellationToken = default)
    {
        var avatarBytes = await Http.GetByteArrayAsync(avatarUrl, cancellationToken);
        using var avatar = Image.Load<Rgba32>(avatarBytes);

        using var gif = new Image<Rgba32>(Resolution, Resolution);
        gif.Metadata.GetGifMetadata().RepeatCount = 0; // loop forever

        for (var i = 0; i < Frames; i++)
        {
            // Bounce: 0,1,2,3,4,5,4,3,2,1 -> hand presses down then releases.
            var j = i < Frames / 2 ? i : Frames - i;

            var width = 0.8 + j * 0.02;
            var height = 0.8 - j * 0.05;
            var offsetX = (1 - width) * 0.5 + 0.1;
            var offsetY = (1 - height) - 0.08;

            var w = (int)Math.Round(Resolution * width);
            var h = (int)Math.Round(Resolution * height);
            var x = (int)Math.Round(Resolution * offsetX);
            var y = (int)Math.Round(Resolution * offsetY);

            using var avatarFrame = avatar.Clone(c => c.Resize(w, h));
            using var canvas = new Image<Rgba32>(Resolution, Resolution);
            canvas.Mutate(c => c
                .DrawImage(avatarFrame, new Point(x, y), 1f)
                .DrawImage(_hands[i], new Point(0, 0), 1f));

            var meta = canvas.Frames.RootFrame.Metadata.GetGifMetadata();
            meta.FrameDelay = FrameDelay;
            meta.DisposalMethod = GifDisposalMethod.RestoreToBackground;

            gif.Frames.AddFrame(canvas.Frames.RootFrame);
        }

        gif.Frames.RemoveFrame(0); // drop the initial blank frame

        var stream = new MemoryStream();
        await gif.SaveAsGifAsync(stream, new GifEncoder { ColorTableMode = GifColorTableMode.Local }, cancellationToken);
        stream.Position = 0;
        return stream;
    }

    public void Dispose()
    {
        foreach (var hand in _hands)
            hand?.Dispose();
    }
}
