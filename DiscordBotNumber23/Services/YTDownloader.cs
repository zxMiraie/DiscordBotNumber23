using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DiscordBotNumber23.Services;

public class YtDownloader(ILogger<YtDownloader> logger)
{
    private readonly YoutubeClient _client = new();

    public async Task<string?> DownloadVideo(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
           logger.LogError("You must provide a video url");
           throw new ArgumentException("You must provide a video url");
        }
            
        Directory.CreateDirectory("temp");

        var video = await _client.Videos.GetAsync(url);
        var manifest = await _client.Videos.Streams.GetManifestAsync(video.Id);
        var streamInfo = manifest
            .GetMuxedStreams()
            .GetWithHighestVideoQuality();

        if (streamInfo is null)
        {
            logger.LogError("Could not find video stream");
            throw new InvalidOperationException("No downloadable streams found for this video.");
        }
            
        var filePath = Path.Combine("temp", $"{video.Id}.mp4"); 
        await _client.Videos.Streams.DownloadAsync(streamInfo, filePath);
        logger.LogInformation($"Downloaded {video.Id} to {filePath}");
        return filePath;
    }
}
