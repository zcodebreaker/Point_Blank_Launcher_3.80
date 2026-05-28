using Launcher.PointBlank.Models;
using Launcher.PointBlank.Properties;
using Launcher.PointBlank.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Launcher.PointBlank
{
    public partial class Main : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        string startupPath = Application.StartupPath;
        private bool isChecking = false;
        private bool _isLoggedIn = false;
        private string _loggedUsername = string.Empty;
        private string _loggedPassword = string.Empty;
        private readonly LauncherConnectionResult _connectionResult;
        private readonly LauncherConnection _connection;

        public Main(LauncherConnectionResult connectionResult, LauncherConnection connection)
        {
            InitializeComponent();
            _connectionResult = connectionResult;
            _connection = connection;
            Load += Main_Load;
            FormClosed += (s, e) => _connection?.Dispose();
            TEXT_STATUS.Text = "Você pode iniciar o jogo.";
            LogoutButton.Visible = false;
            TEXT_USER.Visible = false;
        }
        #region Button Effects
        private void CheckButton_MouseEnter(object sender, EventArgs e)
        {
            if (isChecking)
                return;
            CheckButton.BackgroundImage = Resources.CheckEnter;
            CheckButton.BackColor = Color.Transparent;
        }
        private void CheckButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (isChecking)
                return;
            CheckButton.BackgroundImage = Resources.CheckDown;
            CheckButton.BackColor = Color.Transparent;
        }
        private void CheckButton_MouseLeave(object sender, EventArgs e)
        {
            if (isChecking)
                return;
            CheckButton.BackgroundImage = Resources.CheckLeave;
            CheckButton.BackColor = Color.Transparent;
        }
        private void StartButton_MouseEnter(object sender, EventArgs e)
        {
            StartButton.BackgroundImage = Resources.StartEnter;
            StartButton.BackColor = Color.Transparent;
        }
        private void StartButton_MouseDown(object sender, MouseEventArgs e)
        {
            StartButton.BackgroundImage = Resources.StartDown;
            StartButton.BackColor = Color.Transparent;
        }
        private void StartButton_MouseLeave(object sender, EventArgs e)
        {
            StartButton.BackgroundImage = Resources.StartLeave;
            StartButton.BackColor = Color.Transparent;
        }
        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            CloseButton.BackgroundImage = Resources.Bitmap132;
            CloseButton.BackColor = Color.Transparent;
        }
        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            CloseButton.BackgroundImage = Resources.Bitmap133;
            CloseButton.BackColor = Color.Transparent;
        }
        private void MinimizeButton_MouseEnter(object sender, EventArgs e)
        {
            MinimizeButton.BackgroundImage = Resources.Bitmap137;
            MinimizeButton.BackColor = Color.Transparent;
        }
        private void MinimizeButton_MouseLeave(object sender, EventArgs e)
        {
            MinimizeButton.BackgroundImage = Resources.Bitmap136;
            MinimizeButton.BackColor = Color.Transparent;
        }
        private void ConfigButton_MouseEnter(object sender, EventArgs e)
        {
            ConfigButton.BackgroundImage = Resources.Bitmap1270;
            ConfigButton.BackColor = Color.Transparent;
        }
        private void ConfigButton_MouseLeave(object sender, EventArgs e)
        {
            ConfigButton.BackgroundImage = Resources.Bitmap1271;
            ConfigButton.BackColor = Color.Transparent;
        }
        private void UpdateButton_MouseEnter(object sender, EventArgs e)
        {

            UpdateButton.BackgroundImage = Resources.UpdateEnter;
            UpdateButton.BackColor = Color.Transparent;
        }
        private void UpdateButton_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateButton.BackgroundImage = Resources.UpdateDown;
            UpdateButton.BackColor = Color.Transparent;
        }
        private void UpdateButton_MouseLeave(object sender, EventArgs e)
        {

            UpdateButton.BackgroundImage = Resources.UpdateLeave;
            UpdateButton.BackColor = Color.Transparent;
        }
        private void LoginScreen_MouseEnter(object sender, EventArgs e)
        {
            LoginScreen.BackgroundImage = Resources.Bitmap16;
            LoginScreen.BackColor = Color.Transparent;
        }
        private void LoginScreen_MouseDown(object sender, MouseEventArgs e)
        {
            LoginScreen.BackgroundImage = Resources.Bitmap17;
            LoginScreen.BackColor = Color.Transparent;
        }
        private void LoginScreen_MouseLeave(object sender, EventArgs e)
        {
            LoginScreen.BackgroundImage = Resources.Bitmap15;
            LoginScreen.BackColor = Color.Transparent;
        }
        private void LogoutButton_MouseEnter(object sender, EventArgs e)
        {
            LogoutButton.BackgroundImage = Resources.Bitmap19;
            LogoutButton.BackColor = Color.Transparent;
        }
        private void LogoutButton_MouseDown(object sender, MouseEventArgs e)
        {
            LogoutButton.BackgroundImage = Resources.Bitmap20;
            LogoutButton.BackColor = Color.Transparent;
        }
        private void LogoutButton_MouseLeave(object sender, EventArgs e)
        {
            LogoutButton.BackgroundImage = Resources.Bitmap18;
            LogoutButton.BackColor = Color.Transparent;
        }
        #endregion
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        public void SetProgressBar(ulong received, ulong maximum)
        {
            if (FileBar.Width <= 463)
            {
                FileBar.Width = (int)(received * 463 / maximum);
            }
        }
        public void Set2ProgressBar(ulong received, ulong maximum)
        {
            if (TotalBar.Width <= 463)
            {
                TotalBar.Width = (int)(received * 463 / maximum);
            }
        }
        public void SetButtonsVisible(bool start, bool check, bool update)
        {
            StartButton.Visible = start;
            CheckButton.Visible = check;
            UpdateButton.Visible = update;
        }
        public void SetButtonsEnable(bool start, bool check, bool update)
        {
            StartButton.Enabled = start;
            CheckButton.Enabled = check;
            UpdateButton.Enabled = update;
        }
        private void ConfigButton_Click(object sender, EventArgs e)
        {
            string configPath = Path.Combine(startupPath, "PBConfig.exe");

            if (File.Exists(configPath))
            {
                Process.Start(configPath);
            }
            else
            {
                MessageBox.Show("PBConfig.exe não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("Você tem certeza que deseja sair?", "PBLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            Logger.Log($"Launcher finalizado");
        }
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private async void CheckButton_Click(object sender, EventArgs e)
        {
            isChecking = true;
            CheckButton.BackgroundImage = Resources.CheckDisable;
            await StartFileCheckAsync();
        }
        private async Task StartFileCheckAsync()
        {
            CheckButton.BackgroundImage = Resources.CheckDisable; // check disable
            StartButton.BackgroundImage = Resources.StartDisable; // start disable

            SetButtonsEnable(false, false, false);
            SetButtonsVisible(true, true, false);


            TEXT_STATUS.Text = "Check";
            FILE_TEXT.Visible = true;
            Logger.Log($"Verificando a integridade dos arquivos.");
            Dictionary<string, string> userFiles = LoadUserFile(Path.Combine(Application.StartupPath, "UserFileList.dat"));

            var checkService = new FileCheck(Application.StartupPath);

            var progress = new Progress<FileCheckProgress>(p =>
            {
                FILE_TEXT.Text = $"File {p.CurrentFile}";

                FileBar.Width = p.FileBarWidth;
                TotalBar.Width = p.TotalBarWidth;

                Set2ProgressBar((ulong)p.CurrentIndex, (ulong)p.TotalFiles);
            });

            FileCheckResult result = await checkService.CheckFilesAsync(userFiles, progress);

            if (!result.Success)
            {
                MessageBox.Show( result.Message, "PBLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Aqui depois entra:
                // await DownloadInvalidFilesAsync(result.InvalidFiles);

                return;
            }

            FinishCheckSuccess();
        }
        private void FinishCheckSuccess()
        {
            FILE_TEXT.Visible = true;

            SetButtonsEnable(true, true, false);
            SetButtonsVisible(true, true, false);

            FileBar.Width = 463;
            TotalBar.Width = 463;

            FILE_TEXT.Text = "File";
            TEXT_STATUS.Text = "Fim da verificação. Você já pode jogar.";

            StartButton.BackgroundImage = Resources.StartLeave; // ajuste para sua imagem normal
            CheckButton.BackgroundImage = Resources.CheckLeave; // ajuste para sua imagem normal
        }
        public static Dictionary<string, string> LoadUserFile(string path)
        {
            Dictionary<string, string> strs = new Dictionary<string, string>();
            XmlDocument xmlDocument = new XmlDocument();
            if (!File.Exists(path))
            {
                Logger.Log($"COMMAND_FILE_CHECK_START: UserFileList.dat não encontrado em {path}.");
                return strs;
            }

            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fileStream.Length == 0)
                        return strs;

                    xmlDocument.Load(fileStream);
                }

                XmlNode listNode = xmlDocument.SelectSingleNode("/list");
                if (listNode == null)
                    return strs;

                foreach (XmlNode fileNode in listNode.ChildNodes)
                {
                    if (!"file".Equals(fileNode.Name) || fileNode.Attributes == null)
                        continue;

                    string filePath = GetAttributeValue(fileNode.Attributes, "local", "n");
                    string hash = GetAttributeValue(fileNode.Attributes, "hash", "m");

                    if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(hash))
                    {
                        Logger.Log($"COMMAND_FILE_CHECK_START: Entrada inválida no UserFileList.dat.");
                        continue;
                    }

                    if (!strs.ContainsKey(filePath))
                        strs.Add(filePath, hash);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"COMMAND_FILE_CHECK_START: Ocorreu um erro ao ler os arquivos da lista. {ex.Message}");
            }

            return strs;
        }
        private static string GetAttributeValue(XmlNamedNodeMap attributes, params string[] names)
        {
            foreach (string name in names)
            {
                XmlNode node = attributes.GetNamedItem(name);
                if (node != null)
                    return node.Value;
            }

            return string.Empty;
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!_isLoggedIn)
            {
                using (Login login = new Login(_connectionResult, _connection))
                {
                    if (login.ShowDialog(this) != DialogResult.OK)
                        return;

                    _loggedUsername = login.LoggedUsername;
                    _loggedPassword = login.LoggedPassword;
                    _isLoggedIn = true;
                    SetLoggedInState(_loggedUsername);
                }
            }
            try
            {
                ClientConfig config = ClientConfigService.FromFolder(Application.StartupPath).Load();
                string exePath = Path.Combine(Application.StartupPath, config.Executable);

                if (File.Exists(exePath))
                {
                    Process.Start(exePath, $"{_loggedUsername} {_loggedPassword}");
                    Application.Exit();
                }
                else
                {
                    Logger.Log($"Não foi possível iniciar o jogo: {config.Executable} não encontrado.");
                    MessageBox.Show($"{config.Executable} não encontrado.", "PBLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Não foi possível iniciar o jogo. {ex}");
                MessageBox.Show($"Não foi possível iniciar o jogo.", "PBLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {
            bool clientOk = CheckClientVersion();
            if (!clientOk)
                return;
            bool launcherOk = CheckLauncherVersion();
            if (!launcherOk)
                return;
        }
        private bool CheckClientVersion()
        {
            ClientConfigService clientConfigService = ClientConfigService.FromFolder(Application.StartupPath);
            ClientConfig localConfig = clientConfigService.Load();

            long localVersion;
            long serverVersion;

            bool localOk = long.TryParse(localConfig.ClientVersion, out localVersion);
            bool serverOk = long.TryParse(_connectionResult.ClientVersion, out serverVersion);

            if (!localOk || !serverOk || localVersion < serverVersion)
            {
                TEXT_STATUS.Text = "Clique no botão Update.";
                Logger.Log($"UPDATER_STATE_PRE_CLIENT_VERSION: {localConfig.ClientVersion}");

                StartButton.BackgroundImage = Resources.Bitmap171;
                CheckButton.BackgroundImage = Resources.Bitmap163;

                SetButtonsVisible(false, true, true);
                SetButtonsEnable(false, false, true);

                FileBar.Width = 0;
                TotalBar.Width = 0;

                return false;
            }
            TEXT_STATUS.Text = "Você pode iniciar o jogo.";

            SetButtonsVisible(true, true, false);
            SetButtonsEnable(true, true, false);
            return true;
        }
        private bool CheckLauncherVersion()
        {
            LauncherConfigService launcherConfigService = LauncherConfigService.FromFolder(Application.StartupPath);
            LauncherConfig localConfig = launcherConfigService.Load();

            long localVersion;
            long serverVersion;

            bool localOk = long.TryParse(localConfig.LauncherVersion, out localVersion);
            bool serverOk = long.TryParse(_connectionResult.LauncherVersion, out serverVersion);

            if (!localOk || !serverOk || localVersion < serverVersion)
            {
                TEXT_STATUS.Text = "Clique no botão Update.";
                Logger.Log($"UPDATER_STATE_PRE_LAUNCHER_VERSION: {localConfig.LauncherVersion}");

                SetButtonsVisible(false, false, true);
                SetButtonsEnable(false, false, true);

                FileBar.Width = 0;
                TotalBar.Width = 0;
                return false;
            }

            SetButtonsVisible(true, true, false);
            SetButtonsEnable(true, true, false);

            return true;
        }

        private async void UpdateButton_Click(object sender, EventArgs e)
        {
            SetButtonsEnable(false, false, false);
            SetButtonsVisible(false, false, false);

            FILE_TEXT.Visible = true;
            FileBar.Width = 0;
            TotalBar.Width = 0;
            TEXT_STATUS.Text = "Fazendo download dos arquivos de patch.";

            try
            {
                UpdateManifest manifest = await _connection.GetManifestAsync();

                LauncherUpdateCheckService checkService = new LauncherUpdateCheckService(Application.StartupPath);
                List<UpdateFile> filesToUpdate = checkService.GetFilesToUpdate(manifest);

                if (filesToUpdate.Count == 0)
                {
                    FileBar.Width = 463;
                    TotalBar.Width = 463;
                    FILE_TEXT.Visible = false;
                    TEXT_STATUS.Text = "Você pode iniciar o jogo.";
                    SetButtonsEnable(true, true, false);
                    SetButtonsVisible(true, true, false);
                    return;
                }
                Logger.Log($"Command: UPDATER_STATE_CLIENT_VERSION");

                var progress = new Progress<UpdateDownloadProgress>(p =>
                {
                    FILE_TEXT.Text = $"File {p.CurrentFile}";

                    FileBar.Width = p.TotalBytes > 0
                        ? (int)(p.BytesReceived * 463 / p.TotalBytes)
                        : 0;

                    TotalBar.Width = p.TotalFiles > 0
                        ? (int)(p.CurrentFileIndex * 463 / p.TotalFiles)
                        : 0;

                    TEXT_STATUS.Text = $"[{p.CurrentFileIndex}/{p.TotalFiles}] {Path.GetFileName(p.CurrentFile)}";
                });

                PatchDownloadService downloadService = new PatchDownloadService(Application.StartupPath, _connection);
                await downloadService.DownloadFilesAsync(filesToUpdate, progress);

                FileBar.Width = 463;
                TotalBar.Width = 463;
                FILE_TEXT.Text = "File";
                TEXT_STATUS.Text = "Você pode iniciar o jogo.";
                SetButtonsEnable(true, true, false);
                SetButtonsVisible(true, true, false);
                CheckButton.BackgroundImage = Resources.Bitmap164;
                StartButton.BackgroundImage = Resources.Bitmap172;
                Logger.Log("Logger: Atualização finalizada.");
            }
            catch (Exception ex)
            {
                FileBar.Width = 0;
                TotalBar.Width = 0;
                FILE_TEXT.Text = "File";
                TEXT_STATUS.Text = "Erro durante a atualização.";
                SetButtonsEnable(false, false, true);
                SetButtonsVisible(false, false, true);
                Logger.Log($"Erro no update: {ex.Message}");
                MessageBox.Show($"Erro durante a atualização.\n{ex.Message}", "PBLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoginButton_Click(object sender, EventArgs e)
        {
            using (Login login = new Login(_connectionResult, _connection))
            {
                if (login.ShowDialog(this) == DialogResult.OK)
                {
                    _loggedUsername = login.LoggedUsername;
                    _loggedPassword = login.LoggedPassword;
                    _isLoggedIn = true;
                    SetLoggedInState(_loggedUsername);
                }
            }
        }
        private void SetLoggedInState(string username)
        {
            TEXT_USER.Text = username;
            TEXT_USER.Visible = true;
            LogoutButton.Visible = true;
        }
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            _isLoggedIn = false;
            _loggedUsername = string.Empty;
            _loggedPassword = string.Empty;
            TEXT_USER.Text = string.Empty;
            TEXT_USER.Visible = false;
            LogoutButton.Visible = false;
        }
    }
}
