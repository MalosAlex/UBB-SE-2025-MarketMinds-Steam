using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;
using System;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class CollectionsForVisitorsPage : Page
    {
        private const string FailedToLoadCollectionsErrorMessage = "Failed to load collections. Please try again later.";

        private CollectionsViewModel _collectionsViewModel;
        private int _userId;

        public CollectionsForVisitorsPage()
        {
            this.InitializeComponent();
            _collectionsViewModel = App.CollectionsViewModel;
            _collectionsViewModel.LoadCollections();
            this.DataContext = _collectionsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is int userId)
            {
                _userId = userId;
                LoadCollections();
            }
        }

        private void LoadCollections()
        {
            try
            {
                _collectionsViewModel.IsLoading = true;
                _collectionsViewModel.ErrorMessage = string.Empty;
                
                var collections = _collectionsViewModel.GetPublicCollectionsForUser(_userId);
                _collectionsViewModel.Collections = new ObservableCollection<Collection>(collections);
            }
            catch (Exception)
            {
                _collectionsViewModel.ErrorMessage = FailedToLoadCollectionsErrorMessage;
            }
            finally
            {
                _collectionsViewModel.IsLoading = false;
            }
        }

        private void ViewCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is int collectionId)
            {
                Frame.Navigate(typeof(CollectionGamesPage), collectionId);
            }
        }
    }
}
