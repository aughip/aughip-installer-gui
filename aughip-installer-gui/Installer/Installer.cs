using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using aughip_installer_gui.Pages;
using aughip_installer_gui.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace aughip_installer_gui.Installer
{
    public static class Installer
    {
        const int animationDelayInMillis = 10;

        public static async Task InstallAugHip(InstallPage installerGUI = null)
        {

            if (installerGUI != null)
            {
                installerGUI.vcredistProgress.CurrentState = InstallState.Installing;
                installerGUI.installAughipProgress.CurrentState = InstallState.Waiting;
                installerGUI.ovrProgress.CurrentState = InstallState.Waiting;
                installerGUI.uninstallSetupProgress.CurrentState = InstallState.Waiting;

                await Task.Delay(animationDelayInMillis);
            }

            if (InstallerData.ShouldInstallVCRedist)
            {
                InstallVCRedist();
            }

            if (installerGUI != null)
            {
                installerGUI.vcredistProgress.CurrentState = InstallState.Done;
                installerGUI.installAughipProgress.CurrentState = InstallState.Installing;
                installerGUI.ovrProgress.CurrentState = InstallState.Waiting;
                installerGUI.uninstallSetupProgress.CurrentState = InstallState.Waiting;

                await Task.Delay(animationDelayInMillis);
            }

            if (!Directory.Exists(InstallerData.InstallPath))
                Directory.CreateDirectory(InstallerData.InstallPath);

            Utils.Utils.ExtractAchive(Path.Combine(InstallerData.DownloadDirectory, "augmented-hip.zip"), InstallerData.InstallPath);


            if (installerGUI != null)
            {
                installerGUI.vcredistProgress.CurrentState = InstallState.Done;
                installerGUI.installAughipProgress.CurrentState = InstallState.Done;
                installerGUI.ovrProgress.CurrentState = InstallState.Installing;
                installerGUI.uninstallSetupProgress.CurrentState = InstallState.Waiting;

                await Task.Delay(animationDelayInMillis);
            }

            RegisterOpenVRDriver();


            if (installerGUI != null)
            {
                installerGUI.vcredistProgress.CurrentState = InstallState.Done;
                installerGUI.installAughipProgress.CurrentState = InstallState.Done;
                installerGUI.ovrProgress.CurrentState = InstallState.Done;
                installerGUI.uninstallSetupProgress.CurrentState = InstallState.Installing;

                await Task.Delay(animationDelayInMillis);
            }

            RegisterUninstaller();

            if (installerGUI != null)
            {
                installerGUI.vcredistProgress.CurrentState = InstallState.Done;
                installerGUI.installAughipProgress.CurrentState = InstallState.Done;
                installerGUI.ovrProgress.CurrentState = InstallState.Done;
                installerGUI.uninstallSetupProgress.CurrentState = InstallState.Done;

                await Task.Delay(animationDelayInMillis);
            }
        }

        private static void RegisterOpenVRDriver()
        {
            // Ensure that SteamVR isn't running
            if (!TryKillingSteamVR())
            {
                ThemedMessageBoxUtil.Show(Properties.Resources.ERR_INSTALL_FAIL_STEAMVR_RUNNING);
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
                ThemedMessageBoxUtil.Show(Properties.Resources.ERR_INSTALL_FAIL_VCREDIST_FAILED);
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
                if (ThemedMessageBoxUtil.Show(Properties.Resources.PROMPT_CLOSE_STEAMVR, Properties.Resources.modal_confirm, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
