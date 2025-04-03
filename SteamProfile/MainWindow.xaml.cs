using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using System;
using SteamProfile.Services;

namespace SteamProfile
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            NavigationService.Instance.Initialize(ContentFrame); // Pass the Frame directly !!!!!!!!!!!

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            ContentFrame.Navigate(typeof(LoginPage));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            ContentFrame.Navigate(typeof(RegisterPage));
        }
    }
}