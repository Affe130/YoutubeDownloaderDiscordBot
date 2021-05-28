using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    class FtpClient
    {
        public string host { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        public FtpClient(string host, string userName, string password)
        {
            this.host = host;
            this.userName = userName;
            this.password = password;
        }

        public async Task UploadFile(string path)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("anonymous", userName);

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(path))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Program.logger.ConsoleLog(Logger.LogType.Info, $"Upload File Complete, status {response.StatusDescription}");
            }
        }
    }
}
