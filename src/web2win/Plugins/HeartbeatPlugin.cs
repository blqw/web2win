using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace web2win.Plugins
{
    class HeartbeatPlugin : PluginBase
    {
        public string Url { get; private set; }
        public int Interval { get; private set; }

        public override void Configuration(Config config)
        {
            Url = config.HeartbeatUrl;
            Interval = config.HeartbeatInterval ?? 60000;
            Enabled = Url != null;
        }

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser)
        {
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private void Browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                e.Frame.ExecuteJavaScriptAsync($@"
setInterval(()=>{{
    var iframe = document.createElement('iframe'); 
    iframe.src='{Url}';  
    iframe.style='display:none';  
    iframe.onload = ()=> document.body.removeChild(iframe);
    document.body.appendChild(iframe);
}}, {Interval})");
            }
        }
    }
}
