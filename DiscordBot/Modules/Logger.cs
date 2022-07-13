using Discord;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Logger
    {
        private readonly string LogFilePath;

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
            LogFilePath = filePath;
        }

        public void ConsoleLog(LogType logType, string message)
        {
            Console.WriteLine($"{DateTime.Now} [{logType}] {message}");
            FileLog(logType, message);
        }

        public void FileLog(LogType logType, string message)
        {
            File.AppendAllText(LogFilePath, $"{DateTime.Now} [{logType}] {message}");
        }

        public Task DiscordLog(LogMessage message)
        {
            ConsoleLog(LogType.Discord, message.ToString());
            return Task.CompletedTask;
        }
    }
}
