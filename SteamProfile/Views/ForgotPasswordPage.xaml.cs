using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SteamProfile.Views
{
    public sealed partial class ForgotPasswordPage : Page
    {
        private readonly ForgotPasswordViewModel _viewModel;
        
        public ForgotPasswordPage()
        {
            this.InitializeComponent();
            _viewModel = new ForgotPasswordViewModel(App.PasswordResetService);
            this.DataContext = _viewModel;
            
            // Subscribe to the password reset event to show the login button
            _viewModel.PasswordResetSuccess += OnPasswordResetSuccess;
        }
        
        private void OnPasswordResetSuccess(object sender, System.EventArgs e)
        {
            // Show the login button when password reset is successful
            GoToLoginButton.Visibility = Visibility.Visible;
        }
        
        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to login page
            this.Frame.Navigate(typeof(LoginPage));
        }
        
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to login page
            this.Frame.Navigate(typeof(LoginPage));
        }
    }
}
