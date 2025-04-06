using BusinessLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BusinessLayer.Models;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.Input;
using SteamProfile.Views;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Imaging;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public class FeatureDisplay : INotifyPropertyChanged
    {
        private readonly Feature _feature;
        private readonly bool _isPurchased;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public FeatureDisplay(Feature feature, bool isPurchased)
        {
            _feature = feature;
            _isPurchased = isPurchased;
        }

        public string Name => _feature?.Name ?? "Unknown";
        public string Source
        {
            get
            {
                if (_feature == null)
                {
                    return string.Empty;
                }

                return $"ms-appx:///{_feature.Source}";
            }
        }
        public string DisplayValue { get { return "Value: " + _feature.Value.ToString(); } }
        public bool Equipped => _feature.Equipped;
        public bool IsPurchased => _isPurchased;
        public int FeatureId => _feature.FeatureId;
        public string Type => _feature.Type;
        
        // New properties for UI
        public double Opacity => IsPurchased ? 1.0 : 0.5;
        public Brush BorderBrush => IsPurchased ? new SolidColorBrush(Colors.DodgerBlue) : new SolidColorBrush(Colors.Transparent);
        public Visibility LockIconVisibility => IsPurchased ? Visibility.Collapsed : Visibility.Visible;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FeaturesViewModel : INotifyPropertyChanged
    {
        private readonly FeaturesService _featuresService;
        private readonly IUserService _userService;
        private readonly UserProfilesRepository _userProfilesRepository;
        private string _statusMessage = string.Empty;
        private Brush _statusColor;
        private FeatureDisplay _selectedFeature;
        private XamlRoot _xamlRoot;
        private readonly Frame _frame;

        public IRelayCommand ShowOptionsCommand { get; }
        
        public FeatureDisplay SelectedFeature
        {
            get => _selectedFeature;
            set
            {
                _selectedFeature = value;
                OnPropertyChanged(nameof(SelectedFeature));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FeatureDisplay> Frames { get; } = new();
        public ObservableCollection<FeatureDisplay> Emojis { get; } = new();
        public ObservableCollection<FeatureDisplay> Backgrounds { get; } = new();
        public ObservableCollection<FeatureDisplay> Pets { get; } = new();
        public ObservableCollection<FeatureDisplay> Hats { get; } = new();

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

        public static event EventHandler<int> FeatureEquipStatusChanged;

        public FeaturesViewModel(FeaturesService featuresService, Frame frame)
        {
            _featuresService = featuresService;
            _userService = featuresService.UserService;
            _userProfilesRepository = App.UserProfileRepository;
            _frame = frame;
            _statusColor = new SolidColorBrush(Colors.Black);
            ShowOptionsCommand = new RelayCommand<FeatureDisplay>(ShowOptions);
            LoadFeatures();
        }

        public void Initialize(XamlRoot xamlRoot)
        {
            _xamlRoot = xamlRoot;
        }

        private void LoadFeatures()
        {
            try
            {
                var features = _featuresService.GetFeaturesByCategories();
                
                UpdateCollection(Frames, features.GetValueOrDefault("frame", new List<Feature>()));
                UpdateCollection(Emojis, features.GetValueOrDefault("emoji", new List<Feature>()));
                UpdateCollection(Backgrounds, features.GetValueOrDefault("background", new List<Feature>()));
                UpdateCollection(Pets, features.GetValueOrDefault("pet", new List<Feature>()));
                UpdateCollection(Hats, features.GetValueOrDefault("hat", new List<Feature>()));

                // Remove success message
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to load features. Please try again later.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        private async void ShowOptions(FeatureDisplay feature)
        {
            if (feature == null || _xamlRoot == null) return;

            SelectedFeature = feature;
            
            var dialog = new ContentDialog
            {
                XamlRoot = _xamlRoot
            };
            var buttons = new StackPanel { Spacing = 10 };

            if (!feature.IsPurchased)
            {
                var buyButton = new Button 
                { 
                    Content = "Buy",
                    Style = Application.Current.Resources["AccentButtonStyle"] as Style,
                    Tag = feature.FeatureId // Store the feature ID in the Tag property
                };
                buyButton.Click += BuyButton_Click;
                buttons.Children.Add(buyButton);
            }
            else // Feature is purchased
            {
                if (feature.Equipped)
                {
                    var unequipButton = new Button 
                    { 
                        Content = "Unequip",
                        Style = Application.Current.Resources["AccentButtonStyle"] as Style
                    };
                    unequipButton.Click += (s, e) => 
                    {
                        try
                        {
                            // Call service to unequip feature
                            bool success = _featuresService.UnequipFeature(
                                _userService.GetCurrentUser().UserId, 
                                feature.FeatureId);
                            
                            if (success)
                            {
                                dialog.Hide();
                                StatusMessage = $"{feature.Name} unequipped successfully";
                                StatusColor = new SolidColorBrush(Colors.Green);
                                
                                // Refresh features list to update UI
                                LoadFeatures();
                            }
                            else
                            {
                                StatusMessage = "Failed to unequip feature";
                                StatusColor = new SolidColorBrush(Colors.Red);
                            }
                        }
                        catch (Exception ex)
                        {
                            StatusMessage = $"Error: {ex.Message}";
                            StatusColor = new SolidColorBrush(Colors.Red);
                        }
                    };
                    buttons.Children.Add(unequipButton);
                }
                else
                {
                    var equipButton = new Button 
                    { 
                        Content = "Equip",
                        Style = Application.Current.Resources["AccentButtonStyle"] as Style
                    };
                    equipButton.Click += (s, e) => 
                    {
                        try
                        {
                            // Call service to equip feature
                            bool success = _featuresService.EquipFeature(
                                _userService.GetCurrentUser().UserId, 
                                feature.FeatureId);
                            
                            if (success)
                            {
                                dialog.Hide();
                                StatusMessage = $"{feature.Name} equipped successfully";
                                StatusColor = new SolidColorBrush(Colors.Green);
                                
                                // Refresh features list to update UI
                                LoadFeatures();
                            }
                            else
                            {
                                StatusMessage = "Failed to equip feature";
                                StatusColor = new SolidColorBrush(Colors.Red);
                            }
                        }
                        catch (Exception ex)
                        {
                            StatusMessage = $"Error: {ex.Message}";
                            StatusColor = new SolidColorBrush(Colors.Red);
                        }
                    };
                    buttons.Children.Add(equipButton);
                }
            }

            var previewButton = new Button 
            { 
                Content = "Preview",
                Style = Application.Current.Resources["DefaultButtonStyle"] as Style
            };
            previewButton.Click += async (s, e) => 
            {
                // Close the options dialog
                dialog.Hide();
                
                // Show the preview dialog
                await ShowPreviewDialog(feature);
            };
            buttons.Children.Add(previewButton);

            dialog.Content = buttons;
            dialog.Title = feature.Name;
            dialog.CloseButtonText = "Cancel";

            await dialog.ShowAsync();
        }

        // Event handler for Buy button click
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Close the dialog
                if (button.Parent is StackPanel panel && 
                    panel.Parent is ContentDialog dialog)
                {
                    dialog.Hide();
                }

                // Trigger event for navigation
                NavigateToShopRequested?.Invoke(this, new NavigationEventArgs { FeatureId = (int)button.Tag });
            }
        }

        // Event to request navigation to the Shop page
        public event EventHandler<NavigationEventArgs> NavigateToShopRequested;

        // Navigation event args class
        public class NavigationEventArgs : EventArgs
        {
            public int FeatureId { get; set; }
        }

        private async Task ShowPreviewDialog(FeatureDisplay featureDisplay)
        {
            // Get current user and equipped features
            var user = _userService.GetCurrentUser();
            var userFeatures = _featuresService.GetUserEquippedFeatures(user.UserId);
            
            // Get the user profile to access bio and profile picture
            var userProfile = _userProfilesRepository.GetUserProfileByUserId(user.UserId);
            
            // Create adaptive profile control
            var profileControl = new AdaptiveProfileControl();
            
            // Get profile picture path
            string profilePicturePath = "ms-appx:///Assets/default-profile.png";
            if (userProfile != null && !string.IsNullOrEmpty(userProfile.ProfilePicture))
            {
                profilePicturePath = userProfile.ProfilePicture;
                if (!profilePicturePath.StartsWith("ms-appx:///"))
                {
                    profilePicturePath = $"ms-appx:///{profilePicturePath}";
                }
            }
            
            // Get bio text
            string bioText = "No bio available";
            if (userProfile != null && !string.IsNullOrEmpty(userProfile.Bio))
            {
                bioText = userProfile.Bio;
            }
            
            // Extract feature paths from equipped features
            string hatPath = null;
            string petPath = null;
            string emojiPath = null;
            string framePath = null;
            string backgroundPath = null;
            
            foreach (var feature in userFeatures)
            {
                if (!feature.Equipped) continue;
                
                string path = feature.Source;
                if (!path.StartsWith("ms-appx:///"))
                {
                    path = $"ms-appx:///{path}";
                }
                
                switch (feature.Type.ToLower())
                {
                    case "hat": hatPath = path; break;
                    case "pet": petPath = path; break;
                    case "emoji": emojiPath = path; break;
                    case "frame": framePath = path; break;
                    case "background": backgroundPath = path; break;
                }
            }
            
            // Apply the preview feature
            string previewPath = featureDisplay.Source;
            switch (featureDisplay.Type.ToLower())
            {
                case "hat": hatPath = previewPath; break;
                case "pet": petPath = previewPath; break;
                case "emoji": emojiPath = previewPath; break;
                case "frame": framePath = previewPath; break;
                case "background": backgroundPath = previewPath; break;
            }
            
            // Update the profile control with all the information
            profileControl.UpdateProfile(
                user.Username, 
                bioText, 
                profilePicturePath,
                hatPath, 
                petPath, 
                emojiPath, 
                framePath, 
                backgroundPath
            );
            
            // Adjust size for the dialog
            profileControl.Width = 350;
            profileControl.Height = 500;
            
            // Create and show preview dialog
            var previewDialog = new ContentDialog
            {
                XamlRoot = _xamlRoot,
                Title = "Profile Preview",
                Content = profileControl,
                CloseButtonText = "Close"
            };
            
            await previewDialog.ShowAsync();
        }

        private void UpdateCollection(ObservableCollection<FeatureDisplay> collection, List<Feature> newItems)
        {
            collection.Clear();
            foreach (var item in newItems)
            {
                var isPurchased = _featuresService.IsFeaturePurchased(_userService.GetCurrentUser().UserId, item.FeatureId);
                collection.Add(new FeatureDisplay(item, isPurchased));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool EquipFeature(int featureId)
        {
            try
            {
                // Call service to equip feature
                bool success = _featuresService.EquipFeature(
                    _userService.GetCurrentUser().UserId, 
                    featureId);
                
                if (success)
                {
                    // Notify that a feature was equipped
                    FeatureEquipStatusChanged?.Invoke(this, _userService.GetCurrentUser().UserId);
                    
                    StatusMessage = "Feature equipped successfully";
                    StatusColor = new SolidColorBrush(Colors.Green);
                    
                    // Refresh features list to update UI
                    LoadFeatures();
                }
                else
                {
                    StatusMessage = "Failed to equip feature";
                    StatusColor = new SolidColorBrush(Colors.Red);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                StatusColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        public bool UnequipFeature(int featureId)
        {
            try
            {
                // Call service to unequip feature
                bool success = _featuresService.UnequipFeature(
                    _userService.GetCurrentUser().UserId, 
                    featureId);
                
                if (success)
                {
                    StatusMessage = "Feature unequipped successfully";
                    StatusColor = new SolidColorBrush(Colors.Green);
                    
                    // Refresh features list to update UI
                    LoadFeatures();
                }
                else
                {
                    StatusMessage = "Failed to unequip feature";
                    StatusColor = new SolidColorBrush(Colors.Red);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                StatusColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        public async void ShowPreview(FeatureDisplay feature)
        {
            await ShowPreviewDialog(feature);
        }
    }
}
