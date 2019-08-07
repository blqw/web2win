using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CefSharp.Wpf;

namespace web2win.Plugins
{
    class NotifyIconPlugin : PluginBase
    {
        public bool MinimizeToTray { get; private set; }

        public override void Configuration(Config config)
        {
            Enabled = config.EnableTray;
            MinimizeToTray = config.MinimizeToTray;
        }

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser)
        {
            browser.Dispatcher?.Invoke(() =>
            {
                var notify = new NotifyIcon { Text = window.Title };
                if (window.Icon != null)
                {
                    notify.Icon = ToIcon(window.Icon);
                }
                window.Closing += (_, x) => notify.Dispose();
                notify.ContextMenu = CreateMenu("打开", "退出");
                notify.ContextMenu.MenuItems[0].Click += (_, x) => window.Visibility = Visibility.Visible;
                notify.ContextMenu.MenuItems[1].Click += (_, x) => window.Close();
                notify.DoubleClick += (_, x) => Toggle(window);
                notify.Visible = true;
                if (MinimizeToTray)
                {
                    window.StateChanged += (_, x) =>
                    {
                        if (window.WindowState == WindowState.Minimized)
                        {
                            window.WindowState = WindowState.Normal;
                            window.Hide();
                        }
                    };
                }
                var dpd = DependencyPropertyDescriptor.FromProperty(Window.IconProperty, typeof(Window));
                if (dpd != null)
                {
                    dpd.AddValueChanged(window, delegate
                    {
                        if (window.Icon != null)
                        {
                            notify.Icon = ToIcon(window.Icon);
                        }
                    });
                }
            });

        }

        private ContextMenu CreateMenu(params string[] texts)
            => new ContextMenu(texts.Select(x => new MenuItem(x)).ToArray());

        private void Toggle(Window window)
        {
            if (window.IsVisible)
            {
                window.Hide();
            }
            else
            {
                window.Show();
                window.Activate();
            }
        }

        public static Icon ToIcon(ImageSource source)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)source));
                encoder.Save(ms);
                using (var bitmap = new Bitmap(ms))
                {
                    return Icon.FromHandle(bitmap.GetHicon());
                }
            }
        }
    }
}
