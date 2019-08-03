using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace web2win
{
    public class Config
    {
        private readonly IDictionary<string, object> _dict;

        public Config(IDictionary<string, object> dict)
        {
            foreach (var item in dict)
            {
                if (Properties.TryGetValue(item.Key.Replace("_", ""), out var prop) && prop.CanWrite)
                {
                    prop.SetValue(this, Convert.ChangeType(item.Value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
                }
            }

            _dict = dict;
        }

        public object this[string key]
            => _dict.TryGetValue(key, out var obj) ? obj : null;

        public static Dictionary<string, PropertyInfo> Properties { get; } = typeof(Config).GetProperties().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 初始页面
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 禁用右键
        /// </summary>
        public bool DisableRightClick { get; set; } = true;
        /// <summary>
        /// 禁用历史
        /// </summary>
        public bool DisableHistory { get; set; } = true;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        /// <remarks>优先使用配置中的图标,如果没有则获取快捷方式的图标,如果还没有则获取网站的图标</remarks>
        public string Icon { get; set; }
        /// <summary>
        /// 启用F12调试
        /// </summary>
        public bool EnableF12 { get; set; }
        // 启用小托盘
        public bool EnableTray { get; set; }
        // 启用快捷键
        // 最小化到托盘
        // 限制域名
        // 保持心跳
        public bool Topmost { get; set; }
        // 贴边隐藏
        // 最大宽度
        public int? MaxWidth { get; set; }
        // 最小宽度
        public int? MinWidth { get; set; }
        // 最大高度
        public int? MaxHeight { get; set; }
        // 最小高度
        public int? MinHeight { get; set; }
        // 初始宽度
        public int? Width { get; set; }
        // 初始高度
        public int? Height { get; set; }

        // 初始Top
        public int? Top { get; set; }
        // 初始Left
        public int? Left { get; set; }

        /// <summary>
        /// 关闭时保存窗口位置
        /// </summary>
        public bool ClosedSaveLocation { get; set; }

        /// <summary>
        /// 浏览器日志级别
        /// </summary>
        public string BrowserLogLevel { get; set; }







    }
}
