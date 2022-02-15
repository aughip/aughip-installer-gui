using System;
using System.Windows;

namespace aughip_installer_gui.Popups
{
    /// <summary>
    /// Interaction logic for ThemedMessageBox.xaml
    /// </summary>
    public partial class ThemedMessageBox : Window
    {
        private MessageBoxButton buttonBehavior;
        private string messageBoxText, caption;

        public MessageBoxResult result;

        public ThemedMessageBox(string messageBoxText, string caption, MessageBoxButton buttons)
        {
            InitializeComponent();

            this.messageBoxText = messageBoxText;
            this.caption = caption;
            this.buttonBehavior = buttons;

            // Apply the dialog text
            Title = caption;
            description.Text = messageBoxText;

            // Hide and change the text of the buttons accordingly
            switch (buttonBehavior)
            {
                case MessageBoxButton.OK:
                    button_Right.Content = "OK";
                    button_Right.IsDefault = true;
                    button_Middle.Visibility = Visibility.Collapsed;
                    button_Left.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    button_Right.Content = "Cancel";
                    button_Right.IsCancel = true;
                    button_Middle.Content = "OK";
                    button_Middle.IsDefault = true;
                    button_Left.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    button_Right.Content = "No";
                    button_Right.IsCancel = true;
                    button_Middle.Content = "Yes";
                    button_Middle.IsDefault = true;
                    button_Left.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    button_Right.Content = "Cancel";
                    button_Right.IsCancel = true;
                    button_Middle.Content = "No";
                    button_Left.Content = "Yes";
                    button_Left.IsDefault = true;
                    break;
            }

            result = MessageBoxResult.None;
        }

        // Button behavior
        private void button_Right_Click(object sender, RoutedEventArgs e)
        {
            switch (buttonBehavior)
            {
                case MessageBoxButton.OK:
                    result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.No;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Cancel;
                    break;
            }
            this.Close();
        }

        private void button_Middle_Click(object sender, RoutedEventArgs e)
        {
            switch (buttonBehavior)
            {
                default:
                    result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.Yes;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.No;
                    break;
            }
            this.Close();
        }

        private void button_Left_Click(object sender, RoutedEventArgs e)
        {
            switch (buttonBehavior)
            {
                default:
                    result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Yes;
                    break;
            }
            this.Close();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Utils.Utils.EnableDarkTitleBar(this);
        }
    }
}
