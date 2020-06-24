using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Windows.Forms;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        private List<Entity.YoutubeFormats> FormatBucket;

        public Form1()
        {
            InitializeComponent();
            btnDownload.Enabled = false;
            this.dataTable = new DataTable();

            this.FormatBucket = new List<Entity.YoutubeFormats>();
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
                    Entity.YoutubeFormats f = new Entity.YoutubeFormats();
                    f.SetExt(Convert.ToString(item.ext));
                    f.SetFilesize((int) item.filesize);
                    f.SetAcodec(Convert.ToString(item.acodec));
                    f.SetVcodec(Convert.ToString(item.vcodec));
                    f.SetUrl(Convert.ToString(item.url));
                    f.SetFilename(Convert.ToString(yt.fulltitle));

                    this.FormatBucket.Add(f);
                    Object[] c = { item.ext, ((int)item.filesize / 1024), item.acodec, item.vcodec };
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
            process.StartInfo.FileName = "youtube-dl.exe";
            process.StartInfo.Arguments = "--dump-json " + txtYoutubeURL.Text;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            //* Read the output (or the error)
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            Console.WriteLine(err);
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
            Entity.YoutubeFormats[] items = this.FormatBucket.ToArray();
            Entity.YoutubeFormats item = (Entity.YoutubeFormats) items.GetValue(tableResult.CurrentCell.RowIndex);

            using(WebClient cl = new WebClient())
            {
                cl.DownloadProgressChanged += DownloadProgressChange;
                cl.DownloadFileCompleted += new AsyncCompletedEventHandler(OnCompleteDownload);
                cl.DownloadFileAsync(
                    new System.Uri(item.getUrl()),
                    item.GetFilename() +"." + item.GetExt()
                );
            }
        }

        void OnCompleteDownload(Object sender, AsyncCompletedEventArgs e)
        {
            btnDownload.Enabled = true;
            DownloadProgress.Equals(0);
            MessageBox.Show("Download complete", "Your download has been complete.");
        }

        private void DownloadProgressChange(Object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = e.ProgressPercentage;
        }

    }
}
