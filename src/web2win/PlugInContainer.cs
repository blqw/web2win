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
    static class PlugInContainer
    {

        public static void Scan()
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
            Plugins = types.Select(Activator.CreateInstance).Cast<IPlugin>().ToList().AsReadOnly();
        }

        public static IList<IPlugin> Plugins { get; private set; }

        public static void Configuration(Config config)
        {
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

        public static void Execute<T>(Action<T> action)
            where T : class
        {
            foreach (var plugin in Plugins)
            {
                if (!plugin.Enabled)
                {
                    continue;
                }
                try
                {
                    var x = plugin.GetComponent<T>();
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

        public static void Execute(Action<IPlugin> action)
            => Execute<IPlugin>(action);

    }
}
