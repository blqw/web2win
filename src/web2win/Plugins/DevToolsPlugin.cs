using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;

namespace web2win.Plugins
{
    class DevToolsPlugin : PluginBase
    {
        public override void Configuration(Config config) => Enabled = config.EnableF12;

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser) 
            => window.KeyDown += (_, e) =>
            {
                if (e.Key == Key.F12)
                {
                    browser.ShowDevTools();
                    e.Handled = true;
                }
            };
    }
}
