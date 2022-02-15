using System;
using System.Windows;
using System.Windows.Controls;
using aughip_installer_gui.Pages;
using aughip_installer_gui.Utils;

namespace aughip_installer_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                ((IInstallerPage)((TabItem)this.tabControl.SelectedItem).Content).OnSelected();
            }
        }

        public void GoToTab(int index)
        {
            tabControl.SelectedIndex = index;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tabControl.SelectedIndex == 2) // we can't cancel on the install tab or it might be left in a broken state
            {
                e.Cancel = true; 
            }
            else if (tabControl.SelectedIndex == 1 && // Download page
                ThemedMessageBoxUtil.Show("Are you sure you want to cancel the download(s)?", "Confirm exit", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Utils.Utils.EnableDarkTitleBar(this);
        }
    }
}
