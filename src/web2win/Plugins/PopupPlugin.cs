using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web2win.Plugins
{
    class PopupPlugin : PluginBase
    {
        string _target;
        public override void Configuration(Config config)
            => Enabled = new[] { "_self", "_top" }.Contains(_target = config.PopupPageTarget);

        public void OnBeforePopup(PluginEventArgs args)
        {
            var targetUrl = args.Get<string>("targetUrl");

            switch (_target)
            {
                case "_self":
                    var frame = args.Get<IFrame>("frame");
                    frame.LoadUrl(targetUrl);
                    args.Result = true;
                    break;
                case "_top":
                    var browser = args.Get<IWebBrowser>("chromiumWebBrowser");
                    browser.Load(targetUrl);
                    args.Result = true;
                    break;
                default:
                    args.Result = false;
                    break;
            }

        }
    }
}
