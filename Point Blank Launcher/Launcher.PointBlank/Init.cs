using Launcher.PointBlank.Models;
using Launcher.PointBlank.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.PointBlank
{
    public partial class Init : Form
    {
        private readonly LauncherConnection connectionService;

        public Init()
        {
            InitializeComponent();

            ClientConfig config = ClientConfigService.FromFolder(Application.StartupPath).Load();
            connectionService = new LauncherConnection(config.IpAddress, config.Port);
            Load += Init_Load;
        }
        private async void Init_Load(object sender, EventArgs e)
        {
            await CheckServerAsync();
        }
        private async Task CheckServerAsync()
        {
            try
            {
                INIT_TEXT.Text = "Por favor, aguarde.";
                await Task.Delay(700);
                LauncherConnectionResult result =
                    await connectionService.CheckConnectionAsync();

                if (!result.Success)
                {
                    if (result.Maintenance)
                    {
                        MessageBox.Show( result.MaintenanceMessage, "PBLauncher",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "PBLauncher", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    Application.Exit();
                    return;
                    }

                  Logger.Log($"LauncherVersion: {result.LauncherVersion}");
                 await Task.Delay(500);

                Main main = new Main(result, connectionService);
                main.Show();
                Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Não foi possível conectar ao servidor", "PBLauncher",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}
