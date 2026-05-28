using Launcher.PointBlank.Models;
using Launcher.PointBlank.Properties;
using Launcher.PointBlank.Services;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.PointBlank
{
    public partial class Login : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private readonly LauncherConnection _connection;
        private readonly LoginService _loginService;

        public string LoggedUsername { get; private set; }
        public string LoggedPassword { get; private set; }

        public Login(LauncherConnectionResult connectionResult, LauncherConnection connection)
        {
            InitializeComponent();
            AlignLoginTextBoxes();
            LoadSavedUsername();
            Shown += Login_Shown;
            _connection = connection;
            _loginService = new LoginService(connection);
        }
        private void AlignLoginTextBoxes()
        {
            AlignLoginTextBox(Login_Box);
            AlignLoginTextBox(Password_Box);
        }
        private void AlignLoginTextBox(TextBox textBox)
        {
            Rectangle originalBounds = textBox.Bounds;

            textBox.Multiline = false;
            textBox.TextAlign = HorizontalAlignment.Left;

            int centeredY = originalBounds.Y + (originalBounds.Height - textBox.PreferredHeight) / 2;
            textBox.SetBounds(originalBounds.X, centeredY, originalBounds.Width, textBox.PreferredHeight);
        }
        private void LoadSavedUsername()
        {
            string savedUsername = Settings.Default.SavedUsername;

            if (string.IsNullOrWhiteSpace(savedUsername))
                return;

            Login_Box.Text = savedUsername;
            LoginSave.Checked = true;
        }
        private void SaveUsernamePreference(string username)
        {
            Settings.Default.SavedUsername = LoginSave.Checked ? username : string.Empty;
            Settings.Default.Save();
        }
        private void Login_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                TextBox focusedTextBox = LoginSave.Checked ? Password_Box : Login_Box;

                focusedTextBox.Focus();
                ClearTextSelection(Login_Box);
                ClearTextSelection(Password_Box);
            }));
        }
        private void ClearTextSelection(TextBox textBox)
        {
            textBox.SelectionStart = textBox.TextLength;
            textBox.SelectionLength = 0;
        }
        #region ButtonEffects
        private void LoginButton_MouseEnter(object sender, EventArgs e)
        {
            LoginButton.BackgroundImage = Resources.Bitmap13;
            LoginButton.BackColor = Color.Transparent;
        }
        private void LoginButton_MouseDown(object sender, MouseEventArgs e)
        {
            LoginButton.BackgroundImage = Resources.Bitmap14;
            LoginButton.BackColor = Color.Transparent;
        }
        private void LoginButton_MouseLeave(object sender, EventArgs e)
        {
            LoginButton.BackgroundImage = Resources.Bitmap12;
            LoginButton.BackColor = Color.Transparent;
        }
        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            CloseButton.BackgroundImage = Resources.Bitmap23;
            CloseButton.BackColor = Color.Transparent;
        }
        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            CloseButton.BackgroundImage = Resources.Bitmap21;
            CloseButton.BackColor = Color.Transparent;
        }
        #endregion
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            string username = Login_Box.Text.Trim();
            string password = Password_Box.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preencha o usuário e a senha.", "PBLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoginButton.Enabled = false;

            try
            {
                LoginResult result = await _loginService.LoginAsync(username, password);

                if (result.Success)
                {
                    SaveUsernamePreference(username);
                    LoggedUsername = username;
                    LoggedPassword = password;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "PBLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoginButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao realizar login:\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoginButton.Enabled = true;
            }
        }
    }
}
