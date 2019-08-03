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
        /// 最大宽度
        /// </summary>
        public int? MaxWidth { get; set; }
        /// <summary>
        /// 最小宽度
        /// </summary>
        public int? MinWidth { get; set; }
        /// <summary>
        /// 最大高度
        /// </summary>
        public int? MaxHeight { get; set; }
        /// <summary>
        /// 最小高度
        /// </summary>
        public int? MinHeight { get; set; }
        /// <summary>
        /// 初始宽度
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// 初始高度
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// 初始Top
        /// </summary>
        public int? Top { get; set; }
        /// <summary>
        /// 初始Left
        /// </summary>
        public int? Left { get; set; }
        /// <summary>
        /// 窗口置顶
        /// </summary>
        public bool Topmost { get; set; }
        /// <summary>
        /// 禁用右键
        /// </summary>
        public bool DisableRightClick { get; set; }
        /// <summary>
        /// 禁用历史
        /// </summary>
        public bool DisableHistory { get; set; }
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
        // 最小化到托盘
        public bool MinimizeToTray { get; set; }
        // 启用老板键
        public string BossKey { get; set; }

        /// <summary>
        /// URL过滤
        /// </summary>
        public string UrlFilter { get; set; }

        // 保持心跳
        // 贴边隐藏

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
