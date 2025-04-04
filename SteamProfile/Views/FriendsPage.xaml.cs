using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using BusinessLayer.Services;

namespace SteamProfile.Views
{
    public sealed partial class FriendsPage : Page
    {
        private readonly FriendsViewModel _viewModel;

        public FriendsViewModel ViewModel => _viewModel;

        public FriendsPage()
        {
            InitializeComponent();
            _viewModel = new FriendsViewModel(App.FriendsService,App.UserService);
            DataContext = _viewModel;
            _viewModel.LoadFriends(); // Load friends immediately when page is created
        }

        private void RemoveFriend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                _viewModel.RemoveFriend(friendshipId);
            }
        }

        private void ViewFriend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendId)
            {
                Frame.Navigate(typeof(ProfilePage), friendId);
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage), App.UserService.GetCurrentUser().UserId);
        }
    }
}