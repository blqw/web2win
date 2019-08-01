using CefSharp.Wpf;
using System.Windows;

namespace web2win.Plugins
{
    abstract class PluginBase : IPlugin
    {
        public abstract void Configuration(Config config);

        public virtual bool Enabled { get; protected set; }

        public virtual T GetComponent<T>() where T : class
            => Enabled ? this as T : null;

        public virtual void OnApplicationExit(object sender, ExitEventArgs e) { }
        public virtual void OnWindowLoad(Window window, ChromiumWebBrowser browser) { }
    }
}
