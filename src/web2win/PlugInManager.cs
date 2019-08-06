using CefSharp.Wpf;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                _plugins?.Clear();
                _events?.Clear();
                Execute(x => x.Dispose());
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


        public static void OnWindowLoad(Window window, ChromiumWebBrowser browser)
            => Execute(x => x.OnWindowLoad(window, browser));

        /// <summary>
        /// 执行插件功能
        /// </summary>
        private static void Execute(Action<IPlugin> action)
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
                    action(plugin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }


        static readonly ConcurrentDictionary<string, List<Action<PluginEventArgs>>> _events
            = new ConcurrentDictionary<string, List<Action<PluginEventArgs>>>();

        public static bool Execute(this PluginEventArgs args)
        {
            if (Plugins is null)
            {
                throw new NotSupportedException("请先调用scan方法来扫描所有插件");
            }
            var argtypes = new[] { typeof(PluginEventArgs) };
            var actions = _events.GetOrAdd(args.EventName, methodName =>
            {
                var events = new List<Action<PluginEventArgs>>();
                foreach (var plugin in _plugins)
                {
                    if (!plugin.Enabled)
                    {
                        continue;
                    }
                    var action = plugin.GetType().GetMethod(methodName, argtypes)?.CreateDelegate(typeof(Action<PluginEventArgs>), plugin);
                    if (action != null)
                    {
                        events.Add((Action<PluginEventArgs>)action);
                    }
                }
                return events;
            });

            foreach (var action in actions)
            {
                try
                {
                    action(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return actions.Count > 0;
        }

        public static T GetResult<T>(this PluginEventArgs args, T defaultValue)
            => args?.Result is T t ? t : defaultValue;
    }
}
