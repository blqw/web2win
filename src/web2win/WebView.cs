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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using web2win.Plugins;

namespace web2win
{
    class WebView : ChromiumWebBrowser
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

        const string INDEX_PAGE = "about:blank";

        public WebView(string configJson)
            : base(INDEX_PAGE)
        {
            var handler = new WebViewHandlers();
            MenuHandler = handler;
            //KeyboardHandler = new KeyBoardHandler(this),
            LifeSpanHandler = handler;
            //DownloadHandler = new DownloadHandler(),
            //DragHandler = new DragHandler(),
            RequestHandler = handler;
            FrameLoadStart += firstLoad;

            async void firstLoad(object sender, FrameLoadStartEventArgs e)
            {
                Dispatcher?.Invoke(() => Title = "");
                FrameLoadStart -= firstLoad;

                var json = await this.EvaluateScriptAsync("(function(){return " + configJson + ";})()");
                if (!json.Success || !(json.Result is ExpandoObject obj))
                {
                    throw new Exception("配置文件格式错误:" + json.Message + Environment.NewLine + configJson);
                }
                var config = new Config(obj);
                PlugInManager.Configuration(config);
                Dispatcher?.Invoke(() => Address = GetRealUrl(config.Url));
                Configurated?.Invoke(this, EventArgs.Empty);
            }

            //FrameLoadEnd += WebView_FrameLoadEnd;
        }

        //private void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        //{
        //    this.Dispatcher?.Invoke(() =>
        //    {
        //        var bmp = new RenderTargetBitmap(1, 1, 0, 0, PixelFormats.Pbgra32);
        //        bmp.Render(this);
        //        var stride = (bmp.PixelWidth * bmp.Format.BitsPerPixel + 7) / 8;
        //        var pixelByteArray = new byte[bmp.PixelHeight * stride];
        //        bmp.CopyPixels(pixelByteArray, stride, 0);
        //        Background = new SolidColorBrush(Color.FromArgb(pixelByteArray[3], pixelByteArray[2], pixelByteArray[1], pixelByteArray[0]));
        //    });
        //}

        public event EventHandler Configurated;

        

        private string GetRealUrl(string url)
        {
            var address = Address;
            if (!string.IsNullOrWhiteSpace(url))
            {
                //处理相对路径的硬盘文件
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri == false && url.IndexOf("://", StringComparison.Ordinal) < 0)
                {
                    if (address != null && address.IndexOf("://", StringComparison.Ordinal) >= 0)
                    {
                        return new Uri(new Uri(address), url).AbsoluteUri;
                    }
                    return "file://" + Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + url);
                }
            }
            return url;
        }

    }
}
