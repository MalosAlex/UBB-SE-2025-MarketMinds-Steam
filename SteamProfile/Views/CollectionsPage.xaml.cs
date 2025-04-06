using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using BusinessLayer.Models;
using System;

namespace SteamProfile.Views
{
    public sealed partial class CollectionsPage : Page
    {
        private CollectionsViewModel _collectionsViewModel;
        private UsersViewModel _usersViewModel;

        public CollectionsPage()
        {
            this.InitializeComponent();
            _collectionsViewModel = new CollectionsViewModel(App.CollectionsService, App.UserService);
            _usersViewModel = App.UsersViewModel;
            this.DataContext = _collectionsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadCollections();
        }

        private void LoadCollections()
        {
            _collectionsViewModel.LoadCollectionsCommand.Execute(null);
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
                _collectionsViewModel.DeleteCollectionCommand.Execute(collectionId);
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
                        _collectionsViewModel.UpdateCollectionCommand.Execute(new UpdateCollectionParams
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
                    _collectionsViewModel.CreateCollectionCommand.Execute(new CreateCollectionParams
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
            Frame.Navigate(typeof(ProfilePage), _usersViewModel.GetCurrentUser().UserId);
        }
    }
}
