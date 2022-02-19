using System;
using System.Collections.Generic;

namespace aughip_installer_gui.Utils
{
    public static class Logger
    {
        private const string logFile = "installer-log.txt";
        internal static List<string> logBuffer;

        static Logger()
        {
            // Init Logger
            logBuffer = new List<string>();

        }

        public static void Info(string message)
        {
            Console.WriteLine(message);
        }

        public static void Info(string message, params object[] args)
        {
            Console.WriteLine(string.Format(message, args));
        }
    }
}
