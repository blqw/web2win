using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace web2win.Plugins
{
    public class PluginEventArgs : EventArgs
    {
        IDictionary<string, object> _dict;
        public PluginEventArgs([CallerMemberName]string eventName = null)
        {
            EventName = eventName;
            var obj = new ExpandoObject();
            Data = obj;
            _dict = obj;
        }

        public PluginEventArgs(object args, [CallerMemberName]string eventName = null)
            : this(eventName)
        {
            foreach (var p in args.GetType().GetProperties())
            {
                Set(p.Name, p.GetValue(args));
            }
        }

        public dynamic Data { get; }

        public string EventName { get; private set; }

        public T Get<T>(string name = null)
        {
            if (name != null)
            {
                return _dict.TryGetValue(name, out var value) && value is T t ? t : default;
            }
            return _dict.Where(x => x.Value is T).Select(x => (T)x.Value).SingleOrDefault();
        }

        public void Set(string name, object value)
            => _dict[name] = value;

        public object Result { get; set; }
    }
}
