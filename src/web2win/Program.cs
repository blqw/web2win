using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace web2win
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            ConsoleLog.WriteToFile();

            Console.WriteLine("程序启动");

            var app = new App();
            if (StartupCommands.Config == null)
            {
                app.Run(new ConfigWindow());
            }
            else
            {
                using (WebView.UseCef())
                {
                    //app.Run(ConsoleWindow.Instance);
                    app.Run(new MainWindow());
                }
            }
        }

    }
}
