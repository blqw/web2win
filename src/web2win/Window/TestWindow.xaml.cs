using System;
using System.Collections.Generic;
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
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();


            btn2.SetBinding(Button.ContentProperty, new Binding("Title") { Source = this });
            //"RelativeSource={RelativeSource Mode=FindAncestor,AncestorType=Window},Path=Title");
            //btn2.Content = "{Binding  RelativeSource={RelativeSource Mode=FindAncestor,AncestorType=Window},Path=Title}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Title = Guid.NewGuid().ToString();
        }
    }
}
