using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Commands");
            builder.AddField("help", "Returns this message", false);
            builder.AddField("status 'new status'", "Sets the bots status", false);
            builder.AddField("prefix 'new prefix'", "Sets the command prefix", false);
            builder.AddField("download video 'YouTube video URL'", "Sends a download link to the video", false);
            builder.AddField("download sound 'YouTube video URL'", "Sends a download link to the sound", false);
            builder.AddField("For more documentation and source code visit", "https://github.com/Affe130/YoutubeDownloaderDiscordBot", false);

            builder.WithColor(Color.Green);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("prefix")]
        public async Task Prefix(string prefix)
        {
            Program.settings.CommandPrefix = prefix;
            Program.settings.SaveToJson(Program.configFilePath);
            await ReplyAsync($"Prefix was set to '{prefix}'");
        }

        [Command("status")]
        public async Task Status(string status)
        {
            Program.settings.BotStatus = status;
            await Context.Client.SetGameAsync(status);
            Program.settings.SaveToJson(Program.configFilePath);
            await ReplyAsync($"Status was set to '{status}'");
        }

        [Command("download video")]
        public async Task DownloadVideo(string url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            var builder = new EmbedBuilder();

            builder.WithTitle("Downloading video...");
            builder.AddField("Link", video.Url, false);
            builder.AddField("Channel", video.Author, false);
            builder.AddField("Title", video.Title, false);
            builder.AddField("Description", video.Description, false);

            builder.WithColor(Color.Red);

            await ReplyAsync("", false, builder.Build());

            await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Program.downloadsPath, $"{video.Title}.{streamInfo.Container}"));
            await ReplyAsync("Download Finished");
        }

        [Command("download sound")]
        public async Task DownloadSound(string url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            var builder = new EmbedBuilder();

            builder.WithTitle("Downloading video...");
            builder.AddField("Link", video.Url, false);
            builder.AddField("Channel", video.Author, false);
            builder.AddField("Title", video.Title, false);
            builder.AddField("Description", video.Description, false);

            builder.WithColor(Color.Red);

            await ReplyAsync("", false, builder.Build());

            await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Program.downloadsPath, $"{video.Title}.{streamInfo.Container}"));
            await ReplyAsync("Download Finished");
        }
    }
}
