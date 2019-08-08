using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace web2win
{
    /// <summary>
    /// ConsoleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        private ConsoleWindow()
        {
            InitializeComponent();
            _fileReader = new StreamReader(File.Open(ConsoleLog.LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            Icon = new DrawingImage();
            _timer = new Timer(o =>
            {
                if (this.IsVisible)
                {
                    lock (_fileReader)
                    {
                        var txt = _fileReader.ReadToEnd();
                        if (txt.Length == 0)
                        {
                            return;
                        }
                        var line = txt.Count(c => c == '\n') + (txt.EndsWith("\n") ? 0 : 1);
                        var maxLine = 20;
                        this.Invoke(() =>
                        {
                            var lineCount = showlogs.LineCount + (showlogs.Text.EndsWith("\n") ? -1 : 0);
                            var overflow = lineCount + line - maxLine;
                            var ss = showlogs.SelectionStart;
                            var sl = showlogs.SelectionLength;
                            var deletedCharCount = DeleteTopLines(overflow);
                            showlogs.AppendText(txt);

                            if (ss == showlogs.Text.Length)
                            {
                                // 自动滚屏
                                showlogs.SelectionStart = showlogs.Text.Length;
                                return;
                            }

                            ss -= deletedCharCount;
                            if (ss >= 0)
                            {
                                showlogs.Select(ss, sl);
                                return;
                            }

                            sl += ss;
                            if (sl >= 0)
                            {
                                // 还有部分选择项残留
                                showlogs.Select(0, sl);
                                return;
                            }
                            showlogs.SelectionStart = 0;

                        });
                    }
                }
            }, null, 0, 1000);
            Closing += ConsoleWindow_Closing;
        }

        /// <summary>
        /// 删除指定行数的文本, 返回删除的字符数
        /// </summary>
        /// <param name="lineCount"></param>
        /// <returns></returns>
        private int DeleteTopLines(int lineCount)
        {
            if (lineCount <= 0)
            {
                return 0;
            }
            var firstChar = showlogs.GetCharacterIndexFromLineIndex(lineCount);
            showlogs.Select(0, firstChar);
            showlogs.SelectedText = "";
            return firstChar;
        }

        private void ConsoleWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (base.IsEnabled)
            {
                Hide();
                e.Cancel = true;
            }
            else
            {
                using (_fileReader)
                using (_timer)
                {

                }
            }
        }

        private readonly StreamReader _fileReader;
        private readonly Timer _timer;

        private static Window _instance;

        internal static Window Instance => _instance;

        public static void Initialize()
        {
            var win = new ConsoleWindow();


            _instance = win;
        }

        public new static void Show() => _instance?.Show();

        public new static void Hide() => _instance?.Hide();

        public new static void Close()
        {
            _instance.IsEnabled = false;
            _instance?.Close();
        }

        public static void Toggle()
        {
            if (_instance.IsVisible)
            {
                _instance.Hide();
            }
            else
            {
                _instance.Show();
            }
        }
    }
}
