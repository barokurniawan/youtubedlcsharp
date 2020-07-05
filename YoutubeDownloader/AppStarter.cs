using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeDownloader.Src.Constant;

namespace YoutubeDownloader
{
    public partial class AppStarter : Form
    {
        private string ExecutablePath;

        public AppStarter()
        {
            InitializeComponent();
            string resourceDir = Path.Combine(Environment.CurrentDirectory, ConstantApp.RESOURCE_DIR);
            if (!Directory.Exists(resourceDir))
            {
                Directory.CreateDirectory(resourceDir);
            }

            ExecutablePath = Path.Combine(resourceDir, ConstantApp.EXECUTE_FILE);
        }

        private void AppStarter_Load(object sender, EventArgs e)
        {
            using (WebClient cl = new WebClient())
            {
                cl.DownloadProgressChanged += DownloadProgressChange;
                cl.DownloadFileCompleted += new AsyncCompletedEventHandler(OnCompleteDownload);
                cl.DownloadFileAsync(
                    new System.Uri(ConstantApp.TYDL),
                    ExecutablePath
                );
            }
        }

        void OnCompleteDownload(Object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Download complete", "Your download has been complete.");
            Close();
        }

        private void DownloadProgressChange(Object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}
