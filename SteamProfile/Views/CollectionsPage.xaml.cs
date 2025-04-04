using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SteamProfile.Views
{
    public sealed partial class CollectionsPage : Page
    {
        private CollectionsViewModel _viewModel;

        public CollectionsPage()
        {
            this.InitializeComponent();
            _viewModel = new CollectionsViewModel(App.CollectionsService, App.UserService);
            this.DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadCollections();
        }

        private void LoadCollections()
        {
            _viewModel.LoadCollectionsCommand.Execute(null);
        }

        private void ViewCollection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Collection collection)
            {
                Frame.Navigate(typeof(CollectionGamesPage), (collection.CollectionId, collection.Name));
            }
        }

        private void DeleteCollection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int collectionId)
            {
                _viewModel.DeleteCollectionCommand.Execute(collectionId);
            }
        }

        private async void EditCollection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Collection collection)
            {
                var dialog = new ContentDialog
                {
                    Title = "Edit Collection",
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var panel = new StackPanel { Spacing = 10 };
                
                var nameTextBox = new TextBox
                {
                    Header = "Collection Name",
                    Text = collection.Name,
                    PlaceholderText = "Enter collection name"
                };

                var coverPictureTextBox = new TextBox
                {
                    Header = "Cover Picture URL",
                    Text = collection.CoverPicture,
                    PlaceholderText = "Enter cover picture URL (picture.(jpg/png/svg))"
                };

                var isPublicToggle = new ToggleSwitch
                {
                    Header = "Public Collection",
                    IsOn = collection.IsPublic
                };

                panel.Children.Add(nameTextBox);
                panel.Children.Add(coverPictureTextBox);
                panel.Children.Add(isPublicToggle);

                dialog.Content = panel;

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        _viewModel.UpdateCollectionCommand.Execute(new UpdateCollectionParams
                        {
                            CollectionId = collection.CollectionId,
                            Name = nameTextBox.Text,
                            CoverPicture = coverPictureTextBox.Text,
                            IsPublic = isPublicToggle.IsOn
                        });
                    }
                    catch (Exception ex)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to update collection. Please try again.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
        }

        private async void CreateCollection_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Create New Collection",
                PrimaryButtonText = "Create",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var panel = new StackPanel { Spacing = 10 };
            
            var nameTextBox = new TextBox
            {
                Header = "Collection Name",
                PlaceholderText = "Enter collection name"
            };

            var coverPictureTextBox = new TextBox
            {
                Header = "Cover Picture URL",
                PlaceholderText = "Enter cover picture URL"
            };

            var isPublicToggle = new ToggleSwitch
            {
                Header = "Public Collection",
                IsOn = true
            };

            panel.Children.Add(nameTextBox);
            panel.Children.Add(coverPictureTextBox);
            panel.Children.Add(isPublicToggle);

            dialog.Content = panel;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    _viewModel.CreateCollectionCommand.Execute(new CreateCollectionParams
                    {
                        Name = nameTextBox.Text,
                        CoverPicture = coverPictureTextBox.Text,
                        IsPublic = isPublicToggle.IsOn,
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                    });
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to create collection. Please try again.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage), App.UserService.GetCurrentUser().UserId);
        }
    }
}
