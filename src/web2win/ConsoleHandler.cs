using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace web2win
{
    class ConsoleHandler
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // 启动控制台
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool AllocConsole();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetConsoleWindow();


        public static void Show() => Visible = true;

        public static void Hide() => Visible = false;

        private static bool _visible;

        public static IntPtr Handle { get; private set; } = IntPtr.Zero;


        public static bool Visible
        {
            get => _visible;
            set
            {
                if (Handle == IntPtr.Zero && value)
                {
                    AllocConsole();
                    Handle = GetConsoleWindow();
                    var closeMenu = GetSystemMenu(Handle, IntPtr.Zero);
                    const uint SC_CLOSE = 0xF060;
                    RemoveMenu(closeMenu, SC_CLOSE, 0x0);
                    Console.Title = Process.GetCurrentProcess().MainWindowTitle + " 控制台";
                }
                ShowWindow(Handle, (_visible = value) ? 1 : 0);
            }
        }


    }

}
