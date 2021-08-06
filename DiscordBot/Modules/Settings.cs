using Newtonsoft.Json;
using System.IO;

namespace DiscordBot.Modules
{
    class Settings
    {
        public string CommandPrefix { get; set; }
        public string BotToken { get; set; }
        public string BotStatus { get; set; }

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

        public void SetToDefault(string filePath)
        {
            CommandPrefix = "!";
            BotToken = "BOT TOKEN";
            BotStatus = "!help";
            this.SaveToJson(filePath);
        }
    }
}