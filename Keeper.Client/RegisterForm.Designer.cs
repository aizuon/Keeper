namespace Keeper.Client
{
    partial class RegisterForm
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
            this.passwordLabel = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.id = new System.Windows.Forms.TextBox();
            this.passwordRepeatLabel = new System.Windows.Forms.Label();
            this.passwordRepeat = new System.Windows.Forms.TextBox();
            this.cancel = new System.Windows.Forms.Button();
            this.register = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(109, 107);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 7;
            this.passwordLabel.Text = "Password";
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(109, 58);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(18, 13);
            this.idLabel.TabIndex = 6;
            this.idLabel.Text = "ID";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(112, 123);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(180, 20);
            this.password.TabIndex = 5;
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(112, 74);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(180, 20);
            this.id.TabIndex = 4;
            // 
            // passwordRepeatLabel
            // 
            this.passwordRepeatLabel.AutoSize = true;
            this.passwordRepeatLabel.Location = new System.Drawing.Point(109, 154);
            this.passwordRepeatLabel.Name = "passwordRepeatLabel";
            this.passwordRepeatLabel.Size = new System.Drawing.Size(91, 13);
            this.passwordRepeatLabel.TabIndex = 8;
            this.passwordRepeatLabel.Text = "Password Repeat";
            // 
            // passwordRepeat
            // 
            this.passwordRepeat.Location = new System.Drawing.Point(112, 170);
            this.passwordRepeat.Name = "passwordRepeat";
            this.passwordRepeat.Size = new System.Drawing.Size(180, 20);
            this.passwordRepeat.TabIndex = 9;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(217, 218);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 11;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // register
            // 
            this.register.Location = new System.Drawing.Point(112, 218);
            this.register.Name = "register";
            this.register.Size = new System.Drawing.Size(75, 23);
            this.register.TabIndex = 10;
            this.register.Text = "Register";
            this.register.UseVisualStyleBackColor = true;
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.register);
            this.Controls.Add(this.passwordRepeat);
            this.Controls.Add(this.passwordRepeatLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.idLabel);
            this.Controls.Add(this.password);
            this.Controls.Add(this.id);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegisterForm";
            this.Text = "RegisterForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox id;
        private System.Windows.Forms.Label passwordRepeatLabel;
        private System.Windows.Forms.TextBox passwordRepeat;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button register;
    }
}