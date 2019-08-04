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
                webview.Configurated += (_, e) => PlugInManager.Execute(x => x.OnWindowLoad(this, webview));
                WebView = webview;
                //SetBinding(BackgroundProperty, new Binding("Background") { Source = webview });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
            => ConsoleWindow.Toggle();
        

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //var bmp = new RenderTargetBitmap(1, 1, 0, 0, PixelFormats.Pbgra32);
                //bmp.Render(WebView);
                //var stride = (bmp.PixelWidth * bmp.Format.BitsPerPixel + 7) / 8;
                //var pixelByteArray = new byte[bmp.PixelHeight * stride];
                //bmp.CopyPixels(pixelByteArray, stride, 0);


                //this.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(pixelByteArray[3], pixelByteArray[2], pixelByteArray[1], pixelByteArray[0]));
                //var rc = new System.Drawing.Rectangle(0, 0, 1, 1);
                //using (var bitmap = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //using (var memoryGrahics = Graphics.FromImage(bitmap))
                //{
                //    memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                //    var c = bitmap.GetPixel(0, 0);
                //    this.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
                //}
                //this.Close();
            }
        }
    }
}
