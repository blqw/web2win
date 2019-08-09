using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using web2win.Properties;

namespace web2win.Update
{
    public static class UpdateManager
    {
        static UpdateRelease[] _releases;
        static UpdateAsset _full;
        static UpdateAsset[] _update;
        static UpdateAsset _prefull;
        static UpdateAsset[] _preupdate;
        static string _releaseDescription;
        static Version _version;
        public static void Configure(UpdateRelease[] releases)
        {
            _releases = releases;
            var buffer = new StringBuilder();

            foreach (var release in _releases)
            {
                buffer.Append("## ");
                buffer.Append(release.tag_name);
                if (release.prerelease)
                {
                    buffer.Append("<sup>(预览版)</sup>");
                }
                buffer.Append(" `");
                buffer.Append(DateTime.Parse(release.created_at).ToString("yyyy-MM-dd"));
                buffer.AppendLine("`");
                buffer.AppendLine(release.body);
                buffer.AppendLine();
            }
            _version = typeof(UpdateManager).Assembly.GetName().Version;
            var assets = _releases.Where(x => !x.prerelease).SelectMany(x => x.assets).ToArray();
            var preassets = _releases.Where(x => x.prerelease).SelectMany(x => x.assets).ToArray();

            _full = assets.FirstOrDefault(x => x.IsFull);
            _update = ResolveUpdate(assets)?.Reverse().ToArray();

            _prefull = preassets.FirstOrDefault(x => x.IsFull);
            _preupdate = ResolveUpdate(preassets)?.Reverse().ToArray();

            _releaseDescription = buffer.ToString();
        }

        private static IEnumerable<UpdateAsset> ResolveUpdate(UpdateAsset[] assets)
        {
            var asset = assets.FirstOrDefault(x => x.IsUpdate);
            var list = new List<UpdateAsset>();

            while (asset != null && asset.Version > _version)
            {
                list.Add(asset);
                asset = assets.LastOrDefault(x => x.Version >= asset.MinVersion && x.Version > asset.Version);
            }
            asset = list.LastOrDefault();
            if (asset == null || asset.MinVersion > _version)
            {
                return null;
            }
            return list;
        }

        public static void ShowWindow(Window parent)
        {
            var win = new Window()
            {
                Height = 700,
                Width = 500,
                Title = "当前版本:" + _version,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowStyle = WindowStyle.ToolWindow,
                Owner = parent
            };
            var grid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(5, 5, 5, 5)
            };
            var web = new ChromiumWebBrowser("about:blank")
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 54),
                //Text = _releaseDescription
                MenuHandler = new DisabledMenu(),
            };
            web.IsBrowserInitializedChanged += initialized;
            void initialized(object sender, DependencyPropertyChangedEventArgs e)
            {
                web.IsBrowserInitializedChanged -= initialized;
                web.RegisterResourceHandler("http://update.com/showdown.js", new MemoryStream(Encoding.UTF8.GetBytes(Markdown.SHOWDOWN_JS)), "application/javascript");
                web.LoadHtml(Markdown.HTML, "http://update.com", Encoding.UTF8, false);
                web.FrameLoadEnd += delegate { web.ExecuteScriptAsync("compile", _releaseDescription); };
            };


            var prs = new ProgressBar()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 32),
                Height = 20,
                Maximum = _update?.Sum(x => x.size) ?? _full?.size ?? 0,
                IsEnabled = _update != null && _full != null,
            };

            var btn = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 30,
                Width = 100,
                Content = "立刻升级",
            };

            grid.Children.Add(web);
            grid.Children.Add(prs);
            grid.Children.Add(btn);
            ((IAddChild)win).AddChild(grid);
            win.ShowDialog();

        }

    }



}
