using CefSharp;
using CefSharp.Wpf;
using System;
using System.IO;
using System.Linq;
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
                    //window.Icon = new BitmapImage(new Uri(new Uri(config.Url), "/favicon.ico"));
                    browser.FrameLoadEnd += async (_, x) =>
                    {
                        if (x.Frame.IsMain)
                        {
                            var res = await x.Frame.EvaluateScriptAsync("(function(){return (document.querySelector(\"link[rel = 'shortcut icon']\") || document.querySelector(\"link[rel = 'icon']\") || {}).href;})()");
                            if (res.Success && res.Result is string s && !string.IsNullOrWhiteSpace(s))
                            {
                                window.Dispatcher?.Invoke(() => window.Icon = new BitmapImage(new Uri(s)));
                            }
                        }
                    };

                    if (browser.IsLoaded)
                    {
                        var res = browser.EvaluateScriptAsync("(function(){return (document.querySelector(\"link[rel = 'shortcut icon']\") || document.querySelector(\"link[rel = 'icon']\") || {}).href;})()").Result;
                        if (res.Success && res.Result is string s && !string.IsNullOrWhiteSpace(s))
                        {
                            window.Dispatcher?.Invoke(() => window.Icon = new BitmapImage(new Uri(s)));
                        }
                    }
                }

                if (config.Title == null)
                {
                    window.SetBinding(Window.TitleProperty, new Binding("Title") { Source = browser });
                }
            });


            var cookieManager = browser.GetCookieManager();
            var visitor = new CookieVisitor();
            visitor.Callback(value =>
            {
                var array = value.Split(',').Select(double.Parse).ToArray();
                window.Dispatcher?.Invoke(() =>
                {
                    window.Left = array[0];
                    window.Top = array[1];
                    window.Width = array[2];
                    window.Height = array[3];
                });
            });


            cookieManager.VisitUrlCookies("http://location.com", true, visitor);

            if (Config.SaveExitedLocation)
            {
                window.Closing += Window_Closing;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var window = (Window)sender;
                var cookieManager = Cef.GetGlobalCookieManager();
                cookieManager.SetCookie("http://location.com", new Cookie()
                {
                    Domain = "location.com",
                    Name = "location",
                    Value = $"{window.Left},{window.Top},{window.Width},{window.Height}",
                    Expires = DateTime.MinValue,
                    HttpOnly = false,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public class CookieVisitor : ICookieVisitor
        {
            private Action<string> _action;

            public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
            {
                if (cookie.Name == "location")
                {
                    _action(cookie.Value);
                    return false;
                }
                return true;
            }

            public void Dispose() { }

            public void Callback(Action<string> action) => _action = action;
        }

    }
}