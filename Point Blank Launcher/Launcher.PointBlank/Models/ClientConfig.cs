namespace Launcher.PointBlank.Models
{
    public class ClientConfig
    {
        public string ClientVersion { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public string Executable { get; set; }

        public ClientConfig()
        {
            ClientVersion = "0";
            IpAddress = "127.0.0.1";
            Port = 9000;
            Executable = "PointBlank.exe";
        }
    }
}