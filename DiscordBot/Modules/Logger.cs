using Discord;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    class Logger
    {
        private string logFilePath;

        public enum LogType
        {
            Info,
            Debug,
            Warning,
            Error,
            Fatal,
            Discord
        }

        public Logger(string filePath)
        {
            logFilePath = filePath;
        }

        public void ConsoleLog(LogType logType, string message)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [{logType}] {message}");
        }

        public void FileLog(LogType logType, string message)
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now.ToLongTimeString()} [{logType}] {message}");
        }

        public Task DiscordLog(LogMessage message)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [{LogType.Discord}] {message}");
            return Task.CompletedTask;
        }
    }
}
