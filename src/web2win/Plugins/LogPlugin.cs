using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace web2win.Plugins
{
    class LogPlugin : PluginBase
    {
        public LogSeverity LogLevel { get; private set; }

        public override void Configuration(Config config)
        {
            LogLevel = Enum.TryParse<LogSeverity>(config.BrowserLogLevel, true, out var level) ? level : LogSeverity.Default;
            Enabled = LogLevel < LogSeverity.Disable;
        }

        public override void OnWindowLoad(Window window, ChromiumWebBrowser browser)
            => browser.ConsoleMessage += (_, x) =>
            {
                if (LogLevel <= x.Level)
                {
                    if (string.IsNullOrEmpty(x.Source))
                    {
                        Console.WriteLine($"Cef.Browser >> [{x.Level}] {x.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Cef.Browser >> [{x.Level}] {x.Source},{x.Line} {x.Message}");
                    }
                }
            };
    }
}
