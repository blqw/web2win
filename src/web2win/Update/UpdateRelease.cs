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
        public UpdateAssets[] assets { get; set; }
    }

    public class UpdateAssets
    {
        public int id { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public int download_count { get; set; }
        public string browser_download_url { get; set; }
    }
}
