using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class AddGameToCollectionPage : Page
    {
        private AddGameToCollectionViewModel _addGamesToCollectionViewModel;
        private int _collectionId;

        public AddGameToCollectionPage()
        {
            this.InitializeComponent();
            _addGamesToCollectionViewModel = App.AddGameToCollectionViewModel;
            this.DataContext = _addGamesToCollectionViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int collectionId)
            {
                _collectionId = collectionId;
                _addGamesToCollectionViewModel.Initialize(collectionId);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CollectionGamesPage), (_collectionId, ""));
        }

        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is OwnedGame game)
            {
                _addGamesToCollectionViewModel.AddGameToCollection(game);
            }
        }
    }
} 