using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using BusinessLayer.Services;
using SteamProfile.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;

namespace SteamProfile.Views
{
    public sealed partial class CollectionGamesPage : Page
    {
        private CollectionGamesViewModel _viewModel;
        private int _collectionId;
        private string _collectionName;
        private readonly UserService _userService = App.UserService;

        public CollectionGamesPage()
        {
            this.InitializeComponent();
            _viewModel = new CollectionGamesViewModel(App.CollectionsService);
            this.DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is (int collectionId, string collectionName))
            {
                _collectionId = collectionId;
                _collectionName = collectionName;
                LoadCollectionGames();
            }
            else if (e.Parameter is int backCollectionId)
            {
                // Handle back navigation from AddGameToCollectionPage
                _collectionId = backCollectionId;
                var userId = _userService.GetCurrentUser().UserId;
                var collection = App.CollectionsService.GetCollectionById(_collectionId, userId);
                if (collection != null)
                {
                    _collectionName = collection.Name;
                    LoadCollectionGames();
                }
            }
        }

        private void LoadCollectionGames()
        {
            try
            {
                _viewModel.CollectionName = _collectionName;
                _viewModel.LoadGames(_collectionId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading collection games: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CollectionsPage));
        }

        private void AddGameToCollection_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddGameToCollectionPage), _collectionId);
        }

        private void RemoveGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null) return;

                int gameId = Convert.ToInt32(button.Tag);
                Debug.WriteLine($"Removing game {gameId} from collection {_collectionId}");

                App.CollectionsService.RemoveGameFromCollection(_collectionId, gameId);
                _viewModel.LoadGames(_collectionId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing game from collection: {ex.Message}");
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to remove game from collection. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                dialog.ShowAsync();
            }
        }

        private void ViewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null) return;

                int gameId = Convert.ToInt32(button.Tag);
                Debug.WriteLine($"Navigating to game page for game {gameId}");
                Frame.Navigate(typeof(GamePage), gameId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to game page: {ex.Message}");
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine($"Failed to load image: {e.ErrorMessage}");
            if (sender is Image image && image.DataContext is OwnedGame game)
            {
                Debug.WriteLine($"Failed to load image for game: {game.Title}");
                Debug.WriteLine($"Image path: {game.CoverPicture}");
            }
        }
    }
} 