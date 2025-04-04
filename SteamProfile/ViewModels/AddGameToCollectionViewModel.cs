using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SteamProfile.ViewModels
{
    public class AddGameToCollectionViewModel : ObservableObject
    {
        private readonly CollectionsService _collectionsService;
        private readonly UserService _userService;
        private readonly int _userId;
        private ObservableCollection<OwnedGame> _availableGames;
        private bool _isLoading;
        private string _errorMessage;
        private int _collectionId;

        public ObservableCollection<OwnedGame> AvailableGames
        {
            get => _availableGames;
            set => SetProperty(ref _availableGames, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public AddGameToCollectionViewModel(CollectionsService collectionsService)
        {
            _collectionsService = collectionsService;
            _availableGames = new ObservableCollection<OwnedGame>();
        }

        public void Initialize(int collectionId)
        {
            _collectionId = collectionId;
            LoadAvailableGames();
        }

        private void LoadAvailableGames()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                Debug.WriteLine($"Loading available games for collection {_collectionId}");
                var games = _collectionsService.GetGamesNotInCollection(_collectionId, App.UserService.GetCurrentUser().UserId);
                Debug.WriteLine($"Retrieved {games.Count} available games");

                AvailableGames.Clear();
                foreach (var game in games)
                {
                    AvailableGames.Add(game);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading available games: {ex.Message}");
                ErrorMessage = "Failed to load available games. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AddGameToCollection(OwnedGame game)
        {
            try
            {
                _collectionsService.AddGameToCollection(_collectionId, game.GameId, App.UserService.GetCurrentUser().UserId);
                AvailableGames.Remove(game);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding game to collection: {ex.Message}");
                ErrorMessage = "Failed to add game to collection. Please try again.";
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