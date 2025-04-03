using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;
using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.UI.Xaml.Navigation;

namespace SteamProfile.Views
{
    public sealed partial class CollectionsForVisitorsPage : Page
    {
        private CollectionsViewModel _viewModel;
        private int _userId;

        public CollectionsForVisitorsPage()
        {
            this.InitializeComponent();
            _viewModel = new CollectionsViewModel(App.CollectionsService, App.UserService);
            this.DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int userId)
            {
                _userId = userId;
                LoadCollections();
            }
        }

        private void LoadCollections()
        {
            try
            {
                _viewModel.IsLoading = true;
                _viewModel.ErrorMessage = string.Empty;
                
                var collections = App.CollectionsService.GetPublicCollectionsForUser(_userId);
                _viewModel.Collections = new System.Collections.ObjectModel.ObservableCollection<Models.Collection>(collections);
            }
            catch (Exception ex)
            {
                _viewModel.ErrorMessage = "Failed to load collections. Please try again later.";
                Debug.WriteLine($"Error loading collections: {ex.Message}");
            }
            finally
            {
                _viewModel.IsLoading = false;
            }
        }

        private void ViewCollection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int collectionId)
            {
                Frame.Navigate(typeof(CollectionGamesPage), collectionId);
            }
        }
    }
} 