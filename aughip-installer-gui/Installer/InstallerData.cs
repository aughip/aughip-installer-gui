using System;
using System.IO;

namespace aughip_installer_gui.Installer
{
    public static class InstallerData
    {
        public static Guid UninstallGuid = new Guid("6c974080-b7c4-4dfb-a18e-c83defb1db7e"); // this was randomly generated

        public const string RegistryName = "AugmentedHip";

        // OpenVR consts
        public static readonly string OVRPaths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openvr", "openvrpaths.vrpath");

        // TODO: XML Array for remote files
        // TODO: File hashing for verification
        public const string VCRedistRemote = "https://aka.ms/vs/17/release/vc_redist.x64.exe";
        public const string AugHipRemote = "https://github.com/hyblocker/augmented-hip/releases/download/0.1/augmented-hip.zip";
        public static readonly string DownloadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aughip-installer");

        // TODO: Allow the user to change this in a revised installer version
        public static readonly string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "augmented-hip");

        public static bool ShouldInstallVCRedist = true;
    }
}
