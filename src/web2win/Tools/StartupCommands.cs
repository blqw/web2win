using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web2win
{
    static class StartupCommands
    {
        static StartupCommands()
        {
            try
            {
                var args = Environment.GetCommandLineArgs().Skip(1).ToArray(); // 去掉命令行第一个命令
                foreach (var prop in typeof(StartupCommands).GetProperties())
                {
                    // 循环当前类的属性, 优先获取 --{属性全名,不区分大小写} 如 --loginname
                    // 如果没有取到, 再次获取 -{短命令} 如 -n
                    var arg = args.FirstOrDefault(x => x.StartsWith($"--{prop.Name}", StringComparison.OrdinalIgnoreCase))
                                ?? (Attribute.GetCustomAttribute(prop, typeof(ShortAttribute)) is ShortAttribute attr
                                    ? args.FirstOrDefault(x => x.StartsWith($"-{attr.Command}", StringComparison.Ordinal))
                                    : null);
                    if (arg == null) // 都没取到则忽略
                    {
                        continue;
                    }
                    // 获取命令 冒号(:) 之后的内容, 当做属性值, 如果不存在冒号
                    // 默认值为true , 如: --debug  等于  --debug:true
                    var val = arg.Split(':').ElementAtOrDefault(1)?.Trim() ?? "true";
                    // 转换类型并设置到属性
                    prop.SetValue(null, Convert.ChangeType(val, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("SuperAddon初始化 异常", ex);
                throw new NotSupportedException("启动参数有误", ex);
            }
        }

        /// <summary>
        /// 短命令
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        sealed class ShortAttribute : Attribute
        {
            public ShortAttribute(string command) => Command = command;
            public string Command { get; }
        }

        [Short("c")]
        public static string Config { get; private set; }

        public static bool Console { get; private set; }
    }
}
