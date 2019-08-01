using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace web2win.Plugins
{
    class DisabledBackPlugin : PluginBase
    {
        public override void Configuration(Config config) => Enabled = config.DisableHistory;

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser) => browser.FrameLoadStart += Browser_FrameLoadStart;
        private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
            => ((ChromiumWebBrowser)sender).ExecuteScriptAsync(@"
history.pushState(null, null, document.URL);
window.addEventListener('popstate', function () {
    history.pushState(null, null, document.URL);
});
");
    }
}
