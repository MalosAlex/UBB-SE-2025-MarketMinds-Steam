using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Models;
using SteamProfile.Services;
using SteamProfile.Views;
using System;
using System.Threading.Tasks;
using System.Linq;
using SteamProfile.Exceptions;
using SteamProfile.Validators;
using SteamProfile.Repositories;
using Windows.UI.WebUI;

namespace SteamProfile.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly WalletService _walletService;
        private readonly UserService _userService;
        private readonly UserProfilesRepository _userProfilesRepository;
        private readonly Frame _frame;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string errorMessage;

        // New property for developer status
        [ObservableProperty]
        private bool isDeveloper;

        public RegisterViewModel(Frame frame)
        {
            _walletService = App.WalletService;
            _userService = App.UserService;
            _userProfilesRepository = App.UserProfileRepository;
            _frame = frame;
        }

        [RelayCommand]
        private async Task Register()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "All fields are required.";
                    return;
                }

                // Validate email format
                if (!UserValidator.IsEmailValid(Email))
                {
                    ErrorMessage = "Invalid email.";
                    return;
                }

                // Ensure email and username are unique
                _userService.ValidateUserAndEmail(Email, Username);

                // Validate the password
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return;
                }

                if (!UserValidator.IsPasswordValid(Password))
                {
                    ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one number, and one special character (@_.,/%^#$!%*?&).";
                    return;
                }

                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    IsDeveloper = IsDeveloper
                };


                var createdUser = _userService.CreateUser(user);

                if (createdUser != null)
                {
                    // Navigate to login page on successful registration
                    _userProfilesRepository.CreateProfile(createdUser.UserId);
                    _walletService.CreateWallet(createdUser.UserId);
                    _frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    ErrorMessage = "Failed to create account. Please try again.";
                }
            }
            catch (EmailAlreadyExistsException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (UsernameAlreadyTakenException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            _frame.Navigate(typeof(LoginPage));
        }
    }
} 