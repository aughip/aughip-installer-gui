using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace aughip_installer_gui.Controls
{
    /// <summary>
    /// Interaction logic for InstallItem.xaml
    /// </summary>
    public partial class InstallItem : UserControl
    {
        public InstallItem()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InstallItem), new PropertyMetadata("When the impostor is sus", new PropertyChangedCallback(TextChanged)));

        public InstallState CurrentState
        {
            get { return (InstallState)GetValue(CurrentStateProperty); }
            set { SetValue(CurrentStateProperty, value); }
        }

        public static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.Register("CurrentState", typeof(InstallState), typeof(InstallItem), new PropertyMetadata(InstallState.Waiting, new PropertyChangedCallback(StateChanged)));

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InstallItem).itemLabel.Content = (string)e.NewValue;
        }

        private static void StateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as InstallItem;
            Storyboard anim = item.Resources["spinnerSpin"] as Storyboard;

            switch (e.NewValue)
            {
                case InstallState.Done:
                    item.statusIcon.Source = new BitmapImage(new Uri(@"/aughip-installer-gui;component/Assets/taskComplete.png", UriKind.Relative));
                    anim.Stop(item.statusIcon);
                    break;
                case InstallState.Installing:
                    item.statusIcon.Source = new BitmapImage(new Uri(@"/aughip-installer-gui;component/Assets/taskWorking.png", UriKind.Relative));
                    item.statusIcon.BeginStoryboard(anim, HandoffBehavior.SnapshotAndReplace, true);
                    break;
                case InstallState.Waiting:
                    item.statusIcon.Source = new BitmapImage(new Uri(@"/aughip-installer-gui;component/Assets/taskWaiting.png", UriKind.Relative));
                    anim.Stop(item.statusIcon);
                    break;
            }
        }
    }
}
