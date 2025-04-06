using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Services;
using SteamProfile.Views;
using SteamProfile.Views.ConfigurationsView;
using System;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class ConfigurationsViewModel : ObservableObject
    {
        private readonly Frame _frame;
        private readonly IUserService _userService;

        public ConfigurationsViewModel(Frame frame, IUserService userService)
        {
            _frame = frame;
            _userService = userService ?? throw new NullReferenceException(nameof(userService));
        }

        [RelayCommand]
        private void NavigateToFeatures()
        {
            _frame.Navigate(typeof(FeaturesPage));
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            _frame.Navigate(typeof(ProfilePage),_userService.GetCurrentUser().UserId);
        }
        [RelayCommand]
        private void NavigateToProfileSettings()
        {
            _frame.Navigate(typeof(ModifyProfilePage));
        }
        [RelayCommand]

        private void NavigateToAccountSettings()
        {
            _frame.Navigate(typeof(AccountSettingsPage));
        }
        
    }
}
