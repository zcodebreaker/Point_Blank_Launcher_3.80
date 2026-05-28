using Launcher.Server.Config;
using Launcher.Services.Config;
using Launcher.Services.Database;
using Launcher.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Launcher.Server.Network
{
    public class LAUNCHER_CLIENT_SESSION_ACK
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly ServerConfig _config;

        public LAUNCHER_CLIENT_SESSION_ACK(TcpClient client, ServerConfig config)
        {
            _client = client;
            _stream = client.GetStream();
            _config = config;
        }
        public void Start()
        {
            try
            {
                Console.WriteLine("CLIENT ADRESS: " + _client.Client.RemoteEndPoint);

                while (_client.Connected)
                {
                    LAUNCHER_PACKET_ACK packet = LAUNCHER_PACKET_READER_ACK.Read(_stream);

                    HandlePacket(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Status: " + ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        private void HandlePacket(LAUNCHER_PACKET_ACK packet)
        {
            switch ((LAUNCHER_OPCODE_ACK)packet.Opcode)
            {
                case LAUNCHER_OPCODE_ACK.LAUNCHER_CONNECT_REQ:
                    HandleLauncherConnect();
                    break;

                case LAUNCHER_OPCODE_ACK.LAUNCHER_MANIFEST_REQ:
                    HandleManifestRequest();
                    break;

                case LAUNCHER_OPCODE_ACK.LAUNCHER_FILE_REQ:
                    HandleFileRequest(packet);
                    break;

                case LAUNCHER_OPCODE_ACK.LAUNCHER_LOGIN_REQ:
                    HandleLoginRequest(packet);
                    break;

                default:
                    SendString(
                        LAUNCHER_OPCODE_ACK.LAUNCHER_ERROR_ACK,
                        "Opcode desconhecido: " + packet.Opcode
                    );
                    break;
            }
        }
        private void HandleLauncherConnect()
        {
            Console.WriteLine("Packet Recive: LAUNCHER_CONNECT_REQ");
            if (_config.Maintenance)
            {
                Infos response = new Infos
                {
                    Success = false,
                    Message = _config.MaintenanceMessage,
                    LauncherVersion = _config.LauncherVersion,
                    ClientVersion = _config.ClientVersion,
                    Maintenance = true,
                    MaintenanceMessage = _config.MaintenanceMessage
                };
                SendJson(LAUNCHER_OPCODE_ACK.LAUNCHER_CONNECT_FAIL, response);

                Console.WriteLine("LAUNCHER_CONNECT_FAIL : Manutenção");
                return;
            }
            Infos successResponse = new Infos
            {
                Success = true,
                Message = "Conexão estabelecida com sucesso.",
                LauncherVersion = _config.LauncherVersion,
                ClientVersion = _config.ClientVersion,
                Maintenance = false,
                MaintenanceMessage = _config.MaintenanceMessage,
                ManifestUrl = _config.ManifestUrl
            };
            SendJson(LAUNCHER_OPCODE_ACK.LAUNCHER_CONNECT_ACK, successResponse);
            Console.WriteLine("Packet Send: LAUNCHER_CONNECT_ACK");
        }
        private void SendJson(LAUNCHER_OPCODE_ACK opcode, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] payload = Encoding.UTF8.GetBytes(json);

            byte[] packet = LAUNCHER_PACKET_WRITER_ACK.Build(opcode, payload);

            _stream.Write(packet, 0, packet.Length);
        }
        private void SendString(LAUNCHER_OPCODE_ACK opcode, string message)
        {
            byte[] payload = Encoding.UTF8.GetBytes(message);

            byte[] packet = LAUNCHER_PACKET_WRITER_ACK.Build(opcode, payload);

            _stream.Write(packet, 0, packet.Length);
        }
        private void HandleManifestRequest()
        {
            Console.WriteLine("Packet Receive: LAUNCHER_MANIFEST_REQ");

            string manifestPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                _config.ManifestPath
            );

            if (!File.Exists(manifestPath))
            {
                SendString(LAUNCHER_OPCODE_ACK.LAUNCHER_MANIFEST_FAIL, "Manifest não encontrado.");
                Console.WriteLine("LAUNCHER_MANIFEST_FAIL: arquivo não encontrado em " + manifestPath);
                return;
            }

            byte[] payload = File.ReadAllBytes(manifestPath);
            byte[] packet = LAUNCHER_PACKET_WRITER_ACK.Build(LAUNCHER_OPCODE_ACK.LAUNCHER_MANIFEST_ACK, payload);
            _stream.Write(packet, 0, packet.Length);

            Console.WriteLine("Packet Send: LAUNCHER_MANIFEST_ACK");
        }
        private void HandleFileRequest(LAUNCHER_PACKET_ACK packet)
        {
            Console.WriteLine("Packet Receive: LAUNCHER_FILE_REQ");

            string json = Encoding.UTF8.GetString(packet.Payload);
            string relativePath = JObject.Parse(json)["path"].ToString();

            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                _config.ClientFilesPath,
                relativePath.TrimStart('/', '\\')
            );
            if (!File.Exists(fullPath))
            {
                SendString(LAUNCHER_OPCODE_ACK.LAUNCHER_FILE_FAIL, "Arquivo não encontrado: " + relativePath);
                Console.WriteLine("LAUNCHER_FILE_FAIL: " + relativePath);
                return;
            }
            FileInfo fileInfo = new FileInfo(fullPath);
            long fileSize = fileInfo.Length;

            byte[] infoPayload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { size = fileSize }));
            byte[] infoPacket = LAUNCHER_PACKET_WRITER_ACK.Build(LAUNCHER_OPCODE_ACK.LAUNCHER_FILE_INFO_ACK, infoPayload);
            _stream.Write(infoPacket, 0, infoPacket.Length);

            const int chunkSize = 1024 * 1024;
            byte[] buffer = new byte[chunkSize];

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, chunkSize)) > 0)
                {
                    byte[] chunkPayload = new byte[bytesRead];
                    Array.Copy(buffer, chunkPayload, bytesRead);
                    byte[] chunkPacket = LAUNCHER_PACKET_WRITER_ACK.Build(LAUNCHER_OPCODE_ACK.LAUNCHER_FILE_DATA_ACK, chunkPayload);
                    _stream.Write(chunkPacket, 0, chunkPacket.Length);
                }
            }

            byte[] endPacket = LAUNCHER_PACKET_WRITER_ACK.Build(LAUNCHER_OPCODE_ACK.LAUNCHER_FILE_END_ACK);
            _stream.Write(endPacket, 0, endPacket.Length);

            Console.WriteLine($"Packet Send: LAUNCHER_FILE_END_ACK [{relativePath}] ({fileSize} bytes)");
        }

        private void HandleLoginRequest(LAUNCHER_PACKET_ACK packet)
        {
            Console.WriteLine("Packet Receive: LAUNCHER_LOGIN_REQ");

            try
            {
                string json = Encoding.UTF8.GetString(packet.Payload);
                LoginRequest request = JsonConvert.DeserializeObject<LoginRequest>(json);

                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    SendJson(LAUNCHER_OPCODE_ACK.LAUNCHER_LOGIN_FAIL, new LoginResult
                    {
                        Success = false,
                        Message = "Dados de login inválidos."
                    });
                    return;
                }

                DatabaseService db = new DatabaseService(
                    _config.DbHost, _config.DbPort, _config.DbName, _config.DbUser, _config.DbPassword);

                bool valid = db.ValidateLogin(request.Username, request.Password);

                if (valid)
                {
                    SendJson(LAUNCHER_OPCODE_ACK.LAUNCHER_LOGIN_ACK, new LoginResult
                    {
                        Success = true,
                        Message = "Login realizado com sucesso."
                    });
                    Console.WriteLine("Packet Send: LAUNCHER_LOGIN_ACK [" + request.Username + "]");
                }
                else
                {
                    SendJson(LAUNCHER_OPCODE_ACK.LAUNCHER_LOGIN_FAIL, new LoginResult
                    {
                        Success = false,
                        Message = "Usuário ou senha incorretos."
                    });
                    Console.WriteLine("Packet Send: LAUNCHER_LOGIN_FAIL [" + request.Username + "]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no login: " + ex.Message);
                SendJson(LAUNCHER_OPCODE_ACK.LAUNCHER_LOGIN_FAIL, new LoginResult
                {
                    Success = false,
                    Message = "Erro interno no servidor."
                });
            }
        }

        private void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch
            {

            }
        }
    }
}