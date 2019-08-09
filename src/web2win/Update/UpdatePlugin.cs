using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CefSharp;
using CefSharp.Wpf;
using web2win.Plugins;

namespace web2win.Update
{
    class UpdatePlugin : PluginBase
    {
        private ChromiumWebBrowser _browser;
        private Window _window;

        public override void Configuration(Config config) => Enabled = true;

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser)
        {
            _browser = browser;
            _window = window;
            //_browser.RegisterJsObject("updater", this);
            //_browser.JavascriptObjectRepository.Register("updater", this, true, new BindingOptions());
            _browser.JavascriptObjectRepository.ResolveObject += (_, e) =>
            {
                var repo = e.ObjectRepository;
                if (e.ObjectName == "updater")
                {
                    repo.Register("updater", this, true, BindingOptions.DefaultBinder);
                }
            };
            GetUpdateInfo().ConfigureAwait(false);
        }


        public void Configure(UpdateRelease[] releases)
        {
            UpdateManager.Configure(releases);
            _window.Invoke(() =>
            {
                var btn = new Button() { Content = "有新版本" };
                btn.Click += delegate { UpdateManager.ShowWindow(_window); };
                ((IAddChild)_window).AddChild(btn);
            });
        }

        private async Task GetUpdateInfo(int times = 1)
        {
            try
            {
                Console.WriteLine((times == 1 ? "正在" : "第" + times + "次尝试") + "检查更新...");
                using (var web = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //加上这一句
                    web.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "blqw.web2win/1.0");
                    var source = new CancellationTokenSource(15000);
                    var res = await web.GetAsync("https://api.github.com/repos/blqw/init/releases", source.Token);
                    var json = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("更新描述: " + json);
                    _browser.Invoke(() => _browser.ExecuteScriptAsyncWhenPageLoaded($@"CefSharp.BindObjectAsync('updater','configure').then(()=>updater.configure({json}));"));
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (times >= 3)
                {
                    MessageBox.Show("自动更新检查失败:" + ex.Message);
                    return;
                }
            }
            GetUpdateInfo(times + 1).ConfigureAwait(false);
        }
    }
}
