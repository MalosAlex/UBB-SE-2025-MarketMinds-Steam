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
    public sealed partial class FeaturesPage : Page
    {
        private readonly FeaturesViewModel _viewModel;

        public FeaturesPage()
        {
            this.InitializeComponent();
            _viewModel = new FeaturesViewModel(App.FeaturesService, this.Frame);
            this.DataContext = _viewModel;
            this.Loaded += FeaturesPage_Loaded;
        }

        private void FeaturesPage_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Initialize(this.XamlRoot);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is FeatureDisplay feature)
            {
                ShowOptionsDialog(feature);
            }
        }

        private async void ShowOptionsDialog(FeatureDisplay feature)
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = feature.Name,
                CloseButtonText = "Cancel"
            };

            StackPanel panel = new StackPanel { Spacing = 10 };
            
            if (!feature.IsPurchased)
            {
                Button buyButton = new Button
                {
                    Content = "Buy",
                    Style = Application.Current.Resources["AccentButtonStyle"] as Style
                };
                
                buyButton.Click += (s, e) => 
                {
                    dialog.Hide();
                    this.Frame.Navigate(typeof(ShopPage), feature.FeatureId);
                };
                
                panel.Children.Add(buyButton);
            }
            else if (feature.Equipped)
            {
                Button unequipButton = new Button
                {
                    Content = "Unequip",
                    Style = Application.Current.Resources["AccentButtonStyle"] as Style
                };
                
                unequipButton.Click += (s, e) => 
                {
                    try
                    {
                        bool success = _viewModel.UnequipFeature(feature.FeatureId);
                        if (success)
                        {
                            dialog.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                    }
                };
                
                panel.Children.Add(unequipButton);
            }
            else
            {
                Button equipButton = new Button
                {
                    Content = "Equip",
                    Style = Application.Current.Resources["AccentButtonStyle"] as Style
                };
                
                equipButton.Click += (s, e) => 
                {
                    try
                    {
                        bool success = _viewModel.EquipFeature(feature.FeatureId);
                        if (success)
                        {
                            dialog.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                    }
                };
                
                panel.Children.Add(equipButton);
            }
            
            // Add preview button
            Button previewButton = new Button
            {
                Content = "Preview",
                Style = Application.Current.Resources["DefaultButtonStyle"] as Style
            };
            
            previewButton.Click += (s, e) => 
            {
                dialog.Hide();
                _viewModel.ShowPreview(feature);
            };
            
            panel.Children.Add(previewButton);
            
            dialog.Content = panel;
            await dialog.ShowAsync();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var image = sender as Image;
            if (image != null)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image. Error: {e.ErrorMessage}");
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the current user ID from the service or ViewModel
            int userId = App.UserService.GetCurrentUser().UserId;

            // If we can navigate back (in case we came from Profile)
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                // Navigate directly to ProfilePage with the user ID
                this.Frame.Navigate(typeof(ProfilePage), userId);
            }
        }
    }
}
