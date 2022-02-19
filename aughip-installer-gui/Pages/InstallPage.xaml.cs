using System.Windows;
using System.Windows.Controls;
using aughip_installer_gui.Utils;

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

        public async void OnSelected()
        {
            await Installer.Installer.InstallAugHip(this);
            ThemedMessageBoxUtil.Show("Successfully installed Augmented Hip!", "Install success!");
            Application.Current.Shutdown(0);
        }
    }
}
