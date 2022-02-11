using System;
using System.IO;
using Microsoft.Win32;

namespace aughip_installer_gui.Installer
{
    public static class Uninstaller
    {
        public static bool UninstallAugHip(string installPath)
        {
            try
            {
                // HACK: BAD!!! USE AN XML LIST!!!
                if (Directory.Exists(installPath))
                {
                    DirectoryInfo driverStuff = new DirectoryInfo(installPath);
                    foreach (FileInfo file in driverStuff.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        if (file.Name != "aughip-installer-gui.exe")
                            file.Delete();
                    }
                }

                // Yeet uninstaller
                string uninstallRegKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                using (RegistryKey parent = Registry.CurrentUser.OpenSubKey(uninstallRegKeyPath, true))
                {
                    if (parent == null)
                    {
                        string guidText = InstallerData.UninstallGuid.ToString("B").ToUpper();
                        var key = parent.OpenSubKey(guidText, true);
                        if (key != null)
                        {
                            parent.DeleteSubKey(guidText);
                        }
                    }
                }

                // Yeet traces of the current install
                using (RegistryKey parent = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
                {
                    RegistryKey key = parent.OpenSubKey(InstallerData.RegistryName, true);
                    if (key != null)
                    {
                        parent.DeleteSubKey(InstallerData.RegistryName);
                    }
                }

                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }
    }
}
