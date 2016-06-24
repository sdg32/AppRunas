using System;
using System.Windows.Forms;

namespace AppRunas
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != "")
            {
                Program.SetPassword(txtPassword.Text);
                this.Close();
            }
        }
    }
}
