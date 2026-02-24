using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using static PC_club.App;

namespace PC_club.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // Змінна, яка зберігає поточну сторінку для відображення
        [ObservableProperty]
        private ViewModelBase _currentPage;

        public MainWindowViewModel()
        {
            // При старті програми автоматично відкриваємо Головну сторінку
            CurrentPage = new HomeViewModel();
        }

        // --- Команди для кнопок бокового меню ---

        [RelayCommand]
        private void NavigateToHome()
        {
            // Коли натиснули "Головна", підставляємо HomeViewModel
            CurrentPage = new HomeViewModel();
        }

        [RelayCommand]
        private void NavigateToClients()
        {
            // Коли натиснули "Клієнти", підставляємо ClientsViewModel
            CurrentPage = App.Services.GetRequiredService<ClientsViewModel>();
        }

    }
}