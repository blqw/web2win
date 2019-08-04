using CefSharp.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace web2win.Plugins
{
    class LaunchPlugin : PluginBase
    {
        public Config Config { get; private set; }

        public override void Configuration(Config config) => Enabled = (Config = config) != null;

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser)
        {
            var screeHeight = SystemParameters.FullPrimaryScreenHeight;
            var screeWidth = SystemParameters.FullPrimaryScreenWidth;
            var config = Config;
            window.Dispatcher?.Invoke(() =>
            {
                window.Title = config.Title ?? "web2win";
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.Topmost = config.Topmost;
                window.Height = config.Height ?? 450;
                window.Width = config.Width ?? 750;
                window.Top = config.Top ?? (screeHeight - window.Height) / 2;
                window.Left = config.Left ?? (screeWidth - window.Width) / 2;
                window.MaxHeight = config.MaxHeight ?? window.MaxHeight;
                window.MaxWidth = config.MaxWidth ?? window.MaxWidth;
                window.MinWidth = config.MinWidth ?? window.MinWidth;
                window.MinHeight = config.MinHeight ?? window.MinHeight;
                if (config.Icon != null && File.Exists(config.Icon))
                {
                    try
                    {
                        window.Icon = new BitmapImage(new Uri(config.Url));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                if (window.Icon == null)
                {
                    window.Icon = new BitmapImage(new Uri(new Uri(config.Url), "/favicon.ico"));
                }

                if (config.Title == null)
                {
                    window.SetBinding(Window.TitleProperty, new Binding("Title") { Source = browser });
                }
            });

        }
    }
}