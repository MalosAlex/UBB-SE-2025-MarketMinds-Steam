using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class FriendsPage : Page
    {
        private readonly FriendsViewModel _friendsViewModel;
        private readonly UsersViewModel _usersViewModel;

        public FriendsPage()
        {
            InitializeComponent();
            _friendsViewModel = App.FriendsViewModel;
            _usersViewModel = App.UsersViewModel;
            DataContext = _friendsViewModel;
            
            // Load friends immediately when page is created
            _friendsViewModel.LoadFriends();
        }

        private void RemoveFriend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                _friendsViewModel.RemoveFriend(friendshipId);
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
            Frame.Navigate(typeof(ProfilePage), _usersViewModel.GetCurrentUser().UserId);
        }
    }
}