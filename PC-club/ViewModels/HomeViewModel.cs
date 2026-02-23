using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PC_club.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        [ObservableProperty]
        private int _occupiedPlacesCount;

        [ObservableProperty]
        private int _activeClientsCount;

        [ObservableProperty]
        private int _todaySessionsCount;

        [ObservableProperty]
        private decimal _todayIncome; // Виправлено: тепер це decimal

        public ObservableCollection<Place> PCPlaces { get; set; }

        public HomeViewModel()
        {
            PCPlaces = new ObservableCollection<Place>();
            RefreshHall();
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