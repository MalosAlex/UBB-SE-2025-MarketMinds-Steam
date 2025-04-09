using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;
using System;
using System.Diagnostics;

namespace SteamProfile.Views
{
    public sealed partial class CollectionGamesPage : Page
    {
        // Constants to replace magic strings
        private const string ErrorDialogTitle = "Error";
        private const string RemoveGameErrorMessage = "Failed to remove game from collection. Please try again.";
        private const string CloseButtonTextValue = "OK";

        private CollectionGamesViewModel _collectionGamesViewModel;
        private CollectionsViewModel _collectionsViewModel;
        private UsersViewModel _userViewModel;
        private int _collectionId;
        private string _collectionName = string.Empty;

        public CollectionGamesPage()
        {
            this.InitializeComponent();
            _collectionGamesViewModel = App.CollectionGamesViewModel;
            _collectionsViewModel = App.CollectionsViewModel;
            _collectionsViewModel.LoadCollections();

            _userViewModel = App.UsersViewModel;
            this.DataContext = _collectionGamesViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is (int collectionId, string collectionName))
            {
                _collectionId = collectionId;
                _collectionName = collectionName;
                LoadCollectionGames();
            }
            else if (eventArgs.Parameter is int backCollectionId)
            {
                // Handle back navigation from AddGameToCollectionPage
                _collectionId = backCollectionId;
                var userId = _userViewModel.GetCurrentUser().UserId;
                var collection = _collectionsViewModel.GetCollectionById(_collectionId, userId);
                if (collection != null)
                {
                    _collectionName = collection.Name;
                    LoadCollectionGames();
                }
            }
        }

        private void LoadCollectionGames()
        {
            _collectionGamesViewModel.CollectionName = _collectionName; 
            _collectionGamesViewModel.LoadGames(_collectionId);
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(CollectionsPage));
        }

        private void AddGameToCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(AddGameToCollectionPage), _collectionId);
        }

        private void RemoveGame_Click(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null)
                {
                    return;
                }

                int gameId = Convert.ToInt32(button.Tag);

                _collectionsViewModel.RemoveGameFromCollection(_collectionId, gameId);
                _collectionGamesViewModel.LoadGames(_collectionId);
            }
            catch (Exception exception)
            {
                var dialog = new ContentDialog
                {
                    Title = ErrorDialogTitle,
                    Content = RemoveGameErrorMessage,
                    CloseButtonText = CloseButtonTextValue,
                    XamlRoot = this.XamlRoot
                };
                dialog.ShowAsync();
            }
        }

        private void ViewGame_Click(object sender, RoutedEventArgs eventArgs)
        {
            var button = sender as Button;
            if (button?.Tag == null)
            {
                return;
            }

            int gameId = Convert.ToInt32(button.Tag);
            Frame.Navigate(typeof(GamePage), gameId);
        }
    }
}
