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
        static readonly string _path = $"Logs\\{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.log";

        public static void Enable()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            var logWriter = new StreamWriter(File.OpenWrite(_path));
            //var @out = Console.Error;
            //var writer = (StreamWriter)@out.GetType().GetField("_out", (BindingFlags)(-1)).GetValue(@out);
            //writer.GetType().GetField("stream", (BindingFlags)(-1)).SetValue(writer, new MyStream(writer.BaseStream));
            //@out.GetType().GetField("_out", (BindingFlags)(-1)).SetValue(@out, new StreamWriter(new MyStream(writer.BaseStream)));

            Console.SetOut(new Out(Console.Out.Encoding, Console.Out.FormatProvider, Console.Out, logWriter));
            Console.SetError(new Out(Console.Error.Encoding, Console.Error.FormatProvider, Console.Error, logWriter));
        }

        class MyStream : Stream
        {
            private readonly Stream _stream;

            public MyStream(Stream stream) => _stream = stream;

            public override void Flush() => _stream.Flush();
            public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
            public override void SetLength(long value) => _stream.SetLength(value);
            public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);
            public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);

            public override bool CanRead => _stream.CanRead;

            public override bool CanSeek => _stream.CanSeek;

            public override bool CanWrite => _stream.CanWrite;

            public override long Length => _stream.Length;

            public override long Position { get => _stream.Position; set => _stream.Position = value; }
        }

        class Out : TextWriter
        {
            private void WriteTime()
            {
                foreach (var writer in _writers)
                {
                    writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd.fff  "));
                }
            }

            readonly IEnumerable<TextWriter> _writers;
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
                _writers = list.AsReadOnly();
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
                    writer.Flush();
                }
            }

            public override void Close() => Execute(x => x.Close());

            protected override void Dispose(bool disposing) => Execute(x => x.Dispose());

            public override void Flush() => Execute(x => x.Flush());

            public void Write(Exception value)
            {
                if (value == null)
                {
                    WriteTime();
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
                WriteTime();
                Execute(x => x.Write(buffer));
                _isNewLine = buffer != null && buffer.Length > 0 && buffer[buffer.Length - 1] == '\n';
            }

            public override void Write(char[] buffer, int index, int count)
            {
                WriteTime();
                Execute(x => x.Write(buffer, index, count));
                _isNewLine = buffer != null && buffer.Length > 0 && buffer[Math.Min(buffer.Length - 1, index + count)] == '\n';
            }
        }
    }
}
