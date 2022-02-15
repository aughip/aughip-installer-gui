using System.Windows;
using aughip_installer_gui.Popups;

namespace aughip_installer_gui.Utils
{
    public static class ThemedMessageBoxUtil
    {
        public static MessageBoxResult Show(string messageBoxText)
        {
            return Show(messageBoxText, string.Empty, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            var modalWindow = new ThemedMessageBox(messageBoxText, caption, button);
            modalWindow.Owner = Application.Current.MainWindow;
            modalWindow.ShowDialog();

            return modalWindow.result;
        }
    }
}
