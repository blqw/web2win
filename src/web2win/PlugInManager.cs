using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using web2win.Plugins;

namespace web2win
{
    /// <summary>
    /// 插件管理器
    /// </summary>
    public static class PlugInManager
    {
        /// <summary>
        /// 扫描所有插件, 并返回一个可释放对象, 用于释放所有插件
        /// </summary>
        /// <returns></returns>
        internal static IDisposable Scan()
        {
            var domain = AppDomain.CurrentDomain;

            var core = typeof(int).Assembly;
            var assemblies = domain.GetAssemblies();
            var types = assemblies.SelectMany(x => x.GetReferencedAssemblies())
                                .Where(x => !x.Name.StartsWith("Microsoft") && !x.Name.StartsWith("System"))
                                .Distinct()
                                .Select(Assembly.Load)
                                .Union(assemblies)
                                .Where(x => x != core)
                                .Distinct()
                                .SelectMany(x =>
                                {
                                    if (x == null)
                                    {
                                        return Type.EmptyTypes;
                                    }
                                    try
                                    {
                                        return x.DefinedTypes.Cast<Type>();
                                    }
                                    catch (ReflectionTypeLoadException ex)
                                    {
                                        return ex.Types;
                                    }
                                    catch
                                    {
                                        return Type.EmptyTypes;
                                    }
                                })
                                .Where(x => x != null && x.IsClass && !x.IsAbstract && !x.IsGenericType && typeof(IPlugin).IsAssignableFrom(x))
                                .ToList();
            _plugins = types.Select(Activator.CreateInstance).Cast<IPlugin>().ToList();
            Plugins = _plugins.AsReadOnly();
            return new Disposable();
        }

        class Disposable : IDisposable
        {
            public void Dispose()
            {
                Execute(x => x.Dispose());
                _plugins?.Clear();
            }
        }

        private static List<IPlugin> _plugins;

        /// <summary>
        /// 所有插件的只读集合
        /// </summary>
        public static IReadOnlyList<IPlugin> Plugins { get; private set; }
        /// <summary>
        /// 配置所有插件
        /// </summary>
        public static void Configuration(Config config)
        {
            if (Plugins is null)
            {
                throw new NotSupportedException("请先调用scan方法来扫描所有插件");
            }

            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Configuration(config);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// 执行插件功能
        /// </summary>
        public static void Execute<T>(Action<T> action)
            where T : class
        {
            if (Plugins is null)
            {
                throw new NotSupportedException("请先调用scan方法来扫描所有插件");
            }
            foreach (var plugin in Plugins)
            {
                if (!plugin.Enabled)
                {
                    continue;
                }
                try
                {
                    var x = plugin.GetFeature<T>();
                    if (x != null)
                    {
                        action(x);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        /// <summary>
        /// 执行插件功能
        /// </summary>
        public static void Execute(Action<IPlugin> action)
            => Execute<IPlugin>(action);

    }
}
