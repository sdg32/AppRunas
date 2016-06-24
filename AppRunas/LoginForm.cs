using System;
using System.Windows.Forms;

namespace AppRunas
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != "")
            {
                if (Program.VerifyPassword(txtPassword.Text))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                } else
                {
                    Program.MsgBox("密码错误", "ok", "warning");
                }
            }
        }
    }
}
