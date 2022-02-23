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
        public static InstallerMode installerMode = InstallerMode.Install;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0 && e.Args[0] == "/uninstall") installerMode = InstallerMode.Uninstall;
            else if (e.Args.Length > 0 && e.Args[0] == "/silent") installerMode = InstallerMode.Uninstall;
            else if (e.Args.Length > 0 && e.Args[0] == "/hash") installerMode = InstallerMode.CalcHashes;

            string installPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\" + InstallerData.RegistryName, "InstallPath", "") ?? "";

            switch (installerMode)
            {
                case InstallerMode.Uninstall:
                    // We have to handle closing the app manually so that themed message dialogs dont explode
                    Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
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
                                    var proc = Process.Start(Info);

                                    // this should cause a race condition oops lmao
                                    // proc.WaitForExit();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ThemedMessageBoxUtil.Show(Localization.ERR_UNINSTALL_FAILED);
                        }
                    }

                    Current.Shutdown(0);
                    break;
                case InstallerMode.CalcHashes:

                    break;
            }
        }
    }
}
