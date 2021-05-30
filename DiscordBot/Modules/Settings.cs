using Newtonsoft.Json;
using System.IO;

namespace DiscordBot.Modules
{
    class Settings
    {
        public string CommandPrefix { get; set; }
        public string BotToken { get; set; }
        public string BotStatus { get; set; }

        public string FtpHost { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }

        public string WebsiteURL { get; set; }

        public static Settings LoadFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        public void SaveToJson(string filePath)
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(filePath, json);
        }

        public void SetToStandard(string filePath)
        {
            CommandPrefix = "!";
            BotToken = "BOT TOKEN";
            BotStatus = "!help";

            FtpHost = "FTP HOST";
            FtpUsername = "FTP USERNAME";
            FtpPassword = "FTP PASSWORD";

            WebsiteURL = "WEBSITE URL";
            this.SaveToJson(filePath);
        }
    }
}