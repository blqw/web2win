using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
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


            if (StartupCommands.Console)
            {
                Task.Delay(2000).ContinueWith(t =>
                {
                    //ConsoleHandler.Show();
                });
            }
            var app = new App();
            if (StartupCommands.Config == null)
            {

            }
            else
            {
                using (WebView.UseCef())
                {
                    app.Run(new MainWindow());
                }
            }
        }

    }
}
