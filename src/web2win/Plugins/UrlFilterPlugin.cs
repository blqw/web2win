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
    class UrlFilterPlugin : DefaultRequestHandler, IPlugin
    {
        public Regex Filter { get; private set; }

        public void Configuration(Config config)
        {
            if (config.UrlFilter == null)
            {
                return;
            }
            Filter = new Regex(config.UrlFilter, RegexOptions.IgnoreCase);
            Enabled = true;
        }

        public bool Enabled { get; protected set; }

        public T GetFeature<T>() where T : class => Enabled ? this as T : null;

        public void OnWindowLoad(Window window, ChromiumWebBrowser browser) { }

        public void Dispose() { }


        public override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if (!string.IsNullOrEmpty(request.Url) && request.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return !Filter.IsMatch(request.Url);
            }
            return false;
        }
    }
}
