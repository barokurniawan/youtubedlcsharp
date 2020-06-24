
namespace YoutubeDownloader.Entity
{
    class YoutubeFormats
    {
        public string ext;
        public int filesize;
        public string acodec;
        public string vcodec;
        public string url;
        public string filename;

        public YoutubeFormats(string ext = null, int filesize = 0, string acodec = null, string vcodec = null, string url = null, string filename = null)
        {
            this.SetExt(ext);
            this.SetFilesize(filesize);
            this.SetAcodec(acodec);
            this.SetVcodec(vcodec);
            this.SetUrl(url);
            this.SetFilename(filename);
        }

        public void SetFilename(string f)
        {
            this.filename = f;
        }

        public string GetFilename()
        {
            return this.filename;
        }

        public void SetUrl(string url)
        {
            this.url = url;
        }

        public void SetAcodec(string acodec)
        {
            this.acodec = acodec;
        }
        public void SetVcodec(string vcodec)
        {
            this.vcodec = vcodec;
        }

        public void SetFilesize(int filesize)
        {
            this.filesize = filesize;
        }

        public void SetExt(string ext)
        {
            this.ext = ext;
        }

        public string getUrl()
        {
            return this.url;
        }

        public string GetAcodec()
        {
            return this.acodec;
        }

        public string GetVcodec()
        {
            return this.vcodec;
        }

        public int GetFilesize()
        {
            return this.filesize;
        }

        public string GetExt()
        {
            return this.ext;
        }
    }
}
