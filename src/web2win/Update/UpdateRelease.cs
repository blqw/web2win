using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace web2win.Update
{
    public class UpdateRelease
    {
        private string _created_at;

        public int id { get; set; }
        public string tag_name { get; set; }
        public string name { get; set; }
        public string body { get; set; }
        public bool prerelease { get; set; }
        public string created_at
        {
            get => _created_at;
            set => _created_at = value?.Replace('Z', ' ').Replace('T', ' ');
        }
        public UpdateAsset[] assets { get; set; }
    }

    public class UpdateAsset
    {
        public int id { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public int size { get; set; }
        public int download_count { get; set; }
        public string browser_download_url { get; set; }

        private static readonly char[] _prefix = { 'v', 'V', '.', ' ' };
        public Version MinVersion
        {
            get
            {
                var arr = name.Split('-');
                if (arr.Length == 2)
                {
                    return Version.TryParse(arr[0].TrimStart(_prefix), out var version) ? version : null;
                }
                else if (arr.Length == 3)
                {
                    return Version.TryParse(arr[1].TrimStart(_prefix), out var version) ? version : null;
                }
                return null;
            }
        }

        public Version Version
        {
            get
            {
                var arr = name.Split('-');
                if (arr.Length >= 2)
                {
                    return Version.TryParse(arr[0].TrimStart(_prefix), out var version) ? version : null;
                }
                return null;
            }
        }

        public bool IsFull => name.Contains("-full.");

        public bool IsUpdate => name.Contains("-update.");

        public string SizeText
        {
            get
            {
                if (size < 1024)
                {
                    return 1024 + "B";
                }
                var fsize = size / 1024f;
                if (fsize < 1024f)
                {
                    return fsize.ToString("f1") + "KB";
                }
                fsize /= 1024f;
                if (fsize < 1024f)
                {
                    return fsize.ToString("f1") + "MB";
                }
                fsize /= 1024f;
                if (fsize < 1024f)
                {
                    return fsize.ToString("f1") + "GB";
                }
                fsize /= 1024f;
                return fsize.ToString("f1") + "TB";
            }
        }
    }
}
