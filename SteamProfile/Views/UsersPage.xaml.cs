using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class UsersPage : Page
    {
        public UsersViewModel _viewModel { get; }

        public UsersPage()
        {
            _viewModel = UsersViewModel.Instance;
            this.InitializeComponent();
            _viewModel = UsersViewModel.Instance;
            this.DataContext = _viewModel;
        }
    }
} 