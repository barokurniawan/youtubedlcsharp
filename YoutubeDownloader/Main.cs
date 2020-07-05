using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms;
using YoutubeDownloader.Src.Constant;
using YoutubeDownloader.Src.Entity;

namespace YoutubeDownloader
{
    public partial class Main : Form
    {
        private DataTable dataTable;
        private List<YoutubeFormat> FormatBucket;
        private string ExecutablePath;
        private string DownloadPath;

        public Main()
        {
            string resourceDir = Path.Combine(Environment.CurrentDirectory, ConstantApp.RESOURCE_DIR);
            if (!Directory.Exists(resourceDir))
            {
                Directory.CreateDirectory(resourceDir);
            }

            ExecutablePath = Path.Combine(resourceDir, ConstantApp.EXECUTE_FILE);
            if (!File.Exists(ExecutablePath))
            {
                (new AppStarter()).ShowDialog();
            }

            DownloadPath = Path.Combine(Environment.CurrentDirectory, ConstantApp.DOWNLOAD_DIR);
            if (!Directory.Exists(DownloadPath))
            {
                Directory.CreateDirectory(DownloadPath);
            }

            InitializeComponent();
            btnDownload.Enabled = false;
            this.dataTable = new DataTable();
            this.FormatBucket = new List<YoutubeFormat>();

            DrawDatatable();
        }

        public void DrawDatatable(dynamic yt = null)
        {
            this.dataTable.Columns.Clear();
            this.dataTable.Rows.Clear();

            DataColumn[] columns = {
                new DataColumn("Ext"),
                new DataColumn("Size (Kb)"),
                new DataColumn("Audio Codec"),
                new DataColumn("Video Codec")
            };

            this.dataTable.Columns.AddRange(columns);

            if (yt!= null)
            {
                this.FormatBucket.Clear();
                foreach (var item in yt.formats)
                {
                    var filesize = (item.filesize == null) ? 0 : item.filesize;
                    var vcodec = (item.vcodec == null) ? 0 : item.vcodec;
                    var acodec = (item.acodec == null) ? 0 : item.acodec;

                    YoutubeFormat f = new YoutubeFormat();
                    f.SetExt(Convert.ToString(item.ext));
                    f.SetFilesize((int) filesize);
                    f.SetAcodec(Convert.ToString(acodec));
                    f.SetVcodec(Convert.ToString(vcodec));
                    f.SetUrl(Convert.ToString(item.url));
                    f.SetFilename(Convert.ToString(yt.fulltitle));

                    this.FormatBucket.Add(f);
                    Object[] c = { item.ext, ((int) filesize / 1024), item.acodec, item.vcodec };
                    this.dataTable.Rows.Add(c);
                }
            }

            tableResult.DataSource = this.dataTable;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            btnExecute.Enabled = false;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = ExecutablePath;
            process.StartInfo.Arguments = "--dump-json " + txtYoutubeURL.Text;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            //* Read the output (or the error)
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            dynamic yt = Newtonsoft.Json.JsonConvert.DeserializeObject(output);

            txtTitle.Text = yt.fulltitle;
            DrawDatatable(yt);

            btnDownload.Enabled = true;
            btnExecute.Enabled = true;
            process.WaitForExit();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            btnDownload.Enabled = false;
            YoutubeFormat[] items = this.FormatBucket.ToArray();
            YoutubeFormat item = (YoutubeFormat) items.GetValue(tableResult.CurrentCell.RowIndex);

            using(WebClient cl = new WebClient())
            {
                cl.DownloadProgressChanged += DownloadProgressChange;
                cl.DownloadFileCompleted += new AsyncCompletedEventHandler(OnCompleteDownload);
                cl.DownloadFileAsync(
                    new System.Uri(item.getUrl()),
                    Path.Combine(DownloadPath, item.GetFilename() + "." + item.GetExt())
                );
            }
        }

        void OnCompleteDownload(Object sender, AsyncCompletedEventArgs e)
        {
            btnDownload.Enabled = true;
            DownloadProgress.Value = 0;
            MessageBox.Show("Download complete", "Your download has been complete.");
        }

        private void DownloadProgressChange(Object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = e.ProgressPercentage;
        }
    }
}
