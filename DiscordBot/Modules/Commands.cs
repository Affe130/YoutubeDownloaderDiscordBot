using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public enum DownloadType
        {
            Video,
            Sound
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

            await SendDownloadInfo(video, DownloadType.Video);

            string fileName = $"video.mp4";
            string filePath = Path.Combine(Program.downloadsPath, fileName);

            await youtube.Videos.DownloadAsync(url, filePath);

            try
            {
                await Context.Channel.SendFileAsync(filePath);
            }
            catch
            {
                await ReplyAsync($"Sorry the file is too big, maximum is {FormatBytes.FormatBytesWithSuffix(Context.Guild.MaxUploadLimit)}!");
            }
        }

        [Command("download sound")]
        public async Task YoutubeDownloadSound(string url)
        {
            YoutubeClient youtube = new();

            var video = await youtube.Videos.GetAsync(url);

            await SendDownloadInfo(video, DownloadType.Sound);

            string fileName = $"sound.mp3";
            string filePath = Path.Combine(Program.downloadsPath, fileName);

            await youtube.Videos.DownloadAsync(url, filePath);

            try
            {
                await Context.Channel.SendFileAsync(filePath);
            }
            catch
            {
                await ReplyAsync($"Sorry the file is too big, maximum is {FormatBytes.FormatBytesWithSuffix(Context.Guild.MaxUploadLimit)}!");
            }
        }

        private async Task SendDownloadInfo(YoutubeExplode.Videos.Video video, DownloadType type)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle($"Downloading {type.ToString().ToLower()}...");
            builder.WithImageUrl(video.Thumbnails[0].Url);
            builder.AddField($"Title", video.Title, false);
            builder.AddField($"Channel", video.Author, false);
            builder.AddField($"Link", video.Url, false);
            builder.AddField($"Duration", video.Duration, false);
            if (video.Description.Length >= 1 && video.Description.Length <= 1024)
            {
                builder.AddField($"Description", video.Description, false);
            }
            builder.WithColor(Color.Red);

            await ReplyAsync("", false, builder.Build());
        }
    }
}
