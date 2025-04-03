using SteamProfile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using CommunityToolkit.Mvvm.Input;
using SteamProfile.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;
using System.ComponentModel.DataAnnotations;
using SteamProfile.Validators;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI.Xaml.Media.Animation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Windows.ApplicationModel.Contacts;

namespace SteamProfile.ViewModels
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly string _resetCodesPath;
        private string _email = string.Empty;
        private string _resetCode = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private Brush _statusColor;
        private bool _showEmailSection = true;
        private bool _showCodeSection = false;
        private bool _showPasswordSection = false;
        private bool _showLoginButton = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PasswordResetSuccess;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string ResetCode
        {
            get => _resetCode;
            set
            {
                _resetCode = value;
                OnPropertyChanged(nameof(ResetCode));
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public bool ShowEmailSection
        {
            get => _showEmailSection;
            set
            {
                _showEmailSection = value;
                OnPropertyChanged(nameof(ShowEmailSection));
            }
        }

        public bool ShowCodeSection
        {
            get => _showCodeSection;
            set
            {
                _showCodeSection = value;
                OnPropertyChanged(nameof(ShowCodeSection));
            }
        }

        public bool ShowPasswordSection
        {
            get => _showPasswordSection;
            set
            {
                _showPasswordSection = value;
                OnPropertyChanged(nameof(ShowPasswordSection));
            }
        }

        public bool ShowLoginButton
        {
            get => _showLoginButton;
            set
            {
                _showLoginButton = value;
                OnPropertyChanged(nameof(ShowLoginButton));
            }
        }

        public ICommand SendResetCodeCommand { get; }
        public ICommand VerifyCodeCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public ForgotPasswordViewModel(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
            _resetCodesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResetCodes");
            Directory.CreateDirectory(_resetCodesPath);
            _statusColor = new SolidColorBrush(Colors.Black);

            SendResetCodeCommand = new RelayCommand(() => SendResetCode(Email));
            VerifyCodeCommand = new RelayCommand(ExecuteVerifyCode);
            ResetPasswordCommand = new RelayCommand(ExecuteResetPassword);
        }

        public void SendResetCode(string email)
        {
            try
            {
                // Check if email is empty
                if (string.IsNullOrWhiteSpace(email))
                {
                    StatusMessage = "Please enter your email address.";
                    StatusColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                // Cleanup expired codes first
                _passwordResetService.CleanupExpiredCodes();

                var resetCode = _passwordResetService.GenerateResetCode(email);
                if (resetCode != null)
                {
                    string fileName = $"reset_code_{DateTime.Now:yyyyMMddHHmmss}.txt";
                    string filePath = Path.Combine(_resetCodesPath, fileName);
                    
                    File.WriteAllText(filePath, 
                        $"Reset Code for {email}:\n{resetCode}\n\nThis code will expire in 15 minutes.");

                    System.Diagnostics.Process.Start("notepad.exe", filePath);
                    
                    // Delete the file after a delay
                    Task.Delay(TimeSpan.FromMinutes(15)).ContinueWith(_ =>
                    {
                        try
                        {
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        catch { /* Ignore deletion errors */ }
                    });

                    ShowEmailSection = false;
                    ShowCodeSection = true;
                    StatusMessage = "Reset code has been generated and opened in a text file.";
                    StatusColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    // This is the important change: show an error when the email doesn't exist
                    StatusMessage = "Email address not found. Please check and try again.";
                    StatusColor = new SolidColorBrush(Colors.Red);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "An error occurred. Please try again later.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        private void ExecuteVerifyCode()
        {
            try
            {
                if (_passwordResetService.VerifyResetCode(Email, ResetCode))
                {
                    ShowCodeSection = false;
                    ShowPasswordSection = true;
                    StatusMessage = "Code verified successfully. Please enter your new password.";
                    StatusColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    StatusMessage = "Invalid or expired code.";
                    StatusColor = new SolidColorBrush(Colors.Red);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "An error occurred. Please try again later.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        private void ExecuteResetPassword()
        {
            try
            {
                // First validate the passwords match
                if (NewPassword != ConfirmPassword)
                {
                    StatusMessage = "Passwords do not match.";
                    StatusColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                // Then validate password requirements
                var isValid = UserValidator.IsPasswordValid(NewPassword);
                if (!isValid)
                {
                    StatusMessage = "Invalid password! Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one number, and one special character(@_.,/% ^#$!%*?&).";
                    StatusColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                // If validation passes, attempt to reset the password
                if (_passwordResetService.VerifyResetCode(Email, ResetCode))
                {
                    _passwordResetService.ResetPassword(Email, ResetCode, NewPassword);
                    _passwordResetService.CleanupExpiredCodes();
                    StatusMessage = "Password reset successful. You can now login with your new password.";
                    StatusColor = new SolidColorBrush(Colors.Green);

                    // Raise the password reset success event
                    PasswordResetSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    StatusMessage = "Invalid or expired reset code.";
                    StatusColor = new SolidColorBrush(Colors.Red);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "An error occurred. Please try again later.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        private (bool isValid, string errorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty.");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            if (!password.Any(char.IsUpper))
                return (false, "Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                return (false, "Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                return (false, "Password must contain at least one number.");

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return (false, "Password must contain at least one special character.");

            return (true, string.Empty);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
