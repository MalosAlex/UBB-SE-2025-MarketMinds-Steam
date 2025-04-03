using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace SteamProfile.ViewModels.ConfigurationsViewModels
{
    public partial class ModifyProfileViewModel : ObservableObject
    {
        private int _userId;
        private string _originalImagePath = string.Empty;
        private string _originalDescription = string.Empty;
        private StorageFile _selectedImageFile;

        public ModifyProfileViewModel(Frame frame)
        {
            // Get current user ID (you might need to adjust how you get this)
            _userId = App.UserService.GetCurrentUser().UserId; // Assuming this exists

            // Load existing profile data
            LoadUserProfile();

            // Set initial save state
            UpdateCanSave();
        }

        private void LoadUserProfile()
        {
            var userProfile = App.UserProfileRepository.GetUserProfileByUserId(_userId);
            if (userProfile != null)
            {
                _originalImagePath = userProfile.ProfilePicture ?? string.Empty;
                _originalDescription = userProfile.Bio ?? string.Empty;

                // Set current values
                SelectedImagePath = _originalImagePath;
                SelectedImageName = !string.IsNullOrEmpty(_originalImagePath) ?
                    System.IO.Path.GetFileName(_originalImagePath) : "No image selected";
                Description = _originalDescription;
            }
        }

      

        [RelayCommand]
        private async Task ChooseNewPhotoAsync()
        {
            var filePicker = new FileOpenPicker();

            // Initialize the picker with the app window
            var window = new Window();
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, WinRT.Interop.WindowNative.GetWindowHandle(window));

            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");

            _selectedImageFile = await filePicker.PickSingleFileAsync();

            if (_selectedImageFile != null)
            {
                SelectedImageName = _selectedImageFile.Name;
                SelectedImagePath = _selectedImageFile.Path;
                UpdateCanSave();
            }
        }

        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            // Check if description has validation errors
            if (ValidateDescription())
            {
                bool changesMade = false;

                // Save new picture if changed
                if (_selectedImageFile != null && SelectedImagePath != _originalImagePath)
                {
                    App.UserProfileRepository.UpdateProfilePicture(_userId, _selectedImageFile.Path);
                    _originalImagePath = SelectedImagePath;
                    changesMade = true;
                }

                // Save new description if changed
                if (Description != _originalDescription)
                {
                    App.UserProfileRepository.UpdateProfileBio(_userId, Description);
                    _originalDescription = Description;
                    changesMade = true;
                }

                if (changesMade)
                {
                    SuccessMessage = "Your profile has been updated successfully!";
                    SuccessMessageVisibility = Visibility.Visible;

                    // Hide success message after a few seconds
                    await Task.Delay(3000);
                    SuccessMessageVisibility = Visibility.Collapsed;
                }

                UpdateCanSave();
            }
        }

        private bool ValidateDescription()
        {
            // Example validation - you can customize this
            if (Description.Length > 500)
            {
                DescriptionErrorMessage = "Description must be less than 500 characters.";
                DescriptionErrorVisibility = Visibility.Visible;
                return false;
            }

            DescriptionErrorVisibility = Visibility.Collapsed;
            DescriptionErrorMessage = string.Empty;
            return true;
        }

        private void UpdateCanSave()
        {
            // Enable save button if anything has changed
            CanSave = (Description != _originalDescription || SelectedImagePath != _originalImagePath)
                    && DescriptionErrorVisibility != Visibility.Visible;
        }

        partial void OnDescriptionChanged(string value)
        {
            ValidateDescription();
            UpdateCanSave();
        }

        [ObservableProperty]
        private string selectedImageName = string.Empty;

        [ObservableProperty]
        private string selectedImagePath = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string descriptionErrorMessage = string.Empty;

        [ObservableProperty]
        private Visibility descriptionErrorVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private bool canSave;

        [ObservableProperty]
        private string successMessage = string.Empty;

        [ObservableProperty]
        private Visibility successMessageVisibility = Visibility.Collapsed;
    }
}