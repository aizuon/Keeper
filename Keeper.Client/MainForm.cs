using Keeper.Common;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Keeper.Client
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;

        public MainForm(string username, List<AccountInfo> accounts)
        {
            InitializeComponent();

            welcomeLabel.Text = $"Welcome {username}";

            foreach (var account in accounts)
            {
                string[] rows = new string[] { account.Name, account.Id };
                var listViewItem = new ListViewItem(rows);
                this.accounts.Items.Add(listViewItem);
            }

            Instance = this;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Program.ReleaseCapture();
                Program.SendMessage(Handle, Program.WM_NCLBUTTONDOWN, Program.HT_CAPTION, 0);
            }
        }

        private void close_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void logout_Click(object sender, System.EventArgs e)
        {
            Client.Instance.Disconnect();
            Close();
        }

        private void addNew_Click(object sender, System.EventArgs e)
        {
            using (var addAccountForm = new AddAccountForm())
            {
                addAccountForm.ShowDialog(this);
            }
        }

        private void accounts_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (accounts.SelectedIndices.Count == 1)
            {
                var account = Client.Instance.Accounts[accounts.SelectedIndices[0]];
                this.account.Text = account.Name;
                id.Text = account.Id;
                password.Text = account.Password;
            }
            else if (accounts.SelectedIndices.Count == 0)
            {
                account.Text = string.Empty;
                id.Text = string.Empty;
                password.Text = string.Empty;
            }
        }

        private void search_TextChanged(object sender, System.EventArgs e)
        {
            accounts.Items.Clear();

            if (string.IsNullOrWhiteSpace(search.Text))
            {
                foreach (var account in Client.Instance.Accounts)
                {
                    string[] rows = new string[] { account.Name, account.Id };
                    var listViewItem = new ListViewItem(rows);
                    accounts.Items.Add(listViewItem);
                }
            }
            else
            {
                string lowerSearchText = search.Text.ToLower();

                foreach (var account in Client.Instance.Accounts)
                {
                    if (account.Name.ToLower().Contains(lowerSearchText) || account.Id.ToLower().Contains(lowerSearchText))
                    {
                        string[] rows = new string[] { account.Name, account.Id };
                        var listViewItem = new ListViewItem(rows);
                        accounts.Items.Add(listViewItem);
                    }
                }
            }
        }
    }
}
