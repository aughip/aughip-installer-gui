using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using aughip_installer_gui.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace aughip_installer_gui.Installer
{
    public static class Installer
    {
        public static void InstallAugHip()
        {
            if (InstallerData.ShouldInstallVCRedist)
            {
                InstallVCRedist();
            }

            if (!Directory.Exists(InstallerData.InstallPath))
                Directory.CreateDirectory(InstallerData.InstallPath);

            Utils.Utils.ExtractAchive(Path.Combine(InstallerData.DownloadDirectory, "augmented-hip.zip"), InstallerData.InstallPath);

            RegisterOpenVRDriver();

            RegisterUninstaller();
        }

        private static void RegisterOpenVRDriver()
        {
            // Ensure that SteamVR isn't running
            if (!TryKillingSteamVR())
            {
                ThemedMessageBoxUtil.Show("Install failed (Try closing SteamVR)");
                Application.Current.Shutdown(-1);
            }

            dynamic OpenVRpathsJson = JsonConvert.DeserializeObject(File.ReadAllText(InstallerData.OVRPaths));
            var externalDriversBlock = ((JArray)OpenVRpathsJson["external_drivers"]).ToObject<List<string>>();

            // Don't register duplicate drivers (polluting the drivers list in the process)!!
            if (!externalDriversBlock.Contains(InstallerData.InstallPath))
            {
                externalDriversBlock.Add(InstallerData.InstallPath);
                OpenVRpathsJson["external_drivers"] = JArray.FromObject(externalDriversBlock);
                string newOvrPaths = JsonConvert.SerializeObject(OpenVRpathsJson, Formatting.Indented);
                File.WriteAllText(InstallerData.OVRPaths, newOvrPaths);
            }
        }

        private static void InstallVCRedist()
        {
            try
            {
                Process.Start(Path.Combine(InstallerData.InstallPath, "vc_redist.x64.exe"), "/quiet /norestart").WaitForExit();
            }
            catch (Exception e)
            {
                // TODO: Log
                ThemedMessageBoxUtil.Show("Failed to install Microsoft Visual C++ Redistributable (You might have to manually install it yourself)");
            }
        }

        public static void RegisterUninstaller()
        {
            // Step 1: Copy the installer to the install directory
            string installerFullPath = Assembly.GetExecutingAssembly().Location;
            File.Copy(installerFullPath, Path.Combine(InstallerData.InstallPath, "aughip-installer-gui.exe"), true);

            // Step 2: Register the uninstaller
            using (RegistryKey parent = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
            {
                RegistryKey key = parent.OpenSubKey(InstallerData.RegistryName, true) ?? parent.CreateSubKey(InstallerData.RegistryName);
                key.SetValue("InstallPath", InstallerData.InstallPath);
            }

            string uninstallRegKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey parent = Registry.CurrentUser.OpenSubKey(uninstallRegKeyPath, true))
            {
                if (parent == null)
                {
                    // Logger.Log("Uninstall registry key not found.", isUserRelevant: false);
                    return;
                }
                try
                {
                    RegistryKey key = null;

                    try
                    {
                        string guidText = InstallerData.UninstallGuid.ToString("B").ToUpper();
                        key = parent.OpenSubKey(guidText, true) ??
                              parent.CreateSubKey(guidText);

                        if (key == null)
                        {
                            // Logger.Log(string.Format("Unable to create uninstaller '{0}\\{1}'", uninstallRegKeyPath, guidText), isUserRelevant: false);
                            return;
                        }

                        Assembly asm = Assembly.GetExecutingAssembly();

                        key.SetValue("DisplayName", "Augmented Hip");
                        key.SetValue("ApplicationVersion", "0.1");
                        key.SetValue("Publisher", asm.GetCustomAttribute<AssemblyCompanyAttribute>().Company);
                        key.SetValue("DisplayIcon", Path.Combine(InstallerData.InstallPath, "aughip-installer-gui.exe"));
                        key.SetValue("DisplayVersion", "0.1");
                        key.SetValue("URLInfoAbout", "https://k2vr.tech");
                        key.SetValue("Contact", "https://k2vr.tech");
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("UninstallString", Path.Combine(InstallerData.InstallPath, "aughip-installer-gui.exe") + " /uninstall");
                        key.SetValue("InstallLocation", InstallerData.InstallPath);
                        // https://stackoverflow.com/a/1765801/ and https://stackoverflow.com/a/22111211
                        key.SetValue("EstimatedSize", new DirectoryInfo(InstallerData.InstallPath).EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length) / 1024, RegistryValueKind.DWord);
                    }
                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }
                }
                catch (Exception)
                {
                    // Logger.Log("An error occurred writing uninstall information to the registry.  The service is fully installed but can only be uninstalled manually through the command line.", isUserRelevant: false); ;
                }
            }
        }

        /// <summary>
        /// Tries to kill SteamVR
        /// </summary>
        /// <returns>True if SteamVR is not running</returns>
        private static bool TryKillingSteamVR()
        {
            if (Utils.Utils.IsSteamVRRunning())
            {
                if (ThemedMessageBoxUtil.Show("The installer can't continue if SteamVR is running. Close SteamVR?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Utils.Utils.KillSteamVR();
                    if (Utils.Utils.IsSteamVRRunning())
                    {
                        return false;
                    }
                }
                else
                {
                    if (Utils.Utils.IsSteamVRRunning())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}