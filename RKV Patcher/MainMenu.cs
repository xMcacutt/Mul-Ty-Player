using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Net;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace RKV_Patcher
{
    public partial class MainMenu : Form
    {
        string[] files;
        WebClient client;

        public MainMenu()
        {
            InitializeComponent();
            files = new string[]
            {
                "Music_PC.rkv",
                "Video_PC.rkv",
                "Override_PC.rkv",
                "steam_api.dll",
                "steam_appid.txt",
                "OpenAL32.dll",
                "soft_oal.dll",
                "TY.exe"
            };
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            //Run download file with multiple thread
            string url = "https://drive.google.com/u/0/uc?id=1OTudAOfutB458mm2qKqRlX1Hi73h8WQw&export=download&confirm";
            if (!string.IsNullOrEmpty(url))
            {
                Thread thread = new Thread(() =>
                {
                    Uri uri = new Uri(url);
                    client.DownloadFileAsync(uri, Application.StartupPath + "/" + "Data_PC.rkv");
                });
                thread.Start();
            }
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string baseFolder = SelectTyFolder.SelectedPath;
            DirectoryInfo info = new DirectoryInfo(baseFolder);
            Directory.CreateDirectory(info.Parent.FullName + "/Mul-Ty-Player");
            string newFolder = info.Parent.FullName;
            foreach(string s in files)
            {
                System.IO.File.Copy(info.FullName + "/" + s, newFolder + "/Mul-Ty-Player/" + s);
            }
            System.IO.File.Move("Data_PC.rkv", newFolder + "/Mul-Ty-Player/Data_PC.rkv");
            MessageBox.Show("Mul-Ty-Player has been created next to Ty folder !", "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Update progress bar & label
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                double receive = double.Parse(e.BytesReceived.ToString());
                double total = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = receive / total * 100;
                progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            }));
        }

        private void SelectFolder_Click(object sender, EventArgs e)
        {
            SelectTyFolder.ShowDialog();
            foreach(string s in files)
            {
                if(!System.IO.File.Exists(SelectTyFolder.SelectedPath + "/" + s))
                {
                    message.Text = "One or more files are missing. Check Ty folder and try again.";
                    return;
                }
            }
            message.Text = "";
            SelectFolder.Enabled = false;
            DownloadButton.Enabled = true;
        }
    }
}
