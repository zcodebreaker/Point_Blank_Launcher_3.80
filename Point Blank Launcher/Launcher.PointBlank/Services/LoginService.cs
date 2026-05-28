using Launcher.PointBlank.Models;
using Launcher.PointBlank.Network;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.PointBlank.Services
{
    public class LoginService
    {
        private readonly LauncherConnection _connection;

        public LoginService(LauncherConnection connection)
        {
            _connection = connection;
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            string passwordMd5 = ComputeMd5(password);

            string json = JsonConvert.SerializeObject(new
            {
                Username = username,
                Password = passwordMd5
            });

            return await _connection.SendLoginAsync(json);
        }

        private static string ComputeMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
