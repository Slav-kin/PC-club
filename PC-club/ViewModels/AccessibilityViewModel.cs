using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using PC_club.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PC_club.ViewModels
{
    public partial class AccessibilityViewModel : ViewModelBase
    {
        private readonly PcClubContext _db;
        private readonly SessionEstimatesStore _estimatesStore;

        #region Timeline Variables

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<PlaceTimelineRow> _timelineRows = new();

        private const double PixelsPerHour = 40.0;
        private const double PixelsPerMinute = PixelsPerHour / 60.0;

        #endregion

        #region Navigation Commands

        [RelayCommand]
        private void PreviousDay()
        {
            // Віднімаємо 1 день від поточної вибраної дати
            SelectedDate = SelectedDate.AddDays(-1);
        }

        [RelayCommand]
        private void NextDay()
        {
            // Додаємо 1 день
            SelectedDate = SelectedDate.AddDays(1);
        }

        [RelayCommand]
        private void GoToToday()
        {
            // Повертаємо на сьогоднішній день
            SelectedDate = DateTime.Today;
        }

        #endregion

        #region couters for short stats

        [ObservableProperty]
        private int _countActiveSessions;

        [ObservableProperty]
        private int _countTodayTotalSessions;

        [ObservableProperty]
        private int _countBookingsToday;

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

            // Межі вибраного дня (наприклад: від 00:00 до 23:59:59)
            DateTime dayStart = SelectedDate.Date;
            DateTime dayEnd = dayStart.AddDays(1);

            // 1. Завантажуємо всі необхідні дані з БД
            var places = _db.Places.ToList();
            var allSessions = _db.Sessions.Include(s => s.Client).ToList();

            // РОЗБЛОКУВАЛИ БРОНЮВАННЯ: Завантажуємо їх з клієнтами
            var allBookings = _db.Bookings.Include(b => b.Client).ToList();

            // Змінні для статистики знизу екрана
            int activeSessions = 0;
            int todaySessions = 0;
            int todayBookings = 0;

            foreach (var place in places)
            {
                var row = new PlaceTimelineRow { PlaceName = $"Місце #{place.PlaceId}" };

                // ==========================================
                // 1. МАЛЮЄМО СЕСІЇ
                // ==========================================
                var placeSessions = allSessions.Where(s => s.PlaceId == place.PlaceId).ToList();

                foreach (var session in placeSessions)
                {
                    if (!session.StartSession.HasValue) continue;

                    DateTime itemStart = session.StartSession.Value;

                    // Шукаємо кінець: фактичний -> JSON оцінка -> або хоча б +1 година
                    DateTime itemEnd = session.EndSession ?? _estimatesStore.GetEstimate(session.SessionId) ?? itemStart.AddHours(1);

                    // Якщо сесія ВЗАГАЛІ не потрапляє у вибраний день - пропускаємо
                    if (itemEnd <= dayStart || itemStart >= dayEnd) continue;

                    // Збираємо статистику (для нижніх карток)
                    if (session.Status == "active") activeSessions++;
                    if (itemStart.Date == dayStart.Date) todaySessions++;

                    // ОБРІЗКА (щоб блок не вилазив за межі 00:00 та 24:00)
                    DateTime drawStart = itemStart < dayStart ? dayStart : itemStart;
                    DateTime drawEnd = itemEnd > dayEnd ? dayEnd : itemEnd;

                    double marginMinutes = (drawStart - dayStart).TotalMinutes;
                    double durationMinutes = (drawEnd - drawStart).TotalMinutes;

                    row.Items.Add(new TimelineItem
                    {
                        Label = session.Client?.Nickname ?? "Гость",
                        MarginLeft = marginMinutes * PixelsPerMinute,
                        Width = durationMinutes * PixelsPerMinute,
                        Color = session.Status == "active" ? "#82181a" : "#5C1B1B", // Активна (червоний) / Закрита (зелений)
                        ToolTipText = $"Сесія: {itemStart:HH:mm} - {itemEnd:HH:mm}"
                    });
                }

                // ==========================================
                // 2. МАЛЮЄМО БРОНЮВАННЯ
                // ==========================================
                // Беремо всі бронювання для цього місця, які ще не "completed" (бо completed стали сесіями)
                var placeBookings = allBookings.Where(b => b.PlaceId == place.PlaceId && b.Status != "completed").ToList();

                foreach (var booking in placeBookings)
                {
                    // УВАГА: Якщо в тебе поле називається не StartTime, а інакше - зміни тут!
              

                    DateTime bookStart = booking.BookTime;

                    // УВАГА: Ми обговорювали, що твоє BookLengthMinutes насправді зберігає ГОДИНИ. 
                    // Тому я використовую AddHours. Якщо там хвилини - зміни на AddMinutes.
                    DateTime bookEnd = bookStart.AddHours(booking.BookLengthMinutes);

                    // Якщо бронь не потрапляє у вибраний день - пропускаємо
                    if (bookEnd <= dayStart || bookStart >= dayEnd) continue;

                    // Статистика
                    if (bookStart.Date == dayStart.Date) todayBookings++;

                    // ОБРІЗКА
                    DateTime drawStart = bookStart < dayStart ? dayStart : bookStart;
                    DateTime drawEnd = bookEnd > dayEnd ? dayEnd : bookEnd;

                    double marginMinutes = (drawStart - dayStart).TotalMinutes;
                    double durationMinutes = (drawEnd - drawStart).TotalMinutes;

                    row.Items.Add(new TimelineItem
                    {
                        Label = booking.Client?.Nickname ?? "Бронь",
                        MarginLeft = marginMinutes * PixelsPerMinute,
                        Width = durationMinutes * PixelsPerMinute,
                        Color = "#733e0a", // Коричневий для бронювань
                        ToolTipText = $"Бронь: {bookStart:HH:mm} - {bookEnd:HH:mm}"
                    });
                }

                newRows.Add(row);
            }

            // Оновлюємо список на екрані
            TimelineRows = newRows;

            // Оновлюємо статистику у футері
            //CountActiveSessions = activeSessions;
            //CountTodayTotalSessions = todaySessions;
            //CountBookingsToday = todayBookings;
        }
    }
}