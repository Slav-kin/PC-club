using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
        private DateTime? _selectedDateTime = DateTime.Now;

        [ObservableProperty]
        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        private int _bookingDuration;

        [ObservableProperty]
        private ObservableCollection<string> _bookStatuses = new();

        [ObservableProperty]
        private bool _isAddBookingWindowOpen;
        #endregion


        #region StartSessionVariables

        [ObservableProperty]
        private bool _isStartSessionWindowOpen;

        [ObservableProperty]
        private Booking? _bookingToStart;

        [ObservableProperty]
        private ObservableCollection<string> _accountTypes = new() { "own", "club" };

        [ObservableProperty]
        private string _selectedAccountType = "club";

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

        [RelayCommand]
        private void OpenAddBookingWindow()
        {
            AvailableClients = new ObservableCollection<Client>(_db.Clients.ToList());
            AvailablePlace = new ObservableCollection<Place>(_db.Places.Where(p => p.Status == "active").ToList());

            IsAddBookingWindowOpen = true;
        }

        [RelayCommand]
        private void CloseAddSessionWindow()
        {
            IsAddBookingWindowOpen = false;

            SelectedClient = null;
            SelectedPlace = null;
        }

        [RelayCommand]
        private void SaveNewBooking()
        {
            // 1. Проста перевірка
            if (SelectedClient == null || SelectedPlace == null) return;

            // 2. Об'єднуємо Дату з DatePicker та Час із TimePicker
            DateTime fullStartTime = SelectedDateTime.Value.Date.Add(SelectedTime);

            // 3. Створюємо новий об'єкт для бази
            var newBooking = new Booking
            {
                ClientId = SelectedClient.ClientId,
                PlaceId = SelectedPlace.PlaceId,
                BookTime = fullStartTime,
                BookLengthMinutes = BookingDuration,
                Status = "active" // Або "pending"
            };

            // 4. Зберігаємо в базу
            _db.Bookings.Add(newBooking);
            _db.SaveChanges();

            // 5. Оновлюємо список і закриваємо вікно
            LoadBookings();
            CloseAddSessionWindow();
        }

        [RelayCommand]
        private void CancelBooking(Booking booking)
        {
            if (booking == null) return;
            var bookingToCancel = _db.Bookings.Find(booking.BookId);
            if (bookingToCancel != null && bookingToCancel.Status != "cancel")
            {
                bookingToCancel.Status = "cancel";
                _db.SaveChanges();
                LoadBookings();
            }
        }

        [RelayCommand]
        private void OpenStartSessionWindow(Booking booking)
        {
            if (booking == null) return;
            BookingToStart = booking;
            SelectedAccountType = "club";
            IsStartSessionWindowOpen = true;
        }

        [RelayCommand]
        private void CloseStartSessionWindow()
        {
            IsStartSessionWindowOpen = false;
            BookingToStart = null;
        }


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

        [RelayCommand]
        private void StartSessionFromBooking()
        {
            if (BookingToStart == null) return;

            var tariff = GetTariffForPlace(SelectedPlace.PlaceId);
            if (tariff == null)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка: Тариф не знайдено для місця з ID {SelectedPlace.PlaceId}");
                return;
            }

            var newSession = new Session
            {
                ClientId = BookingToStart.ClientId,
                PlaceId = BookingToStart.PlaceId,
                StartSession = DateTime.Now,
                Status = "active",
                GameAccount = SelectedAccountType,
                TariffId = tariff.TariffId
            };

            var bookingInDb = _db.Bookings.Find(BookingToStart.BookId);
            if (bookingInDb != null)
            {
                bookingInDb.Status = "completed";
            }
        }

    }
}