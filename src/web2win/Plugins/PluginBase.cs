using CefSharp.Wpf;
using System.Windows;

namespace web2win.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        public abstract void Configuration(Config config);

        public virtual bool Enabled { get; protected set; }

        public virtual T GetFeature<T>() where T : class => Enabled ? this as T : null;

        public virtual void OnWindowLoad(Window window, ChromiumWebBrowser browser) { }

        public virtual void Dispose() { }
    }
}
