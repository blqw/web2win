using CefSharp.Wpf;
using System.Windows;

namespace web2win.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        public abstract void Configuration(Config config);

        public virtual bool Enabled { get; protected set; }

        public virtual void OnWindowLoad(Window window, ChromiumWebBrowser browser) { }

        public virtual void Dispose() { }
    }
}
