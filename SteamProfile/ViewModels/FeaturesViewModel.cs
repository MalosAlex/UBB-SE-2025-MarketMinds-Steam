using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SteamProfile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamProfile.ViewModels
{
    public partial class FeaturesViewModel : ObservableObject
    {
        private readonly FeaturesService featuresService;
        private readonly IUserService userService;
        private XamlRoot xamlRoot;
        private string statusMessage = string.Empty;
        private readonly UserProfilesRepository _userProfilesRepository;
        private SolidColorBrush statusColor = new(Colors.Black);
        private FeatureDisplay selectedFeature;
        private XamlRoot _xamlRoot;

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public SolidColorBrush StatusColor
        {
            get => statusColor;
            set => SetProperty(ref statusColor, value);
        }

        public FeatureDisplay SelectedFeature
        {
            get => selectedFeature;
            set => SetProperty(ref selectedFeature, value);
        }

        public ObservableCollection<FeatureDisplay> Frames { get; } = new();
        public ObservableCollection<FeatureDisplay> Emojis { get; } = new();
        public ObservableCollection<FeatureDisplay> Backgrounds { get; } = new();
        public ObservableCollection<FeatureDisplay> Pets { get; } = new();
        public ObservableCollection<FeatureDisplay> Hats { get; } = new();
  
        public FeaturesViewModel(FeaturesService featuresService, IUserService userService)
        {
            this.featuresService = featuresService;
            this.userService = userService;
            LoadFeatures();
        }

        public void SetXamlRoot(XamlRoot xamlRoot)
        {
            this.xamlRoot = xamlRoot;
        }
        public void Initialize(XamlRoot xamlRoot)
        {
            _xamlRoot = xamlRoot;
        }
        private void LoadFeatures()
        {
            try
            {
                var features = featuresService.GetFeaturesByCategories();
                
                UpdateCollection(Frames, features.GetValueOrDefault("frame", new()));
                UpdateCollection(Emojis, features.GetValueOrDefault("emoji", new()));
                UpdateCollection(Backgrounds, features.GetValueOrDefault("background", new()));
                UpdateCollection(Pets, features.GetValueOrDefault("pet", new()));
                UpdateCollection(Hats, features.GetValueOrDefault("hat", new()));

                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to load features. Please try again later.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        private void UpdateCollection(ObservableCollection<FeatureDisplay> collection, List<Feature> features)
        {
            collection.Clear();
            var currentUser = userService.GetCurrentUser();
            foreach (var feature in features)
            {
                bool isPurchased = featuresService.IsFeaturePurchased(currentUser.UserId, feature.FeatureId);
                collection.Add(new FeatureDisplay(feature, isPurchased));
            }
        }

        [RelayCommand]
        private async Task ShowOptionsAsync(FeatureDisplay feature)
        {
            if (feature == null || xamlRoot == null) return;

            SelectedFeature = feature;
            var currentUser = userService.GetCurrentUser();
            
            var dialog = new ContentDialog
            {
                XamlRoot = xamlRoot,
                Title = feature.Name
            };

            var buttons = new StackPanel { Spacing = 10 };

            if (feature.IsPurchased)
            {
                if (feature.Equipped)
                {
                    buttons.Children.Add(new Button
                    {
                        Content = "Unequip",
                        Command = new RelayCommand(() => UnequipFeature(currentUser.UserId, feature))
                    });
                }
                else
                {
                    buttons.Children.Add(new Button
                    {
                        Content = "Equip",
                        Command = new RelayCommand(() => EquipFeature(feature.FeatureId))
                    });
                }
            }
            else
            {
                buttons.Children.Add(new Button
                {
                    Content = "Purchase",
                    Command = new RelayCommand(() => PurchaseFeature(currentUser.UserId, feature))
                });
            }

            dialog.Content = buttons;
            await dialog.ShowAsync();
        }

        public static event EventHandler<int> FeatureEquipStatusChanged;
        public bool EquipFeature(int featureId)
        {
            try
            {
                // Call service to equip feature
                bool success = featuresService.EquipFeature(
                    userService.GetCurrentUser().UserId,
                    featureId);

                if (success)
                {
                    // Notify that a feature was equipped
                    FeatureEquipStatusChanged ?.Invoke(this, userService.GetCurrentUser().UserId);

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

        public bool UnequipFeature(int userId, FeatureDisplay feature)
        {
            var result = featuresService.UnequipFeature(userId, feature.FeatureId);
            StatusMessage = result.Item2;
            StatusColor = new SolidColorBrush(result.Item1 ? Colors.Green : Colors.Red);
            
            if (result.Item1)
            {
                LoadFeatures();
            }
            return result.Item1;
        }

        public async void ShowPreview(FeatureDisplay feature)
        {
            await ShowPreviewDialog(feature);
        }

        private void PurchaseFeature(int userId, FeatureDisplay feature)
        {
            // TODO: Implement purchase logic
            StatusMessage = "Purchase feature not implemented yet.";
            StatusColor = new SolidColorBrush(Colors.Red);
        }

        private async Task ShowPreviewDialog(FeatureDisplay featureDisplay)
        {
            // Get current user and equipped features
            var user = userService.GetCurrentUser();
            var userFeatures = featuresService.GetUserEquippedFeatures(user.UserId);

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
    }

    public class FeatureDisplay : ObservableObject
    {
        private readonly Feature feature;
        private readonly bool isPurchased;
        private bool isEquipped;

        public FeatureDisplay(Feature feature, bool isPurchased)
        {
            this.feature = feature;
            this.isPurchased = isPurchased;
            this.isEquipped = feature.Equipped;
        }

        public int FeatureId => feature.FeatureId;
        public string Name => feature.Name;
        public string Description => feature.Description;
        public string Type => feature.Type;
        public int Value => feature.Value;
        public bool IsPurchased => isPurchased;
        public bool Equipped
        {
            get => isEquipped;
            set => SetProperty(ref isEquipped, value);
        }
        public string Source
        {
            get
            {
                if (feature == null)
                {
                    return string.Empty;
                }

                return $"ms-appx:///{feature.Source}";
            }
        }
    }
}
