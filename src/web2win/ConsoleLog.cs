using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace web2win
{
    class ConsoleLog
    {
        static readonly string _path = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.log";

        public static void Enable()
            => Console.SetOut(new Out(Console.Out.Encoding, Console.Out.FormatProvider, Console.Out, new StreamWriter(File.OpenWrite(_path))));

        class Out : TextWriter
        {
            readonly IEnumerable<TextWriter> _writers;
            public Out(Encoding encoding, IFormatProvider formatProvider, params TextWriter[] writers)
            {
                Encoding = encoding;
                FormatProvider = formatProvider;
                var list = new List<TextWriter>();
                foreach (var writer in writers)
                {
                    if (Console.Out is Out o)
                    {
                        list.AddRange(o._writers);
                    }
                    else
                    {
                        list.Add(writer);
                    }
                }
                _writers = list.AsReadOnly();
            }

            public override Encoding Encoding { get; }

            public override IFormatProvider FormatProvider { get; }

            void Execute(Action<TextWriter> action)
            {
                foreach (var writer in _writers)
                {
                    action(writer);
                }
            }

            public override void Close() => Execute(x => x.Close());

            protected override void Dispose(bool disposing) => Execute(x => x.Dispose());

            public override void Flush() => Execute(x => x.Flush());

            public void Write(Exception value)
            {
                if (value == null)
                {
                    Execute(x => x.Write(value));
                }
            }

            public override void Write(char value) => Execute(x => x.Write(value));

            public override void Write(object value) => Execute(x => x.Write(value));

            public override void Write(string format, object arg0)
            {
                Execute(x => x.Write(format, arg0));
                Write(arg0 as Exception);
            }

            public override void Write(string format, object arg0, object arg1)
            {
                Execute(x => x.Write(format, arg0, arg1));
                Write(arg0 as Exception);
                Write(arg1 as Exception);
            }

            public override void Write(string format, object arg0, object arg1, object arg2)
            {
                Execute(x => x.Write(format, arg0, arg1, arg2));
                Write(arg0 as Exception);
                Write(arg1 as Exception);
                Write(arg2 as Exception);
            }

            public override void Write(string format, params object[] args)
            {
                Execute(x => x.Write(format, args));
                foreach (var arg in args)
                {
                    Write(arg as Exception);
                }
            }

            public override void Write(string value) => Execute(x => x.Write(value));
        }
    }
}
