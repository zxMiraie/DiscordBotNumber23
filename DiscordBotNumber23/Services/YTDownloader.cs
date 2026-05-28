using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DiscordBotNumber23.Services;

public class YtDownloader
{
    private readonly YoutubeClient _client = new();

    public async Task<string?> DownloadVideo(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Video url must be provided.", nameof(url));

        Directory.CreateDirectory("temp");

        var video = await _client.Videos.GetAsync(url);
        var manifest = await _client.Videos.Streams.GetManifestAsync(video.Id);
        var streamInfo = manifest
            .GetMuxedStreams()
            .GetWithHighestVideoQuality();

        if (streamInfo is null)
            throw new InvalidOperationException("No downloadable streams found for this video.");

        var filePath = Path.Combine("temp", $"{video.Id}.mp4"); 

        await _client.Videos.Streams.DownloadAsync(streamInfo, filePath);
        
        return filePath;
    }
}
