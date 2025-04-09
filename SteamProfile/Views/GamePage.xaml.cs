using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;

namespace SteamProfile.Views
{
    public sealed partial class GamePage : Page
    {
        private int _gameId;

        public GamePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int gameId)
            {
                _gameId = gameId;
                GameIdText.Text = gameId.ToString();
                Debug.WriteLine($"GamePage: Loaded game with ID {gameId}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
} 