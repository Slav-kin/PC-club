using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PC_club.ViewModels
{
    internal partial class SessionViewModel : ViewModelBase
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
    }
}
