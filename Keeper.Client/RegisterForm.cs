using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Keeper.Client
{
    public partial class RegisterForm : Form
    {
        public static RegisterForm Instance;

        public RegisterForm()
        {
            InitializeComponent();

            Instance = this;
        }

        private void RegisterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Program.ReleaseCapture();
                Program.SendMessage(Handle, Program.WM_NCLBUTTONDOWN, Program.HT_CAPTION, 0);
            }
        }

        private void cancel_Click(object sender, System.EventArgs e)
        {
            Close();
            Dispose(true);
        }

        public void SetErrorLabel(string error)
        {
            errorLabel.ForeColor = Color.Red;
            errorLabel.Text = error;
        }

        public void SetSuccessLabel(string info)
        {
            errorLabel.ForeColor = Color.Green;
            errorLabel.Text = info;
        }

        private async void register_Click(object sender, System.EventArgs e)
        {
            if (id.Text.Length < 4)
            {
                SetErrorLabel("ID must be longer than 4 characters.");
                return;
            }
            if (password.Text.Length < 4 || passwordRepeat.Text.Length < 4)
            {
#if !DEBUG
                SetErrorLabel("Password must be longer than 12 characters.");
                return;
#endif
            }
            if (password.Text != passwordRepeat.Text)
            {
                SetErrorLabel("Passwords are not equal.");
                return;
            }
            if (!password.Text.Any(char.IsUpper))
            {
#if !DEBUG
                SetErrorLabel("Password should contain an uppercase character.");
                return;
#endif
            }
            if (!password.Text.Any(char.IsLower))
            {
#if !DEBUG
                SetErrorLabel("Password should contain a lowercase character.");
                return;
#endif
            }
            if (!password.Text.Any(char.IsNumber))
            {
#if !DEBUG
                SetErrorLabel("Password should contain a number.");
                return;
#endif
            }
            if (!password.Text.Any(char.IsSymbol))
            {
#if !DEBUG
                SetErrorLabel("Password should contain a symbol.");
                return;
#endif
            }

            await Client.Instance.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1337));
            await Client.Instance.KeyExchangeEvent.WaitAsync();
            Client.Instance.Send_RegisterReq(id.Text, password.Text);
        }
    }
}
