using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Net;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public enum DownloadType
        {
            video,
            sound
        }

        [Command("help")]
        public async Task Help()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle($"Commands");
            builder.AddField($"help", "Returns this message", false);
            builder.AddField($"status 'new status'", "Sets the bots status", false);
            builder.AddField($"prefix 'new prefix'", "Sets the command prefix", false);
            builder.AddField($"download video 'YouTube video URL'", "Sends a download link to the video", false);
            builder.AddField($"download sound 'YouTube video URL'", "Sends a download link to the sound", false);
            builder.AddField($"For more documentation and source code visit", "https://github.com/Affe130/YoutubeDownloaderDiscordBot", false);
            builder.WithColor(Color.Green);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("prefix")]
        public async Task Prefix(string prefix)
        {
            Program.settings.CommandPrefix = prefix;
            Program.settings.SaveToJson(Program.configFilePath);
            await ReplyAsync($"Command prefix was set to {prefix}");
        }

        [Command("status")]
        public async Task Status(string status)
        {
            Program.settings.BotStatus = status;
            Program.settings.SaveToJson(Program.configFilePath);
            await Context.Client.SetGameAsync(status);
            await ReplyAsync($"Bot status was set to {status}");
        }

        [Command("download video")]
        public async Task DownloadYoutubeVideo(string url)
        {
            YoutubeClient youtube = new();
            YoutubeExplode.Videos.Video video = await youtube.Videos.GetAsync(url);
            StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            IVideoStreamInfo streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            await SendDownloadInfo(video, DownloadType.video);

            string fileName = $"{video.Title}.{streamInfo.Container}";
            string filePath = Path.Combine(Program.downloadsPath, fileName);

            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
            await ReplyAsync($"Download finished, download link: {filePath}");
        }

        [Command("download sound")]
        public async Task YoutubeDownloadSound(string url)
        {
            YoutubeClient youtube = new YoutubeClient();
            YoutubeExplode.Videos.Video video = await youtube.Videos.GetAsync(url);
            StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            IStreamInfo streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            await SendDownloadInfo(video, DownloadType.sound);

            string fileName = $"{video.Title}.{streamInfo.Container}";
            string filePath = Path.Combine(Program.downloadsPath, fileName);

            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
            await ReplyAsync($"Download finished, download link: {filePath}");
        }

        private async Task SendDownloadInfo(YoutubeExplode.Videos.Video video, DownloadType type)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle($"Downloading {type}...");
            builder.AddField($"Title", video.Title, false);
            builder.AddField($"Channel", video.Author, false);
            builder.AddField($"Link", video.Url, false);
            builder.AddField($"Duration", video.Duration, false);
            builder.AddField($"Description", video.Description, false);
            builder.WithColor(Color.Red);

            await ReplyAsync("", false, builder.Build());
        }
    }
}
