using Launcher.PointBlank.Models;
using System.IO;
using System.Xml.Linq;

namespace Launcher.PointBlank.Services
{
    public class ClientConfigService
    {
        public const string EncryptedFileName = "config.zpt";

        private readonly string _folderPath;
        public ClientConfigService(string folderPath)
        {
            _folderPath = folderPath;
        }
        public static ClientConfigService FromFolder(string folderPath)
        {
            return new ClientConfigService(folderPath);
        }
        public ClientConfig Load()
        {
            string encryptedPath = Path.Combine(_folderPath, EncryptedFileName);

            if (!File.Exists(encryptedPath))
                throw new FileNotFoundException("Arquivo de configuração encriptado não encontrado.", encryptedPath);

            XDocument document = XDocument.Parse(ConfigCryptoService.DecryptFile(encryptedPath));

            XElement root = document.Element("CLIENT_CONFIG");
            if (root == null)
            {
                return new ClientConfig();
            }
            XElement versionElement    = root.Element("CLIENT_VERSION");
            XElement ipElement         = root.Element("IP_ADRESS");
            XElement portElement       = root.Element("PORT");
            XElement executableElement = root.Element("EXECUTABLE");

            int port = 9000;
            if (portElement != null)
                int.TryParse(portElement.Value.Trim(), out port);

            return new ClientConfig
            {
                ClientVersion = versionElement    != null ? versionElement.Value.Trim()    : "0",
                IpAddress     = ipElement         != null ? ipElement.Value.Trim()         : "127.0.0.1",
                Port          = port,
                Executable    = executableElement != null ? executableElement.Value.Trim() : "PointBlank.exe"
            };
        }
    }
}
