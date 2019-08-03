using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();

            Height = 100;
            Width = 100;
            Top = -500;
            WindowStyle = WindowStyle.None;
            if (!File.Exists(StartupCommands.Config))
            {
                MessageBox.Show("配置不存在");
                this.Close();
                return;
            }
            var config = File.ReadAllText(StartupCommands.Config);
            try
            {
                new WebView().Bind(this, config);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
                return;
            }
        }
    }
}
