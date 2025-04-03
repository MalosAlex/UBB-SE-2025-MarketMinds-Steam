using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;

namespace SteamProfile.ViewModels
{
    public partial class ShopViewModel : ObservableObject
    {
        private readonly Frame _frame;

        public ShopViewModel(Frame frame)
        {
            _frame = frame;
        }

        [RelayCommand]
        private void NavigateBack()
        {
            _frame.GoBack();
        }
    }
} 