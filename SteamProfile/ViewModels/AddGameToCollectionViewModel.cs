using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public class AddGameToCollectionViewModel : ObservableObject
    {
        private readonly ICollectionsService collectionsService;
        private readonly IUserService userService;
        private int userId;
        private ObservableCollection<OwnedGame> availableGames;
        private bool isLoading;
        private string errorMessage;
        private int collectionId;

        public ObservableCollection<OwnedGame> AvailableGames
        {
            get => availableGames;
            set => SetProperty(ref availableGames, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public AddGameToCollectionViewModel(ICollectionsService collectionsService, IUserService userService)
        {
            this.collectionsService = collectionsService ?? throw new ArgumentNullException(nameof(collectionsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            availableGames = new ObservableCollection<OwnedGame>();
        }

        public void Initialize(int collectionId)
        {
            this.collectionId = collectionId;
            LoadAvailableGames();
        }

        private void LoadAvailableGames()
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                Debug.WriteLine($"Loading available games for collection {collectionId}");
                var gamesNotInCollection = collectionsService.GetGamesNotInCollection(collectionId, userService.GetCurrentUser().UserId);
                Debug.WriteLine($"Retrieved {gamesNotInCollection.Count} available games");

                AvailableGames.Clear();
                foreach (var availableGame in gamesNotInCollection)
                {
                    AvailableGames.Add(availableGame);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading available games: {ex.Message}");
                errorMessage = "Failed to load available games. Please try again.";
            }
            finally
            {
                isLoading = false;
            }
        }

        public void AddGameToCollection(OwnedGame game)
        {
            try
            {
                collectionsService.AddGameToCollection(collectionId, game.GameId, userService.GetCurrentUser().UserId);
                AvailableGames.Remove(game);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding game to collection: {ex.Message}");
                errorMessage = "Failed to add game to collection. Please try again.";
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
