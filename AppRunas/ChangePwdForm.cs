using System;
using System.Windows.Forms;

namespace AppRunas
{
    public partial class ChangePwdForm : Form
    {
        public ChangePwdForm()
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
