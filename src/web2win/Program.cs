using System;
using System.Collections;
using System.IO;
using System.Windows;

namespace web2win
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            ConsoleLog.Enable();

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
