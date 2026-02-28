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

        [ObservableProperty] private bool _isHomeSelected;
        [ObservableProperty] private bool _isClientsSelected;
        [ObservableProperty] private bool _isSessionsSelected;
        [ObservableProperty] private bool _isBookingSelected;

        public MainWindowViewModel()
        {
            // При старті програми автоматично відкриваємо Головну сторінку
            CurrentPage = new HomeViewModel();
            ResetSelection();
            IsHomeSelected = true;
        }

        // --- Команди для кнопок бокового меню ---

        [RelayCommand]
        private void NavigateToHome()
        {
            // Коли натиснули "Головна", підставляємо HomeViewModel
            CurrentPage = App.Services.GetRequiredService<HomeViewModel>();
            ResetSelection();
            IsHomeSelected = true;
        }

        [RelayCommand]
        private void NavigateToClients()
        {
            // Коли натиснули "Клієнти", підставляємо ClientsViewModel
            CurrentPage = App.Services.GetRequiredService<ClientsViewModel>();
            ResetSelection();
            IsClientsSelected = true;
        }

        [RelayCommand]
        private void NavigateToSessions()
        {
            CurrentPage = App.Services.GetRequiredService<SessionViewModel>();
            ResetSelection();
            IsSessionsSelected = true;
        }

        [RelayCommand]
        private void NavigateToBookings()
        {
            CurrentPage = App.Services.GetRequiredService<BookingsViewModel>();
            ResetSelection();
            IsBookingSelected = true;
        }

        private void ResetSelection()
        {
            IsHomeSelected = false;
            IsClientsSelected = false;
            IsSessionsSelected = false;
            IsBookingSelected = false;
        }
    }
}