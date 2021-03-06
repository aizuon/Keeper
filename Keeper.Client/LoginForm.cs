﻿using Keeper.Common;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace Keeper.Client
{
    public partial class LoginForm : Form
    {
        public static LoginForm Instance;

        public LoginForm()
        {
            InitializeComponent();

            Instance = this;
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Program.ReleaseCapture();
                Program.SendMessage(Handle, Program.WM_NCLBUTTONDOWN, Program.HT_CAPTION, 0);
            }
        }

        private void register_Click(object sender, System.EventArgs e)
        {
            using (var registerForm = new RegisterForm())
            {
                registerForm.ShowDialog(this);
            }
        }

        private void close_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        public void SetErrorLabel(string error)
        {
            errorLabel.ForeColor = Color.Red;
            errorLabel.Text = error;
        }

        private async void login_Click(object sender, System.EventArgs e)
        {
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

            await Client.Instance.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1337));
            await Client.Instance.KeyExchangeEvent.WaitAsync();
            Client.Instance.Send_LoginReq(id.Text, password.Text);
        }

        public void MoveToMainForm(List<AccountInfo> accounts)
        {
            Hide();
            using (var mainForm = new MainForm(id.Text, accounts))
            {
                mainForm.ShowDialog(this);
            }
            if (!IsDisposed)
            {
                Show();
            }
        }
    }
}
