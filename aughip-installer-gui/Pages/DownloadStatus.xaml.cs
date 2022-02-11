using System.IO;
using System.Windows.Controls;
using aughip_installer_gui.Installer;

namespace aughip_installer_gui.Pages
{
    public partial class DownloadStatus : UserControl, IInstallerPage
    {
        public DownloadStatus()
        {
            InitializeComponent();
        }

        public void OnSelected()
        {
            // Ensure VC Redist is installed
            InstallerData.ShouldInstallVCRedist = !Utils.Utils.IsVCRedistInstalled();
            if (InstallerData.ShouldInstallVCRedist)
            {
                if (!Utils.Utils.DownloadSafely(InstallerData.VCRedistRemote, Path.Combine(InstallerData.DownloadDirectory, "vc_redist.x64.exe")))
                {
                    throw new System.Exception("Failed to download VCRedist! Join the Discord (https://k2vr.tech) for help!");
                }
            }

            if (!Utils.Utils.DownloadSafely(InstallerData.AugHipRemote, Path.Combine(InstallerData.DownloadDirectory, "augmented-hip.zip")))
            {
                throw new System.Exception("Failed to download AugHip! Join the Discord (https://k2vr.tech) for help!");
            }

            // Install!
            ((MainWindow)App.Current.MainWindow).GoToTab(2);
        }
    }
}