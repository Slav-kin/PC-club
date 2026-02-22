using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PC_club.ViewModels
{
    // Обов'язково PARTIAL для роботи генераторів коду!
    public partial class MainWindowViewModel : ViewModelBase
    {
        // 1. Навігація
        [ObservableProperty]
        private ViewModelBase _currentPage;

        // 2. Дані для головного екрану (залишаємо вашу логіку)
        public ObservableCollection<Place> PCPlaces { get; set; } = new();

        [ObservableProperty]
        private Client _client = new Client();

        [ObservableProperty]
        private decimal _todayIncome;

        [ObservableProperty]
        private int _todaySessionsCount;

        [ObservableProperty]
        private int _occupiedPlacesCount;

        [ObservableProperty]
        private int _activeClientsCount;

        public MainWindowViewModel()
        {
            // Встановлюємо себе як початкову сторінку
            _currentPage = this;

            RefreshHall();

            using (var db = new PcClubContext())
            {
                Client = db.Clients.FirstOrDefault() ?? new Client();
            }
        }

        // Команди для кнопок (генерують NavigateToHomeCommand тощо)
        [RelayCommand]
        private void NavigateToHome() => CurrentPage = this;

        [RelayCommand]
        private void NavigateToClients()
        {
            CurrentPage = new ClientsViewModel();
        }

        public void RefreshHall()
        {
            using (var db = new PcClubContext())
            {
                var data = db.Places
                    .Include(p => p.Sessions).ThenInclude(s => s.Client)
                    .Include(p => p.Sessions).ThenInclude(s => s.Tariff)
                    .ToList();

                PCPlaces.Clear();
                foreach (var item in data) PCPlaces.Add(item);

                var today = System.DateTime.Today;

                ActiveClientsCount = db.Clients.Count(c => c.Status == "active");
                OccupiedPlacesCount = db.Sessions.Count(s => s.Status == "active");
                TodaySessionsCount = db.Sessions.Count(s => s.StartSession.HasValue && s.StartSession.Value.Date == today);
                TodayIncome = db.Sessions
                    .Where(s => s.StartSession.HasValue && s.StartSession.Value.Date == today)
                    .Sum(s => s.TotalPrice) ?? 0;
            }
        }
    }
}