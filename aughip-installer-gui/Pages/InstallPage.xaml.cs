using System.Windows;
using System.Windows.Controls;

namespace aughip_installer_gui.Pages
{
    /// <summary>
    /// Interaction logic for InstallPage.xaml
    /// </summary>
    public partial class InstallPage : UserControl, IInstallerPage
    {
        public InstallPage()
        {
            InitializeComponent();
        }

        public void OnSelected()
        {
            Installer.Installer.InstallAugHip();
            MessageBox.Show("Successfully installed Augmented Hip!", "Install success!");
            Application.Current.Shutdown(0);
        }
    }
}
