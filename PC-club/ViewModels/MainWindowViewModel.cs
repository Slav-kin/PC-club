using Microsoft.EntityFrameworkCore;
using PC_club.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PC_club.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Допоміжний метод для оновлення властивостей
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 1. Список, який Avalonia буде відображати на екрані
        // Ми використовуємо ObservableCollection, щоб UI автоматично оновлювався при змінах
        public ObservableCollection<Place> PCPlaces { get; set; } = new();

        // Додано властивість Client, щоб XAML міг прив'язатися до Client.Nickname
        private Client _client = new Client();
        public Client Client
        {
            get => _client;
            set => SetProperty(ref _client, value);
        }

        // Конструктор ViewModel
        public MainWindowViewModel()
        {
            // Викликаємо завантаження даних відразу при запуску програми
            RefreshHall();

            // Ініціалізуємо об'єкт Client для прив'язки в XAML
            using (var db = new PcClubContext())
            {
                Client = db.Clients.FirstOrDefault() ?? new Client();
            }
        }

        // 2. Метод для завантаження або оновлення списку місць з бази
        public void RefreshHall()
        {
            using (var db = new PcClubContext())
            {
                // Оновлюємо список місць для схеми залу
                var data = db.Places.Include(p => p.Category).ToList();
                PCPlaces.Clear();
                foreach (var item in data) PCPlaces.Add(item);

                // Рахуємо активних учасників (ті, хто мають статус active)
                ActiveClientsCount = db.Clients.Count(c => c.Status == "active");

                // Рахуємо зайняті місця (кількість активних ігрових сесій)
                OccupiedPlacesCount = db.Sessions.Count(s => s.Status == "active");
            }
        }
        private int _activeClientsCount;
        public int ActiveClientsCount
        {
            get => _activeClientsCount;
            set
            {
                if (_activeClientsCount != value)
                {
                    _activeClientsCount = value;
                    OnPropertyChanged(); // Повідомляємо UI про зміни
                }
            }
        }

        private int _occupiedPlacesCount;
        public int OccupiedPlacesCount
        {
            get => _occupiedPlacesCount;
            set
            {
                if (_occupiedPlacesCount != value)
                {
                    _occupiedPlacesCount = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
