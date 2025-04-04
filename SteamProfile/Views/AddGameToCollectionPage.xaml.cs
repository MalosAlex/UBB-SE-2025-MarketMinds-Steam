using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;
using System;

namespace SteamProfile.Views
{
    public sealed partial class AddGameToCollectionPage : Page
    {
        private AddGameToCollectionViewModel _viewModel;
        private int _collectionId;

        public AddGameToCollectionPage()
        {
            this.InitializeComponent();
            _viewModel = new AddGameToCollectionViewModel(App.CollectionsService);
            this.DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int collectionId)
            {
                _collectionId = collectionId;
                _viewModel.Initialize(collectionId);
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
                _viewModel.AddGameToCollection(game);
            }
        }
    }
} 