using CefSharp;
using CefSharp.Handler;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace web2win.Plugins
{
    class UrlFilterPlugin : PluginBase
    {
        public Regex Filter { get; private set; }

        public override void Configuration(Config config)
        {
            if (config.UrlFilter == null)
            {
                return;
            }
            Filter = new Regex(config.UrlFilter, RegexOptions.IgnoreCase);
            Enabled = true;
        }

        public void OnBeforeBrowse(PluginEventArgs args)
        {
            var request = args.Get<IRequest>();
            if (!string.IsNullOrEmpty(request.Url) && request.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                args.Result =!Filter.IsMatch(request.Url);
            }
        }
    }
}
