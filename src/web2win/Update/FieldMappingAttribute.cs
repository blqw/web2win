using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web2win.Update
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class FieldMappingAttribute : Attribute
    {
        public FieldMappingAttribute(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }

        public Func<object, object> Converter;
    }
}
