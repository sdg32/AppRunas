using System;
using System.Windows.Forms;

namespace AppRunas
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private string GetSelectedAppName()
        {
            DataGridViewRow row = this.dvApps.CurrentRow;
            if (row != null)
            {
                return row.Cells["name"].Value.ToString();
            }
            return null;
        }

        private void RefreshData()
        {
            this.dvApps.Rows.Clear();
            foreach (Program.App app in Program.GetApps())
            {
                int idx = this.dvApps.Rows.Add();
                this.dvApps.Rows[idx].Cells["name"].Value = app.name;
                this.dvApps.Rows[idx].Cells["path"].Value = app.path;
                this.dvApps.Rows[idx].Cells["username"].Value = app.username;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AppForm form = new AppForm(null);
            form.ShowDialog();
            RefreshData();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string name = GetSelectedAppName();
            if (name != null)
            {
                string msg = string.Format("确定要运行任务：{0} ({1})", name, this.dvApps.CurrentRow.Cells["path"].Value.ToString());
                if (Program.MsgBox(msg, "yesno", "info") == DialogResult.Yes)
                {
                    Program.RunApp(name);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string name = GetSelectedAppName();
            if (name != null)
            {
                AppForm form = new AppForm(name);
                form.ShowDialog();
                RefreshData();
            }
        }
        
        private void btnRemove_Click(object sender, EventArgs e)
        {
            string name = GetSelectedAppName();
            if (name != null)
            {
                string msg = string.Format("确定要删除任务：{0} ({1})", name, this.dvApps.CurrentRow.Cells["path"].Value.ToString());
                if (Program.MsgBox(msg, "yesno", "warning") == DialogResult.Yes)
                {
                    Program.RemoveApp(name);
                    RefreshData();
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            if (Program.MsgBox("确定要删除所有任务吗？", "yesno", "warning") == DialogResult.Yes)
            {
                foreach (Program.App app in Program.GetApps())
                {
                    Program.RemoveApp(app.name);
                }
                RefreshData();
            }
        }

        private void btnChangePwd_Click(object sender, EventArgs e)
        {
            ChangePwdForm form = new ChangePwdForm();
            form.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // DataGridView paint
            this.dvApps.Columns.Add("name", "名称");
            this.dvApps.Columns.Add("path", "程序");
            this.dvApps.Columns.Add("username", "用户名");
            this.dvApps.Columns["path"].Width = 250;
            this.dvApps.Columns["username"].Width = 150;

            if (Program.IsNeedRegister())
            {
                RegisterForm form = new RegisterForm();
                form.ShowDialog();
                if (Program.IsNeedRegister())
                {
                    System.Environment.Exit(0);
                }
            } else
            {
                LoginForm form = new LoginForm();
                if (form.ShowDialog() != DialogResult.OK)
                {
                    System.Environment.Exit(0);
                }
            }

            // Refresh data
            RefreshData();
        }
    }
}
