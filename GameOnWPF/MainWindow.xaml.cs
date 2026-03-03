using System.Windows;
using System.Windows.Controls;
using GameOnWPF.Components; // for GameBuildSettings

namespace GameOnWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowUserControl(UserControl control)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            MainContent.Content = control;
        }

        public void ShowMainMenu()
        {
            MainContent.Content = null;
            MainMenuGrid.Visibility = Visibility.Visible;
        }

        private void OnGameBuildClick(object sender, RoutedEventArgs e)
        {
            ShowUserControl(new GameScene(this));
        }

        private void OnExitGameClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
