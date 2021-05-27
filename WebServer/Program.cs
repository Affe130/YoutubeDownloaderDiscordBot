using System;
using System.IO;
using System.Net;

namespace WebServers
{
    class Program
    {
        private static string rootPath;
        private static string downloadsPath;

        static void Main(string[] args)
        {
            rootPath = Directory.GetCurrentDirectory();
            downloadsPath = Path.Combine(rootPath, "downloads");
            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }
            string server = 
            WebServer ws = new("http://localhost:25565/", server);
            Webser
        }

        public static string SendResponse(HttpListenerRequest request)
        {

        }

    }
}
