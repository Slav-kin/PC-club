using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_club.ViewModels
{
    public partial class BookingsViewModel : ViewModelBase
    {
        private readonly PcClubContext _db;

        [ObservableProperty]
        private ObservableCollection<Booking> _bookings = new();

        #region ShortStats

        [ObservableProperty]
        private int _countActiveBookings;

        [ObservableProperty]
        private int _countTotalBooking;

        [ObservableProperty]
        private int _countExpectedSoonBooking;
        #endregion

        #region NewBookingVarible 

        [ObservableProperty]
        private ObservableCollection<Client> _availableClients = new();

        [ObservableProperty]
        private ObservableCollection<Place> _availablePlace = new();

        [ObservableProperty]
        private Client? _selectedClient;

        [ObservableProperty]
        private Place? _selectedPlace;

        [ObservableProperty]
        private DateTimeOffset _selectedDateTime = DateTimeOffset.Now;

        [ObservableProperty]
        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        private int _bookingDuration;

        [ObservableProperty]
        private ObservableCollection<string> _bookStatuses = new();


        [ObservableProperty]
        private bool _isAddBookingWindowOpen;
        #endregion

        public BookingsViewModel(PcClubContext db)
        {
            _db = db;   
          
            LoadBookings();
        }

        private void LoadBookings()
        {
            var bookingList = _db.Bookings
                 .Include(s => s.Client)
                 .Include(s => s.Place)
                 .OrderByDescending(s => s.BookId)
                 .ToList();

            CountActiveBookings = bookingList.Count(b => b.Status == "active");

            CountTotalBooking = bookingList.Count;

            var now = DateTime.Now;
            var soonThreshold = now.AddMinutes(15);

            CountExpectedSoonBooking = bookingList.Count(b =>
            b.Status == "active" &&
            b.BookTime >= now &&
            b.BookTime <= soonThreshold);

            Bookings = new ObservableCollection<Booking>(bookingList);
        }
    }
}
