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

namespace aughip_installer_gui.Controls
{
    /// <summary>
    /// Interaction logic for DownloadItem.xaml
    /// </summary>
    public partial class DownloadItem : UserControl
    {
        public DownloadItem()
        {
            InitializeComponent();
        }

        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(DownloadItem), new PropertyMetadata(0.0, new PropertyChangedCallback(ProgressValueChanged)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DownloadItem), new PropertyMetadata("DownloadProgress", new PropertyChangedCallback(TextChanged)));


        private static void ProgressValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DownloadItem).progressBar.Value = (double)e.NewValue;
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DownloadItem).label.Content = (string)e.NewValue;
        }
    }
}
