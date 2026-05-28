using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services.Config
{
   public class ServerConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string LauncherVersion { get; set; }
        public string ClientVersion { get; set; }
        public bool Maintenance { get; set; }
        public string MaintenanceMessage { get; set; }
        public string ManifestPath { get; set; }
        public string ManifestUrl { get; set; }
        public string ClientFilesPath { get; set; }
        public string LauncherFilesPath { get; set; }
        public int MaxPacketSize { get; set; }
        public bool EnablePacketLog { get; set; }
        public string DbHost { get; set; }
        public int DbPort { get; set; }
        public string DbName { get; set; }
        public string DbUser { get; set; }
        public string DbPassword { get; set; }

        public ServerConfig()
        {
            Host = "0.0.0.0";
            Port = 9000;

            LauncherVersion = "202605";
            ClientVersion = "20260514";
            Maintenance = false;
            MaintenanceMessage = "Servidor em manutenção.";

            ManifestPath = @"Info\manifest.json";
            ManifestUrl = "http://localhost/patch/manifest.json";
            ClientFilesPath = @"Data\Client";
            LauncherFilesPath = @"Data\Launcher";

            MaxPacketSize = 10485760;
            EnablePacketLog = true;

            DbHost = string.Empty;
            DbPort = 0;
            DbName = string.Empty;
            DbUser = string.Empty;
            DbPassword = string.Empty;
        }
    }
}
