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
            ConsoleLog.Enable();

            Console.WriteLine("程序启动");


            var app = new App();
            if (StartupCommands.Config == null)
            {

            }
            else
            {
                if (StartupCommands.Console)
                {
                    ConsoleWindow.Initialize();
                }
                using (WebView.UseCef())
                {
                    //app.Run(ConsoleWindow.Instance);
                    app.Run(new MainWindow());
                }
            }
        }

    }
}
