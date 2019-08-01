using CefSharp.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace web2win.Plugins
{
    interface IPlugin
    {
        void Configuration(Config config);

        bool Enabled { get; }

        T GetComponent<T>()
            where T : class;

        void OnApplicationExit(object sender, ExitEventArgs e);

        void OnWindowLoad(Window window, ChromiumWebBrowser browser);

    }
}
