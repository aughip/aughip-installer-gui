using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace aughip_installer_gui.Utils
{
    public static partial class Utils
    {
        /// <summary>
        /// Returns whether VC Redist 2015-2019 is installed on the current system
        /// </summary>
        /// <returns></returns>
        public static bool IsVCRedistInstalled()
        {
            int vcRedistInstalled =
                (int)(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\X64",
                           "Installed", null) ??
                       Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\X64", "Installed", 0));

            return vcRedistInstalled == 1;
        }

        public static void ExtractAchive(string archivePath, string extractPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string extractedFilePath = Path.Combine(extractPath, entry.FullName);
                    if (!Directory.Exists(Path.GetDirectoryName(extractedFilePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(extractedFilePath));
                    entry.ExtractToFile(extractedFilePath, true);
                }
            }
        }

        // https://stackoverflow.com/questions/44935273/
        public static bool DownloadSafely(string url, string filePath, bool autoResume = true)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            long totalBytesRead = 0;
            long MaxContentLength = 0;
            long RequestContentLength = 0;
            int i = 0;

            while (MaxContentLength == 0 || totalBytesRead < MaxContentLength)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                if (totalBytesRead > 0)
                {
                    request.AddRange(totalBytesRead);
                }

                WebResponse response = request.GetResponse();

                Console.WriteLine("=============== Request #{0} ==================", ++i);
                for (var j = 0; j < response.Headers.Count; j++)
                {
                    var header = response.Headers.Get(j);
                    if (header.Contains("Content-Length") || header.Contains("Content-Range"))
                    {
                        Console.WriteLine("{0}: {1}", header, response.Headers[header.ToString()]);
                    }
                }

                if (response.ContentLength > MaxContentLength)
                {
                    MaxContentLength = response.ContentLength;
                }

                var ns = response.GetResponseStream();
                RequestContentLength = 0;
                try
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (FileStream localFileStream = new FileStream(filePath, FileMode.Append))
                        {
                            var buffer = new byte[4096];
                            int bytesRead;

                            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                totalBytesRead += bytesRead;
                                RequestContentLength += bytesRead;
                                localFileStream.Write(buffer, 0, bytesRead);
                            }

                            Console.WriteLine("Got bytes: {0}", RequestContentLength);
                        }

                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Got bytes: {0}", RequestContentLength);
                }
            }

            if (MaxContentLength == totalBytesRead)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether SteamVR is running
        /// </summary>
        /// <returns>true if SteamVR is running</returns>
        public static bool IsSteamVRRunning()
        {
            if (Process.GetProcesses().FirstOrDefault((Process proc) => proc.ProcessName == "vrserver" || proc.ProcessName == "vrmonitor") == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries killing SteamVR
        /// </summary>
        /// <returns>True if SteamVR has successfully been killed</returns>
        public static bool KillSteamVR()
        {
            // Close SteamVR Dashboard
            foreach (Process process in Process.GetProcesses().Where((proc) => proc.ProcessName == "vrmonitor"))
            {
                process.CloseMainWindow();
                Thread.Sleep(5000);
                if (!process.HasExited)
                {
                    /* When SteamVR is open with no headset detected,
                        CloseMainWindow will only close the "headset not found" popup
                        so we kill it, if it's still open */
                    process.Kill();
                    Thread.Sleep(3000);
                }
            }

            // Close VrServer
            // Apparently, SteamVR server can run without the monitor,
            // so we close that, if it's open as well (monitor will complain if you close server first)
            foreach (Process process in Process.GetProcesses().Where((proc) => proc.ProcessName == "vrserver"))
            {
                // CloseMainWindow won't work here because it doesn't have a window
                process.Kill();
                Thread.Sleep(5000);
                if (!process.HasExited)
                {
                    return false;
                }
            }

            return true;
        }


        // Darkmode Title bar... not fun for linux probably
        // https://stackoverflow.com/a/64927217
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        public static void EnableDarkTitleBar(this Window window)
        {
            var Handle = new WindowInteropHelper(window).Handle;

            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
    }
}