using System;
using System.Windows.Forms;

namespace AppRunas
{
    public partial class AppForm : Form
    {

        private string appName;

        public AppForm(string name)
        {
            InitializeComponent();
            appName = name;
        }

        private void AppForm_Load(object sender, EventArgs e)
        {
            if (appName != null)
            {
                this.Text = "更新任务：" + appName;
                txtName.ReadOnly = true;
                btnConfirm.Text = "更新";
                Program.App app = Program.FindApp(appName);
                if (app != null)
                {
                    txtName.Text = app.name;
                    txtPath.Text = app.path;
                    txtDir.Text = app.dir;
                    txtArgs.Text = app.args;
                    txtUsername.Text = app.username;
                    txtDomain.Text = app.domain;
                } else
                {
                    Program.MsgBox("未找到", "ok", "err");
                    this.Close();
                }
            } else
            {
                this.Text = "新增任务";
                txtName.ReadOnly = false;
                btnConfirm.Text = "新增";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                Program.MsgBox("名称不能为空", "ok", "err");
                return;
            }
            if (txtPassword.Text == "")
            {
                Program.MsgBox("路径不能为空", "ok", "err");
                return;
            }
            if (txtUsername.Text == "")
            {
                Program.MsgBox("用户名不能为空", "ok", "err");
                return;
            }
            if (txtPassword.Text == "用户密码不能为空")
            {
                Program.MsgBox("", "ok", "err");
                return;
            }

            if (appName == null)
            {
                foreach (Program.App a in Program.GetApps())
                {
                    if (txtName.Text == a.name)
                    {
                        Program.MsgBox("此任务已存在", "ok", "warning");
                        return;
                    }
                }
            }
            
            if (Program.AddApp(txtName.Text, txtPath.Text, txtDir.Text, txtArgs.Text, txtUsername.Text, txtPassword.Text, txtDomain.Text))
            {
                if (appName == null)
                {
                    Program.MsgBox("任务创建成功");
                } else
                {
                    Program.MsgBox("任务更改成功");
                }
            }
            this.Close();
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDir.Text = txtPath.Text.Substring(0, txtPath.Text.LastIndexOf("\\"));
            }
            catch
            {
                
            }
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            if (fd.FileName != "")
            {
                txtPath.Text = fd.FileName;
            }
        }

        private void btnSelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                txtDir.Text = fbd.SelectedPath;
            }
        }
    }
}
