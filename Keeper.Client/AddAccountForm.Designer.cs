namespace Keeper.Client
{
    partial class AddAccountForm
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
            this.account = new System.Windows.Forms.TextBox();
            this.accountLabel = new System.Windows.Forms.Label();
            this.id = new System.Windows.Forms.TextBox();
            this.idLabel = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordGeneratorOptionsLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.passwordLength = new System.Windows.Forms.Label();
            this.includeSymbols = new System.Windows.Forms.CheckBox();
            this.includeLowercaseCharacters = new System.Windows.Forms.CheckBox();
            this.includeNumbers = new System.Windows.Forms.CheckBox();
            this.includeUppercaseCharacters = new System.Windows.Forms.CheckBox();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.generatePassword = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(34, 39);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(328, 20);
            this.account.TabIndex = 0;
            // 
            // accountLabel
            // 
            this.accountLabel.AutoSize = true;
            this.accountLabel.Location = new System.Drawing.Point(31, 23);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(47, 13);
            this.accountLabel.TabIndex = 1;
            this.accountLabel.Text = "Account";
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(34, 84);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(328, 20);
            this.id.TabIndex = 2;
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(31, 68);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(18, 13);
            this.idLabel.TabIndex = 3;
            this.idLabel.Text = "ID";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(34, 130);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(328, 20);
            this.password.TabIndex = 4;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(31, 114);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 5;
            this.passwordLabel.Text = "Password";
            // 
            // passwordGeneratorOptionsLabel
            // 
            this.passwordGeneratorOptionsLabel.AutoSize = true;
            this.passwordGeneratorOptionsLabel.Location = new System.Drawing.Point(31, 171);
            this.passwordGeneratorOptionsLabel.Name = "passwordGeneratorOptionsLabel";
            this.passwordGeneratorOptionsLabel.Size = new System.Drawing.Size(142, 13);
            this.passwordGeneratorOptionsLabel.TabIndex = 6;
            this.passwordGeneratorOptionsLabel.Text = "Password Generator Options";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(34, 206);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 8;
            // 
            // passwordLength
            // 
            this.passwordLength.AutoSize = true;
            this.passwordLength.Location = new System.Drawing.Point(31, 190);
            this.passwordLength.Name = "passwordLength";
            this.passwordLength.Size = new System.Drawing.Size(89, 13);
            this.passwordLength.TabIndex = 9;
            this.passwordLength.Text = "Password Length";
            // 
            // includeSymbols
            // 
            this.includeSymbols.AutoSize = true;
            this.includeSymbols.Location = new System.Drawing.Point(218, 232);
            this.includeSymbols.Name = "includeSymbols";
            this.includeSymbols.Size = new System.Drawing.Size(103, 17);
            this.includeSymbols.TabIndex = 10;
            this.includeSymbols.Text = "Include Symbols";
            this.includeSymbols.UseVisualStyleBackColor = true;
            // 
            // includeLowercaseCharacters
            // 
            this.includeLowercaseCharacters.AutoSize = true;
            this.includeLowercaseCharacters.Location = new System.Drawing.Point(34, 232);
            this.includeLowercaseCharacters.Name = "includeLowercaseCharacters";
            this.includeLowercaseCharacters.Size = new System.Drawing.Size(170, 17);
            this.includeLowercaseCharacters.TabIndex = 11;
            this.includeLowercaseCharacters.Text = "Include Lowercase Characters";
            this.includeLowercaseCharacters.UseVisualStyleBackColor = true;
            // 
            // includeNumbers
            // 
            this.includeNumbers.AutoSize = true;
            this.includeNumbers.Location = new System.Drawing.Point(218, 209);
            this.includeNumbers.Name = "includeNumbers";
            this.includeNumbers.Size = new System.Drawing.Size(106, 17);
            this.includeNumbers.TabIndex = 12;
            this.includeNumbers.Text = "Include Numbers";
            this.includeNumbers.UseVisualStyleBackColor = true;
            // 
            // includeUppercaseCharacters
            // 
            this.includeUppercaseCharacters.AutoSize = true;
            this.includeUppercaseCharacters.Location = new System.Drawing.Point(34, 255);
            this.includeUppercaseCharacters.Name = "includeUppercaseCharacters";
            this.includeUppercaseCharacters.Size = new System.Drawing.Size(170, 17);
            this.includeUppercaseCharacters.TabIndex = 13;
            this.includeUppercaseCharacters.Text = "Include Uppercase Characters";
            this.includeUppercaseCharacters.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(111, 297);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 14;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(205, 297);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 15;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // generatePassword
            // 
            this.generatePassword.Location = new System.Drawing.Point(235, 251);
            this.generatePassword.Name = "generatePassword";
            this.generatePassword.Size = new System.Drawing.Size(75, 23);
            this.generatePassword.TabIndex = 16;
            this.generatePassword.Text = "Generate";
            this.generatePassword.UseVisualStyleBackColor = true;
            // 
            // AddAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.generatePassword);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.save);
            this.Controls.Add(this.includeUppercaseCharacters);
            this.Controls.Add(this.includeNumbers);
            this.Controls.Add(this.includeLowercaseCharacters);
            this.Controls.Add(this.includeSymbols);
            this.Controls.Add(this.passwordLength);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.passwordGeneratorOptionsLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.password);
            this.Controls.Add(this.idLabel);
            this.Controls.Add(this.id);
            this.Controls.Add(this.accountLabel);
            this.Controls.Add(this.account);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AddAccountForm";
            this.Text = "AddAccountForm";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AddAccountForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox account;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.TextBox id;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label passwordGeneratorOptionsLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label passwordLength;
        private System.Windows.Forms.CheckBox includeSymbols;
        private System.Windows.Forms.CheckBox includeLowercaseCharacters;
        private System.Windows.Forms.CheckBox includeNumbers;
        private System.Windows.Forms.CheckBox includeUppercaseCharacters;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button generatePassword;
    }
}