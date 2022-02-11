using System.Windows.Controls;

namespace aughip_installer_gui.Pages
{
    public partial class LandingPage : UserControl, IInstallerPage
    {
        public LandingPage()
        {
            InitializeComponent();
        }

        public void OnSelected()
        { }

        private void Install_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).GoToTab(1);
        }
    }
}

/*
background: linear-gradient(180deg, #FFB7B7 0%, #727272 100%), radial-gradient(60.91% 100% at 50% 0%, #FFD1D1 0%, #260000 100%), linear-gradient(238.72deg, #FFDDDD 0%, #720066 100%), linear-gradient(127.43deg, #00FFFF 0%, #FF4444 100%), radial-gradient(100.22% 100% at 70.57% 0%, #FF0000 0%, #00FFE0 100%), linear-gradient(127.43deg, #B7D500 0%, #3300FF 100%);
background-blend-mode: screen, overlay, hard-light, color-burn, color-dodge, normal;
 */