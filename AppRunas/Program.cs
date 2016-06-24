using System;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AppRunas
{
    static class Program
    {

        private const string ENCRYPT_SALT_KEY = "UK9ei";
        private const string ENCRYPT_IV = "3gbkpH9MOoSeXD1u";

        private static string configPath = Environment.CurrentDirectory + "/config.xml";
        private static string userId = Environment.MachineName + "|" + Environment.UserDomainName + "|" + Environment.UserName;

        private static XmlDocument doc = LoadConfig();
        private static XmlElement user;

        public class App
        {
            public string name, path, dir, args, username, password, domain;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                RunApp(args[0]);
                Application.Exit();
            }
            Application.Run(new MainForm());
        }

        public static bool IsNeedRegister()
        {
            return user.GetAttribute("password") == "";
        }

        public static void RunApp(string name)
        {
            App app = FindApp(name);
            if (app == null)
            {
                return;
            }
            try
            {
                Process p = new Process();
                ProcessStartInfo info = new ProcessStartInfo();

                info.UserName = app.username;
                info.Domain = app.domain;
                SecureString password = new SecureString();
                foreach (char c in app.password.ToCharArray())
                {
                    password.AppendChar(c);
                }
                info.Password = password;

                info.FileName = app.path;
                info.Arguments = app.args;
                if (app.dir == "")
                {
                    info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                } else
                {
                    info.WorkingDirectory = app.dir;
                }

                info.UseShellExecute = false;
                info.LoadUserProfile = true;
                p.StartInfo = info;
                p.Start();
            }
            catch (Exception err)
            {
                MsgBox(err.Message, "ok", "err");
            }
        }

        public static bool SetPassword(string password)
        {
            user.SetAttribute("password", Encrypt(password));
            doc.Save(configPath);
            return true;
        }

        public static bool VerifyPassword(string password)
        {
            if (user == null)
            {
                return false;
            }
            return Decrypt(user.GetAttribute("password")) == password;
        }

        public static bool AddApp(string name, string path, string dir, string args, string username, string password, string domain)
        {
            XmlElement appXML = FindAppXML(name);
            if (appXML == null)
            {
                appXML = doc.CreateElement("app");
                user.AppendChild(appXML);
            }
            appXML.SetAttribute("name", Encrypt(name));
            appXML.SetAttribute("path", Encrypt(path));
            appXML.SetAttribute("args", Encrypt(args));
            appXML.SetAttribute("dir", Encrypt(dir));
            appXML.SetAttribute("username", Encrypt(username));
            appXML.SetAttribute("password", Encrypt(password));
            appXML.SetAttribute("domain", Encrypt(domain));
            doc.Save(configPath);
            return true;
        }

        public static bool RemoveApp(string name)
        {
            XmlElement appXML = FindAppXML(name);
            if (appXML != null)
            {
                user.RemoveChild(appXML);
                doc.Save(configPath);
            }
            return true;
        }

        public static App FindApp(string name)
        {
            XmlElement appXML = FindAppXML(name);
            App app = new App();
            app.name = Decrypt(appXML.GetAttribute("name"));
            app.path = Decrypt(appXML.GetAttribute("path"));
            app.args = Decrypt(appXML.GetAttribute("args"));
            app.dir = Decrypt(appXML.GetAttribute("dir"));
            app.username = Decrypt(appXML.GetAttribute("username"));
            app.password = Decrypt(appXML.GetAttribute("password"));
            app.domain = Decrypt(appXML.GetAttribute("domain"));
            return app;
        }

        public static App[] GetApps()
        {
            XmlNodeList appsXML = user.SelectNodes("app");
            App[] apps = new App[appsXML.Count];
            int i = 0;
            foreach (XmlElement appXML in appsXML)
            {
                App app = new App();
                app.name = Decrypt(appXML.GetAttribute("name"));
                app.path = Decrypt(appXML.GetAttribute("path"));
                app.args = Decrypt(appXML.GetAttribute("args"));
                app.dir = Decrypt(appXML.GetAttribute("dir"));
                app.username = Decrypt(appXML.GetAttribute("username"));
                app.password = Decrypt(appXML.GetAttribute("password"));
                app.domain = Decrypt(appXML.GetAttribute("domain"));

                apps[i] = app;
                i++;
            }
            return apps;
        }

        public static XmlElement FindAppXML(string name)
        {
            string query = string.Format("app[@name='{0}']", Encrypt(name));
            return (XmlElement)user.SelectSingleNode(query);
        }

        private static XmlDocument LoadConfig()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(configPath);
            }
            catch
            {
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                doc.AppendChild(doc.CreateElement("users"));
                doc.Save(configPath);
            }
            string query = string.Format("users/user[@id='{0}']", userId);
            user = (XmlElement)doc.SelectSingleNode(query);
            if (user == null)
            {
                XmlElement users = (XmlElement)doc.SelectSingleNode("users");
                user = doc.CreateElement("user");
                user.SetAttribute("id", userId);
                users.AppendChild(user);
                doc.Save(configPath);
            }
            return doc;
        }

        private static string Encrypt(string text)
        {
            byte[] key = Encoding.UTF8.GetBytes(getEncryptKey());
            byte[] data = Encoding.UTF8.GetBytes(text);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.IV = Encoding.UTF8.GetBytes(ENCRYPT_IV);
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            try
            {
                byte[] rv = cTransform.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(rv, 0, rv.Length);
            }
            catch
            {
                return "";
            }
        }

        private static string Decrypt(string text)
        {
            byte[] key = Encoding.UTF8.GetBytes(getEncryptKey());
            byte[] data = Convert.FromBase64String(text);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.IV = Encoding.UTF8.GetBytes(ENCRYPT_IV);
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            try
            {
                byte[] rv = cTransform.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(rv);
            }
            catch
            {
                return "";
            }
        }

        private static string getEncryptKey()
        {
            string key = ENCRYPT_SALT_KEY + Environment.UserName + Environment.MachineName;
            while (key.Length < 32)
            {
                key += key;
            }
            return key.Substring(0, 32);
        }

        public static DialogResult MsgBox(string msg, string btn="ok", string icon="info")
        {
            MessageBoxButtons msgBtn;
            if (btn == "yesno")
            {
                msgBtn = MessageBoxButtons.YesNo;
            } else {
                msgBtn = MessageBoxButtons.OK;
            }

            MessageBoxIcon msgIcon;
            if (icon == "err")
            {
                msgIcon = MessageBoxIcon.Error;
            } else if (icon == "warning")
            {
                msgIcon = MessageBoxIcon.Exclamation;
            } else
            {
                msgIcon = MessageBoxIcon.Information;
            }

            return MessageBox.Show(msg, "AppRunas", msgBtn, msgIcon);
        }
    }
}
