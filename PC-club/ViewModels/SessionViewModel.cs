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

        public ObservableCollection<Client> AvailableClients { get; set; } = new();
        public ObservableCollection<Place> AvailablePlace { get; set; } = new();
        public ObservableCollection<Session> AvailableAcaunt { get; set; } = new();

        [ObservableProperty]
        private bool _isAddSessionWindowOpen;

        [ObservableProperty]
        private Client? _selectedClient;

        [ObservableProperty]
        private Place? _selectedPlace;

        [ObservableProperty]
        private Session? _selectedGameAcaunt;
        #endregion


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

            CountActiveSessions = sessionsFromDb.Count(s => s.Status == "Active");

            CountTotalEarnToday = sessionsFromDb.Sum(s => s.TotalPrice ?? 0m);
        }


        [RelayCommand]
        private void OpenAddSessionWindow()
        {
            AvailableClients = new ObservableCollection<Client>(_db.Clients.ToList());
            AvailablePlace = new ObservableCollection<Place>(_db.Places.Where(p => p.Status == "Active").ToList());
            AvailableAcaunt = new ObservableCollection<Session>(_db.Sessions.ToList());

            IsAddSessionWindowOpen = true;
        }

        [RelayCommand]
        private void SaveNewSession()
        {
            if (SelectedClient == null || SelectedPlace == null || SelectedGameAcaunt == null) return;

            var newSession = new Session()
            {
                ClientId = SelectedClient.ClientId,
                PlaceId = SelectedPlace.PlaceId,
                GameAccount = SelectedGameAcaunt.GameAccount,
                StartSession = DateTime.Now,
                Status = "Active",
            };

            _db.Sessions.Add(newSession);
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
    }
}
