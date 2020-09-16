namespace Keeper.Client
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.accounts = new System.Windows.Forms.ListView();
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.logout = new System.Windows.Forms.Button();
            this.search = new System.Windows.Forms.TextBox();
            this.account = new System.Windows.Forms.TextBox();
            this.accountLabel = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.id = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.addNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // accounts
            // 
            this.accounts.HideSelection = false;
            this.accounts.Location = new System.Drawing.Point(12, 81);
            this.accounts.Name = "accounts";
            this.accounts.Size = new System.Drawing.Size(360, 333);
            this.accounts.TabIndex = 0;
            this.accounts.UseCompatibleStateImageBehavior = false;
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.Location = new System.Drawing.Point(9, 21);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(77, 13);
            this.welcomeLabel.TabIndex = 1;
            this.welcomeLabel.Text = "Welcome User";
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(297, 16);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 2;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // logout
            // 
            this.logout.Location = new System.Drawing.Point(216, 16);
            this.logout.Name = "logout";
            this.logout.Size = new System.Drawing.Size(75, 23);
            this.logout.TabIndex = 3;
            this.logout.Text = "Logout";
            this.logout.UseVisualStyleBackColor = true;
            // 
            // search
            // 
            this.search.Location = new System.Drawing.Point(12, 55);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(360, 20);
            this.search.TabIndex = 4;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(12, 444);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(360, 20);
            this.account.TabIndex = 5;
            // 
            // accountLabel
            // 
            this.accountLabel.AutoSize = true;
            this.accountLabel.Location = new System.Drawing.Point(9, 428);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(47, 13);
            this.accountLabel.TabIndex = 6;
            this.accountLabel.Text = "Account";
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(9, 478);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(18, 13);
            this.idLabel.TabIndex = 7;
            this.idLabel.Text = "ID";
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(12, 494);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(360, 20);
            this.id.TabIndex = 8;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(12, 541);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(360, 20);
            this.password.TabIndex = 10;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(9, 525);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 9;
            this.passwordLabel.Text = "Password";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(157, 576);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 11;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            // 
            // addNew
            // 
            this.addNew.Location = new System.Drawing.Point(135, 16);
            this.addNew.Name = "addNew";
            this.addNew.Size = new System.Drawing.Size(75, 23);
            this.addNew.TabIndex = 12;
            this.addNew.Text = "Add New";
            this.addNew.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 601);
            this.Controls.Add(this.addNew);
            this.Controls.Add(this.save);
            this.Controls.Add(this.password);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.id);
            this.Controls.Add(this.idLabel);
            this.Controls.Add(this.accountLabel);
            this.Controls.Add(this.account);
            this.Controls.Add(this.search);
            this.Controls.Add(this.logout);
            this.Controls.Add(this.close);
            this.Controls.Add(this.welcomeLabel);
            this.Controls.Add(this.accounts);
            this.Name = "MainForm";
            this.Text = "Keeper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView accounts;
        private System.Windows.Forms.Label welcomeLabel;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button logout;
        private System.Windows.Forms.TextBox search;
        private System.Windows.Forms.TextBox account;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.TextBox id;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button addNew;
    }
}