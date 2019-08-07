using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using CefSharp.Wpf;
using MessageBox = System.Windows.MessageBox;

namespace web2win.Plugins
{
    class BossKeyPlugin : PluginBase
    {
        public HotKey.KeyModifiers Modifiers { get; private set; }
        public Keys Key { get; private set; }
        public Window Window { get; private set; }

        public override void Configuration(Config config)
        {
            var bosskey = config.BossKey;
            if (bosskey == null)
            {
                return;
            }
            var modifiers = HotKey.KeyModifiers.None;
            var key = Keys.None;

            try
            {
                foreach (var item in bosskey.Split('+'))
                {
                    switch (item.Trim().ToLowerInvariant())
                    {
                        case "alt":
                            if (modifiers.HasFlag(HotKey.KeyModifiers.WindowsKey))
                            {
                                return;
                            }
                            modifiers |= HotKey.KeyModifiers.Alt;
                            break;
                        case "ctrl":
                            if (modifiers.HasFlag(HotKey.KeyModifiers.Ctrl))
                            {
                                return;
                            }
                            modifiers |= HotKey.KeyModifiers.Ctrl;
                            break;
                        case "shift":
                            if (modifiers.HasFlag(HotKey.KeyModifiers.Shift))
                            {
                                return;
                            }
                            modifiers |= HotKey.KeyModifiers.Shift;
                            break;
                        case "win":
                            if (modifiers.HasFlag(HotKey.KeyModifiers.WindowsKey))
                            {
                                return;
                            }
                            modifiers |= HotKey.KeyModifiers.WindowsKey;
                            break;
                        default:
                            if (key != Keys.None)
                            {
                                return;
                            }
                            if (!Enum.TryParse(item, true, out key))
                            {
                                return;
                            }
                            break;
                    }
                }

                if (modifiers == HotKey.KeyModifiers.None || key == Keys.None)
                {
                    return;
                }

                Modifiers = modifiers;
                Key = key;

                Enabled = true;
            }
            finally
            {
                if (!Enabled && !string.IsNullOrWhiteSpace(config.BossKey))
                {
                    Console.WriteLine(new Exception("快捷键注册失败"));
                }
            }


        }

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser)
        {
            var b = window.Dispatcher?.Invoke(() =>
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                var id = 0x1234;
                if (!HotKey.RegKey(hwnd, id, Modifiers, Key))
                {
                    Console.WriteLine(new Exception("快捷键注册失败"));
                    return false;
                }
                var source = PresentationSource.FromVisual(window) as HwndSource;
                source.AddHook(WndProc);
                Window = window;
                window.Closing += (_, x) =>
                {
                    HotKey.UnRegKey(hwnd, id);
                    source.RemoveHook(WndProc);
                };
                return true;
            });

            if (b != true)
            {
                MessageBox.Show("快捷键注册失败");
            }
        }

        /// <summary>
        /// 热键消息
        /// </summary>
        public const int WM_HOTKEY = 0x312;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == 0x1234)
            {
                if (Window.IsVisible)
                {
                    Window.Hide();
                }
                else
                {
                    Window.Show();
                    Window.Activate();
                }
            }
            return IntPtr.Zero;
        }
    }
}
