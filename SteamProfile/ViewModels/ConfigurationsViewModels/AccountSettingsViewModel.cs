using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Models;
using SteamProfile.Services;
using SteamProfile.Views;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamProfile.ViewModels.ConfigurationsViewModels
{
    public partial class AccountSettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _successMessage = string.Empty;

        [ObservableProperty]
        private string _passwordConfirmationError = string.Empty;

        [ObservableProperty]
        private Visibility _emailErrorMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _passwordErrorMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _usernameErrorMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _passwordConfirmationErrorVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _successMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private bool _updateEmailEnabled = false;

        [ObservableProperty]
        private bool _updateUsernameEnabled = false;

        [ObservableProperty]
        private bool _updatePasswordEnabled = false;

        // Event to request the View to show the password confirmation dialog
        public event EventHandler RequestPasswordConfirmation;

        private readonly UserService _userService;
        private Action _pendingAction;

        public AccountSettingsViewModel()
        {
            _userService = App.UserService;

            var currentUser = _userService.GetCurrentUser();
            if (currentUser != null)
            {
                _username = currentUser.Username;
                _email = currentUser.Email;
            }
        }

        partial void OnPasswordChanged(string value)
        {
            ValidatePassword(value);
        }

        private void ValidatePassword(string password)
        {
            bool isValid = IsValidPassword(password);
            PasswordErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdatePasswordEnabled = isValid;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$");
            return regex.IsMatch(password);
        }

        partial void OnEmailChanged(string value)
        {
            ValidateEmail(value);
        }

        private void ValidateEmail(string email)
        {
            bool isValid = IsValidEmail(email);
            EmailErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdateEmailEnabled = isValid;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email) && _userService.GetUserByEmail(email) == null;
        }

        partial void OnUsernameChanged(string value)
        {
            ValidateUsername(value);
        }

        private void ValidateUsername(string username)
        {
            bool isValid = IsValidUsername(username);
            UsernameErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdateUsernameEnabled = isValid;
        }

        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return false;
            return _userService.GetUserByUsername(username) == null; // Username should be unique
        }

        [RelayCommand]
        private void UpdateUsername()
        {
            _pendingAction = () =>
            {
                if (_userService.UpdateUserUsername(Username, CurrentPassword))
                {
                    ShowSuccessMessage("Username updated successfully!");
                }
                else
                {
                    ErrorMessage = "Failed to update username. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void UpdateEmail()
        {
            _pendingAction = () =>
            {
                if (_userService.UpdateUserEmail(Email, CurrentPassword))
                {
                    ShowSuccessMessage("Email updated successfully!");
                }
                else
                {
                    ErrorMessage = "Failed to update email. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void UpdatePassword()
        {
            _pendingAction = () =>
            {
                if (_userService.UpdateUserPassword(Password, CurrentPassword))
                {
                    ShowSuccessMessage("Password updated successfully!");
                    Password = string.Empty; // Clear the password field
                }
                else
                {
                    ErrorMessage = "Failed to update password. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Logout()
        {
            _userService.Logout();
            NavigationService.Instance.Navigate(typeof(LoginPage));
        }

        [RelayCommand]
        private void DeleteAccount()
        {
            _pendingAction = () =>
            {
                _userService.DeleteUser(_userService.GetCurrentUser().UserId);

                NavigationService.Instance.Navigate(typeof(LoginPage));

            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidateCurrentPassword()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ErrorMessage = "Current password is required.";
                return false;
            }
            return _userService.VerifyUserPassword(CurrentPassword);
        }

        private void ShowSuccessMessage(string message)
        {
            SuccessMessage = message;
            SuccessMessageVisibility = Visibility.Visible;

            // Hide success message after 3 seconds
            Task.Delay(3000).ContinueWith(_ =>
            {
                SuccessMessageVisibility = Visibility.Collapsed;
                SuccessMessage = string.Empty;
            });
        }

        public bool VerifyPassword(string password)
        {
            return _userService.VerifyUserPassword(password);
        }

        public void ExecutePendingAction()
        {
            if (_pendingAction != null)
            {
                _pendingAction();
                _pendingAction = null;
            }
            CurrentPassword = string.Empty;
        }

        public void CancelPendingAction()
        {
            _pendingAction = null;
            CurrentPassword = string.Empty;
        }
    }
}