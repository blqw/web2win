using CefSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace web2win
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        WebView WebView { get; }

        public MainWindow()
        {
            InitializeComponent();
            btnConsole.Visibility = StartupCommands.Console ? Visibility.Visible : Visibility.Hidden;
            Height = 100;
            Width = 100;
            Top = -500;
            WindowStyle = WindowStyle.None;
            if (!File.Exists(StartupCommands.Config))
            {
                MessageBox.Show("配置不存在");
                Close();
                return;
            }
            var config = File.ReadAllText(StartupCommands.Config);
            try
            {
                var webview = new WebView(config)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                main.Children.Add(webview);
                webview.Configurated += (_, e) => PlugInManager.OnWindowLoad(this, webview);
                WebView = webview;
                //SetBinding(BackgroundProperty, new Binding("Background") { Source = webview });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }



            if (StartupCommands.Console)
            {
                ConsoleWindow.Initialize();
                this.Closing += (_, x) => ConsoleWindow.Close();
            }
        }

        private void ShowConsole(object sender, RoutedEventArgs e)
            => ConsoleWindow.Toggle();

        private void ShowDevTools(object sender, RoutedEventArgs e)
           => WebView.ShowDevTools();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.Close();
            }
        }
    }
}
