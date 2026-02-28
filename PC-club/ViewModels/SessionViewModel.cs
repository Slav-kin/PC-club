using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace PC_club.ViewModels
{
    public partial class SessionViewModel : ViewModelBase
    {
        private readonly PcClubContext _db;

        private ObservableCollection<Session> _sessions = new();
        public ObservableCollection<Session> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }
        #region ShortStats

        [ObservableProperty]
        private int _countTodayTotalSessions;

        [ObservableProperty]
        private int _countActiveSessions;

        [ObservableProperty]
        private decimal _countTotalEarnToday;

        #endregion

        #region NewSessionVarible

        [ObservableProperty]
        private ObservableCollection<Client> _availableClients = new();

        [ObservableProperty]
        private ObservableCollection<Place> _availablePlace = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableAcaunt = new() { "Власний", "Клубний" };

        [ObservableProperty]
        private bool _isAddSessionWindowOpen;

        [ObservableProperty]
        private Client? _selectedClient;

        [ObservableProperty]
        private Place? _selectedPlace;

        [ObservableProperty]
        private string? _selectedGameAcaunt;

        #endregion

        private Tariff? GetTariffForPlace(int PlaceId)
        {
           
            string expectedTariffName = PlaceId switch
            {
                <= 5 => "basic",
                <= 10 => "consol",
                <= 15 => "pro",
                _ => "vip"
            };

            return _db.Tariffs.FirstOrDefault(t => t.TariffName == expectedTariffName);
        }

        public SessionViewModel(PcClubContext db)
        {
            _db = db;

            LoadSessions();
        }

        private void LoadSessions()
        {

            var today = DateTime.Today;

            var sessionsFromDb = _db.Sessions
                .Include(s => s.Client)
                .Include(s => s.Tariff)
                .Include(s => s.Place)
                .OrderByDescending(s => s.SessionId)
                .ToList();

            Sessions = new ObservableCollection<Session>(sessionsFromDb);

            CountTodayTotalSessions = sessionsFromDb.Count(s => s.StartSession.HasValue && s.StartSession.Value.Date == today);

            CountActiveSessions = sessionsFromDb.Count(s => s.Status == "active");

            CountTotalEarnToday = sessionsFromDb.Sum(s => s.TotalPrice ?? 0m);

        }


        [RelayCommand]
        private void OpenAddSessionWindow()
        {
            AvailableClients = new ObservableCollection<Client>(_db.Clients.ToList());
            AvailablePlace = new ObservableCollection<Place>(_db.Places.Where(p => p.Status == "active").ToList());
         
            IsAddSessionWindowOpen = true;
        }   

        [RelayCommand]
        private void SaveNewSession()
        {
            if (SelectedClient == null || SelectedPlace == null || SelectedGameAcaunt == null) return;

            var tariff = GetTariffForPlace(SelectedPlace.PlaceId);
            if (tariff == null)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка: Тариф не знайдено для місця з ID {SelectedPlace.PlaceId}");
                return;
            }

            string dbAccountValue = SelectedGameAcaunt switch
            {
                "Власний" => "own",
                "Клубний" => "club",
                _ => "own" 
            };

            var newSession = new Session()
            {
                ClientId = SelectedClient.ClientId,
                PlaceId = SelectedPlace.PlaceId,
                TariffId = tariff.TariffId,
                GameAccount = dbAccountValue,
                StartSession = DateTime.Now,
                Status = "active",
            };

            _db.Sessions.Add(newSession);

            var placeInDb = _db.Places.FirstOrDefault(p => p.PlaceId == SelectedPlace.PlaceId);
            if (placeInDb != null)
            {
                placeInDb.Status = "inactive";
            }

            _db.SaveChanges();

            LoadSessions();
            IsAddSessionWindowOpen = false;
            CloseAddSessionWindow();
        }

        [RelayCommand]
        private void CloseAddSessionWindow()
        {
            IsAddSessionWindowOpen = false;

            SelectedClient = null;
            SelectedPlace = null;

        }

        [RelayCommand]
        private void EndActiveSession(Session sessionToClose)
        {
            if (sessionToClose == null || sessionToClose.Status != "active") return;

            // Завантажуємо сесію з БД разом із пов'язаним тарифом і місцем
            var session = _db.Sessions
                .Include(s => s.Tariff)
                .Include(s => s.Place)
                .FirstOrDefault(s => s.SessionId == sessionToClose.SessionId);

            if (session == null) return;

            // 1. Фіксуємо час завершення
            session.EndSession = DateTime.Now;
            session.Status = "completed"; // Змінюємо статус сесії

            // 2. Підраховуємо тривалість і ціну
            if (session.StartSession.HasValue)
            {
                TimeSpan duration = session.EndSession.Value - session.StartSession.Value;
                // Тут ми множимо ціну за годину на кількість відсиджених годин (разом із хвилинами)
                decimal pricePerHour = session.Tariff.TariffPrice;

                decimal total = (decimal)duration.TotalHours * pricePerHour;

                // Округлюємо до 2 знаків після коми (або використовуй Math.Ceiling, якщо округляєш на користь клубу)
                session.TotalPrice = Math.Round(total, 2);
            }

            // 3. Звільняємо комп'ютер (Місце)
            if (session.Place != null)
            {
                session.Place.Status = "active"; // Повертаємо ПК статус вільного
            }

            // 4. Зберігаємо зміни
            _db.SaveChanges();

            // 5. Оновлюємо список і статистику на екрані
            LoadSessions();
        }
    }
}
