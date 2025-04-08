using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Navigation;

namespace SteamProfile.Views
{
    public sealed partial class CollectionsForVisitorsPage : Page
    {
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
                _collectionsViewModel.Collections = new System.Collections.ObjectModel.ObservableCollection<BusinessLayer.Models.Collection>(collections);
            }
            catch (Exception exception)
            {
                _collectionsViewModel.ErrorMessage = "Failed to load collections. Please try again later.";
                Debug.WriteLine($"Error loading collections: {exception.Message}");
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