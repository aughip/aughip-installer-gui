using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using aughip_installer_gui.Installer;

namespace aughip_installer_gui.Pages
{
    public partial class DownloadStatus : UserControl, IInstallerPage
    {
        UpdaterModule currentModule = UpdaterModule.VCRedist;

        public DownloadStatus()
        {
            InitializeComponent();
        }

        public async void DownloadFile()
        {

        }

        public async void ProgressUpdate(float value)
        {
            switch (currentModule)
            {
                case UpdaterModule.VCRedist:
                    vcRedistProgress.ProgressValue = 100.0 * value;
                    break;
                case UpdaterModule.AugHip:
                    augHipProgress.ProgressValue = 100.0 * value;
                    break;
            }
            // System.Console.WriteLine(value);
        }

        public async void OnSelected()
        {
            // Ensure VC Redist is installed
            InstallerData.ShouldInstallVCRedist = !Utils.Utils.IsVCRedistInstalled();
            if (InstallerData.ShouldInstallVCRedist)
            {
                if (!await Utils.Utils.DownloadSafely(InstallerData.VCRedistRemote, Path.Combine(InstallerData.DownloadDirectory, "vc_redist.x64.exe"), ProgressUpdate))
                {
                    Utils.ThemedMessageBoxUtil.Show("Failed to download VCRedist! Join the Discord (https://k2vr.tech) for help!");
                }
            }
            vcRedistProgress.ProgressValue = 100.0;
            await Task.Delay(10);

            if (!await Utils.Utils.DownloadSafely(InstallerData.AugHipRemote, Path.Combine(InstallerData.DownloadDirectory, "augmented-hip.zip"), ProgressUpdate))
            {
                Utils.ThemedMessageBoxUtil.Show("Failed to download AugHip! Join the Discord (https://k2vr.tech) for help!");
            }
            augHipProgress.ProgressValue = 100.0;
            await Task.Delay(10);

            // Install!
            ((MainWindow)App.Current.MainWindow).GoToTab(2);
        }
    }
}
