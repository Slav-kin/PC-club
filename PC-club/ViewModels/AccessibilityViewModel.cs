using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore; // Обов'язково для Include
using PC_club.Models;
using PC_club.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace PC_club.ViewModels
{
    public partial class AccessibilityViewModel: ViewModelBase
    {
        private readonly PcClubContext _db;
        private readonly SessionEstimatesStore _estimatesStore;


        #region Timeline Variables

        // Поточна дата, яку показує графік (щоб можна було гортати дні)
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        // Список рядків для нашого XAML
        [ObservableProperty]
        private ObservableCollection<PlaceTimelineRow> _timelineRows = new();

        // Константи для розмірів (як ти і просив 40 на 52)
        private const double PixelsPerHour = 40.0;
        private const double PixelsPerMinute = PixelsPerHour / 60.0;

        #endregion

        public AccessibilityViewModel(PcClubContext db, SessionEstimatesStore estimatesStore)
        {
            _db = db;
            _estimatesStore = estimatesStore;

            GenerateTimeline();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            GenerateTimeline();
        }

        private void GenerateTimeline()
        {
            var newRows = new ObservableCollection<PlaceTimelineRow>();

            // Межі вибраного дня (наприклад: 20.02.2026 00:00:00 до 21.02.2026 00:00:00)
            DateTime dayStart = SelectedDate.Date;
            DateTime dayEnd = dayStart.AddDays(1);

            // 1. Отримуємо всі місця
            var places = _db.Places.ToList();

            // 2. Отримуємо всі сесії та бронювання (які перетинаються з вибраним днем)
            var allSessions = _db.Sessions.Include(s => s.Client).ToList();
            // Якщо маєш бронювання: var allBookings = _db.Bookings.Include(b => b.Client).ToList();

            foreach (var place in places)
            {
                var row = new PlaceTimelineRow { PlaceName = $"Місце #{place.PlaceId}" };

                // --- ДОДАЄМО СЕСІЇ ---
                var placeSessions = allSessions.Where(s => s.PlaceId == place.PlaceId).ToList();

                foreach (var session in placeSessions)
                {
                    if (!session.StartSession.HasValue) continue;

                    DateTime itemStart = session.StartSession.Value;
                    // Беремо час завершення: або фактичний, або орієнтовний (з нашого JSON)
                    DateTime itemEnd = session.EndSession ?? _estimatesStore.GetEstimate(session.SessionId) ?? itemStart.AddHours(1);

                    // ПЕРЕВІРКА 1: Чи потрапляє ця сесія у вибраний день взагалі?
                    if (itemEnd <= dayStart || itemStart >= dayEnd) continue;

                    // ПЕРЕВІРКА 2: Обрізка на межі опівночі (щоб блок не виліз за екран)
                    DateTime drawStart = itemStart < dayStart ? dayStart : itemStart;
                    DateTime drawEnd = itemEnd > dayEnd ? dayEnd : itemEnd;

                    // МАТЕМАТИКА ПІКСЕЛІВ
                    double marginMinutes = (drawStart - dayStart).TotalMinutes;
                    double durationMinutes = (drawEnd - drawStart).TotalMinutes;

                    row.Items.Add(new TimelineItem
                    {
                        Label = session.Client?.Nickname ?? "Гость",
                        MarginLeft = marginMinutes * PixelsPerMinute,
                        Width = durationMinutes * PixelsPerMinute,
                        Color = session.Status == "active" ? "#5C1B1B" : "#1B5C28", // Червоний якщо активна, синій якщо закрита
                        ToolTipText = $"Сесія: {itemStart:HH:mm} - {itemEnd:HH:mm}"
                    });
                }

                // --- ТУТ ТАК САМО МОЖНА ДОДАТИ БРОНЮВАННЯ (іншим кольором, наприклад жовтим #EAB308) ---

                newRows.Add(row);
            }

            TimelineRows = newRows;
        }
    }
}
