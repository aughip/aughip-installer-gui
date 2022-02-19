using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using aughip_installer_gui.Installer;
using aughip_installer_gui.Utils;
using Localization = aughip_installer_gui.Properties.Resources;
using Microsoft.Win32;

namespace aughip_installer_gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool isUninstall = false;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0 && e.Args[0] == "/uninstall") isUninstall = true;

            string installPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\" + InstallerData.RegistryName, "InstallPath", "") ?? "";

            if (isUninstall)
            {
                if (ThemedMessageBoxUtil.Show(Localization.PROMPT_UNINSTALL, Localization.modal_confirm, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (Uninstaller.UninstallAugHip(installPath))
                        {
                            ThemedMessageBoxUtil.Show(Localization.UNINSTALL_SUCCESS);
                            if (Directory.Exists(installPath))
                            {
                                // self YEET
                                // https://stackoverflow.com/a/1305478/
                                ProcessStartInfo Info = new ProcessStartInfo();
                                Info.Arguments = "/C choice /C Y /N /D Y /T 3 & rmdir /S /Q \"" + installPath + "\"";
                                Info.WindowStyle = ProcessWindowStyle.Hidden;
                                Info.CreateNoWindow = true;
                                Info.FileName = "cmd.exe";
                                Process.Start(Info);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ThemedMessageBoxUtil.Show(Localization.ERR_UNINSTALL_FAILED);
                        // new ExceptionDialog(ex).ShowDialog();
                    }
                }

                Current.Shutdown(0);
            }
        }
    }
}
