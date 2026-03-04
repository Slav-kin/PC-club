using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PC_club.Models // Або PC_club.ViewModels, залежно від того, де ти його створиш
{
    // Окремий кольоровий блок на графіку (одна сесія або бронь)
    public class TimelineItem
    {
        public string Label { get; set; } = string.Empty; // Текст всередині (напр. ім'я клієнта)
        public double Width { get; set; } // Ширина в пікселях
        public double MarginLeft { get; set; } // Відступ від 00:00 в пікселях
        public string Color { get; set; } = "#8B5CF6"; // Колір блоку (можна передавати HEX)
        public string ToolTipText { get; set; } = string.Empty; // Текст при наведенні мишки
    }

    // Один рядок графіка (один комп'ютер/місце)
    public class PlaceTimelineRow
    {
        public string PlaceName { get; set; } = string.Empty;
        // Список усіх блоків (сесій та броней), які належать цьому місцю
        public ObservableCollection<TimelineItem> Items { get; set; } = new();
    }
}