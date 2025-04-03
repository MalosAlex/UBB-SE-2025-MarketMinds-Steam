using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Models;
using SteamProfile.Services;
using SteamProfile.Views;
using System;
using System.Threading.Tasks;

namespace SteamProfile.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly Frame _frame;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        public LoginViewModel(Frame frame)
        {
            _userService = App.UserService;
            _frame = frame;
        }

        [RelayCommand]
        private async Task Login()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter both username and password.";
                    return;
                }

                var user = _userService.Login(Username, Password);
                if (user != null)
                {
                    // Navigate to profile page after successful login
                    _frame.Navigate(typeof(ProfilePage), user.UserId);  // !!!!!!!
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            _frame.Navigate(typeof(RegisterPage));
        }

        [RelayCommand]
        private void NavigateToForgotPassword()
        {
            _frame.Navigate(typeof(ForgotPasswordPage));
        }
    }
}
