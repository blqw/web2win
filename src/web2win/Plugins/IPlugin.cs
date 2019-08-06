using CefSharp.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace web2win.Plugins
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// 使用配置文件配置插件
        /// </summary>
        void Configuration(Config config);
        /// <summary>
        /// 插件是否生效
        /// </summary>
        bool Enabled { get; }
        /// <summary>
        /// 软件窗口加载时执行
        /// </summary>
        void OnWindowLoad(Window window, ChromiumWebBrowser browser);
    }
}
