using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace web2win
{
    class ConsoleLog
    {
        public static void Enable()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            var logWriter = new StreamWriter(File.Open(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
            logWriter.AutoFlush = true;

            Console.SetOut(new Out(Console.Out.Encoding, Console.Out.FormatProvider, Console.Out, logWriter));
            Console.SetError(new Out(Console.Error.Encoding, Console.Error.FormatProvider, Console.Error, logWriter));
        }

        public static string LogFilePath { get; } = $"Logs\\{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.log";

        class Out : TextWriter
        {
            private void WriteTime()
            {
                foreach (var writer in _writers)
                {
                    writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff  "));
                }
            }

            readonly List<TextWriter> _writers;
            private bool _isNewLine = true;
            public Out(Encoding encoding, IFormatProvider formatProvider, params TextWriter[] writers)
                : base(formatProvider)
            {
                Encoding = encoding;
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
                _writers = list;
            }

            public override Encoding Encoding { get; }

            void Execute(Action<TextWriter> action)
            {
                if (_isNewLine)
                {
                    _isNewLine = false;
                    WriteTime();
                }
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
                    _isNewLine = true;
                }
            }

            public override void Write(char value)
            {
                Execute(x => x.Write(value));
                _isNewLine = value == '\n';
            }

            public override void Write(object value) => Execute(x => x.Write(value));

            public override void Write(string format, object arg0)
            {
                Execute(x => x.Write(format, arg0));
                _isNewLine = format != null && format.Length > 0 && format[format.Length - 1] == '\n';
                Write(arg0 as Exception);
            }

            public override void Write(string format, object arg0, object arg1)
            {
                Execute(x => x.Write(format, arg0, arg1));
                _isNewLine = format != null && format.Length > 0 && format[format.Length - 1] == '\n';
                Write(arg0 as Exception);
                Write(arg1 as Exception);
            }

            public override void Write(string format, object arg0, object arg1, object arg2)
            {
                Execute(x => x.Write(format, arg0, arg1, arg2));
                _isNewLine = format != null && format.Length > 0 && format[format.Length - 1] == '\n';
                Write(arg0 as Exception);
                Write(arg1 as Exception);
                Write(arg2 as Exception);
            }

            public override void Write(string format, params object[] args)
            {
                Execute(x => x.Write(format, args));
                _isNewLine = format != null && format.Length > 0 && format[format.Length - 1] == '\n';
                foreach (var arg in args)
                {
                    Write(arg as Exception);
                }
            }

            public override void Write(string value)
            {
                Execute(x => x.Write(value));
                _isNewLine = value != null && value.Length > 0 && value[value.Length - 1] == '\n';
            }

            public override void Write(char[] buffer)
            {
                Execute(x => x.Write(buffer));
                _isNewLine = buffer != null && buffer.Length > 0 && buffer[buffer.Length - 1] == '\n';
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Execute(x => x.Write(buffer, index, count));
                _isNewLine = buffer != null && buffer.Length > 0 && buffer[Math.Min(buffer.Length - 1, index + count)] == '\n';
            }
        }
    }
}
