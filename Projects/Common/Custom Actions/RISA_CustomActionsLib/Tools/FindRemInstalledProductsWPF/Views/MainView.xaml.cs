using FindRemInstalledProductsWPF.Helpers;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Media;

namespace FindRemInstalledProductsWPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
        }
        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            // still need to set BaseLight or BaseDark in App.xaml
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManagerHelper.CreateAppStyleBy(Colors.LimeGreen, true);
        }
    }
}
