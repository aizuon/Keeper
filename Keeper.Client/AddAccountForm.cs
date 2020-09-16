using System.Drawing;
using System.Windows.Forms;

namespace Keeper.Client
{
    public partial class AddAccountForm : Form
    {
        public AddAccountForm()
        {
            InitializeComponent();
        }

        private void AddAccountForm_MouseDown(object sender, MouseEventArgs e)
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
        }

        public void SetErrorLabel(string error)
        {
            errorLabel.ForeColor = Color.Red;
            errorLabel.Text = error;
        }

        private void save_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(account.Text))
            {
                SetErrorLabel("Account can't be empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(id.Text))
            {
                SetErrorLabel("ID can't be empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                SetErrorLabel("Password can't be empty.");
                return;
            }

            Client.Instance.Send_AccountAddReq(account.Text, id.Text, password.Text);
        }
    }
}
