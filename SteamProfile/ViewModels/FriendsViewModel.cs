using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;
using SteamProfile.Views;
using Windows.System;

namespace SteamProfile.ViewModels
{
    public partial class FriendsViewModel : ObservableObject
    {
        #region Constants
        // Error message constants
        private const string ErrLoadFriends = "Error loading friends: ";
        private const string ErrUnexpectedLoadFriends = "Unexpected error loading friends: ";
        private const string ErrRemoveFriend = "Error removing friend: ";
        private const string ErrUnexpectedRemoveFriend = "Unexpected error removing friend: ";
        private const string ErrDetailsPrefix = "\nDetails: ";
        #endregion

        private readonly IFriendsService friendsService;
        private readonly IUserService userService;

        [ObservableProperty]
        private ObservableCollection<Friendship> friendships = new ObservableCollection<Friendship>();

        [ObservableProperty]
        private Friendship selectedFriendship;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public FriendsViewModel(IFriendsService friendsService, IUserService userService)
        {
            this.friendsService = friendsService ?? throw new ArgumentNullException(nameof(friendsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [RelayCommand]
        public void LoadFriends()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var friendships = friendsService.GetAllFriendships();

                Friendships.Clear();
                foreach (var friendship in friendships)
                {
                    Friendships.Add(friendship);
                }
            }
            catch (ServiceException serviceException)
            {
                ErrorMessage = ErrLoadFriends + serviceException.Message;
                if (serviceException.InnerException != null)
                {
                    ErrorMessage += ErrDetailsPrefix + serviceException.InnerException.Message;
                }
            }
            catch (Exception generalException)
            {
                ErrorMessage = ErrUnexpectedLoadFriends + generalException.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void RemoveFriend(int friendshipId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                friendsService.RemoveFriend(friendshipId);

                // Refresh the friends list
                var updatedFriendships = friendsService.GetAllFriendships();
                Friendships.Clear();
                foreach (var friendship in updatedFriendships)
                {
                    Friendships.Add(friendship);
                }
            }
            catch (ServiceException serviceException)
            {
                ErrorMessage = ErrRemoveFriend + serviceException.Message;
                if (serviceException.InnerException != null)
                {
                    ErrorMessage += ErrDetailsPrefix + serviceException.InnerException.Message;
                }
            }
            catch (Exception generalException)
            {
                ErrorMessage = ErrUnexpectedRemoveFriend + generalException.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void SelectFriendship(Friendship friendship)
        {
            SelectedFriendship = friendship;
        }
    }
}
