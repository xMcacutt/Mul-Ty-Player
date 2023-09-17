using PropertyChanged;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SetupViewModel
    {
        public string TyPath { get; set; }
        public string MTPPath { get; set; }

        public string[] TyFileNames = { "Data_PC.rkv", "Music_PC.rkv", "Override_PC.rkv", "Video_PC.rkv", "OpenAL32.dll", "soft_oal.dll", "steam_api.dll", "steam_appid.txt", "TY.exe"};

        public string ProgressText { get; set; }
        public int ProgressPercentage { get; set; }

        BackgroundWorker worker;

        public SetupViewModel()
        {
            string path = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "/Steam/Steamapps/Common/Ty the Tasmanian Tiger/";
            if (Path.Exists(path)) TyPath = path;
            else TyPath = null;
        }


        public void Install()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += InstallWorker_DoWork;
            worker.ProgressChanged += InstallWorker_ProgressChanged;
            worker.RunWorkerCompleted += InstallWorker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void InstallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Directory.GetFiles(MTPPath).Length == 0)
            {
                long totalBytes;
                foreach (string file in TyFileNames)
                {
                    var sourceFilePath = Path.Combine(TyPath, file);
                    var destinationFilePath = Path.Combine(MTPPath, file);
                    if(file == "TY.exe") destinationFilePath = Path.Combine(MTPPath, "Mul-Ty-Player.exe");
                    var fileInfo = new FileInfo(sourceFilePath);
                    totalBytes = fileInfo.Length;

                    using var sourceStream = File.OpenRead(sourceFilePath);
                    using var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);

                    var buffer = new byte[81920];
                    int bytesRead;
                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destinationStream.Write(buffer, 0, bytesRead);

                        var bytesWritten = destinationStream.Position;
                        var progressPercentage = (double)bytesWritten / totalBytes * 100;
                        worker.ReportProgress((int)progressPercentage, $"Copying file {file}: {(int)progressPercentage}%");
                    }
                }
            }
            string url = "https://drive.google.com/u/0/uc?id=1OTudAOfutB458mm2qKqRlX1Hi73h8WQw&export=download&confirm";
            HttpClient client = new();
            if (!string.IsNullOrEmpty(url))
            {
                Uri uri = new Uri(url);
                HttpResponseMessage response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
                response.EnsureSuccessStatusCode();
                using Stream contentStream = response.Content.ReadAsStreamAsync().Result;
                var totalBytes = response.Content.Headers.ContentLength;
                var destinationPath = Path.Combine(MTPPath, "Patch_PC.rkv");
                using FileStream fileStream = new(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);

                var buffer = new byte[4096];
                var bytesRead = default(int);
                var bytesWritten = 0L;
                do
                {
                    bytesRead = contentStream.ReadAsync(buffer, 0, buffer.Length).Result;
                    fileStream.WriteAsync(buffer, 0, bytesRead).Wait();
                    bytesWritten += bytesRead;

                    if (totalBytes.HasValue)
                    {
                        var progressPercentage = bytesWritten * 100d / totalBytes.Value;
                        worker.ReportProgress((int)progressPercentage, $"Downloading Patch: {(int)progressPercentage}%");
                    }
                } while (bytesRead > 0);
                e.Result = "Mul-Ty-Player was successfully installed.";
            }
        }

        private void InstallWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
            ProgressText = e.UserState.ToString();
        }

        private void InstallWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Handle the error.
            }
            else if (e.Cancelled)
            {
                // Handle the cancellation.
            }
            else
            {
                ProgressText = e.Result.ToString();
            }
        }
    }
}
