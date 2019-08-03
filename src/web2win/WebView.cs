using CefSharp;
using CefSharp.Wpf;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using web2win.Plugins;

namespace web2win
{
    class WebView
    {
        public static IDisposable UseCef()
        {
            if (Cef.IsInitialized)
            {
                return null;
            }
            var scan = Task.Run(PlugInManager.Scan); //异步扫描
            var settings = CreateCefSettings();
            SupportAnyCpu(settings);
            Cef.Initialize(settings);
            scan.Wait();
            Cef.AddDisposable(scan.Result);
            return new Disposable();
        }

        private class Disposable : IDisposable
        {
            public void Dispose() => Cef.Shutdown();
        }

        private static CefSettings CreateCefSettings()
        {
            var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var config = Path.GetFileNameWithoutExtension(StartupCommands.Config);
            var settings = new CefSettings
            {
                CachePath = Path.Combine(path, "Browser", config, "Cache"),
                LogFile = Path.Combine(path, "Browser", config, "Logs"),
                UserDataPath = Path.Combine(path, "Browser", config, "UserData"),
                AcceptLanguageList = "zh,zh-cn,zh-tw;q=0.9",
                PersistSessionCookies = true,
                Locale = "zh-CN",
                LogSeverity = LogSeverity.Disable,

            };
            return settings;
        }

        private static void SupportAnyCpu(AbstractCefSettings settings)
        {
            var basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            const string exe = "CefSharp.BrowserSubprocess.exe";
            if (!File.Exists(Path.Combine(basePath, exe)))
            {
                var perform = Environment.Is64BitProcess ? "x64" : "x86";
                settings.BrowserSubprocessPath = Path.Combine(basePath, perform, exe);
            }
        }

        //=============================================================================================

        public ChromiumWebBrowser Browser { get; private set; }

        private Task CreateBrowser()
        {
            var handler = new WebViewHandlers();
            const string indexPage = "about:blank";
            Browser = new ChromiumWebBrowser(indexPage)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                MenuHandler = handler,
                //KeyboardHandler = new KeyBoardHandler(this),
                LifeSpanHandler = handler,
                //DownloadHandler = new DownloadHandler(),
                //DragHandler = new DragHandler(),
                RequestHandler = handler,
            };
            Browser.ConsoleMessage += (_, x) => Console.WriteLine($"Cef.Browser >> [{x.Level}] {x.Source},{x.Line} {x.Message}");
            var semaphore = new SemaphoreSlim(0, 1);

            Browser.FrameLoadStart += FrameLoadStart;

            return semaphore.WaitAsync();

            void FrameLoadStart(object sender, FrameLoadStartEventArgs e)
            {
                Browser.Dispatcher?.Invoke(() => Browser.Title = "");
                using (semaphore)
                {
                    Browser.FrameLoadStart -= FrameLoadStart;
                    semaphore.Release();
                }
            }
        }


        public void Bind(Window window, string configJson)
        {
            CreateBrowser()
                .ContinueWith(async task =>
                {
                    var json = await Browser.EvaluateScriptAsync("(function(){return " + configJson + ";})()");
                    if (!json.Success || !(json.Result is ExpandoObject obj))
                    {
                        MessageBox.Show("配置文件格式错误:" + json.Message + Environment.NewLine + configJson);
                        window.Dispatcher?.Invoke(window.Close);
                        return;
                    }
                    var config = new Config(obj);
                    PlugInManager.Configuration(config);
                    PlugInManager.Execute(x => x.OnWindowLoad(window, Browser));
                    Address = GetRealUrl(config.Url);
                });
            window.Closed += Window_Closed;
            ((IAddChild)window).AddChild(Browser);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var action in new Action[]{
                () => Browser?.CloseDevTools(),
                () => Browser?.GetBrowser().CloseBrowser(true),
                () => Browser?.Dispose(),
            })
            {
                try
                {
                    action();
                }
                catch { }
            }
        }

        public string Address
        {
            get => Browser?.Dispatcher?.Invoke(() => Browser.Address);
            set => Browser?.Dispatcher?.Invoke(() => Browser.Address = value);
        }

        private string GetRealUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                //处理相对路径的硬盘文件
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri == false && url.IndexOf("://", StringComparison.Ordinal) < 0)
                {
                    if (Browser?.Address != null && Browser.Address.IndexOf("://", StringComparison.Ordinal) >= 0)
                    {
                        return new Uri(new Uri(Browser.Address), url).AbsoluteUri;
                    }
                    return "file://" + Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + url);
                }
            }
            return url;
        }

    }
}
