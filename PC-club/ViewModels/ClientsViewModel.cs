using System.Collections.ObjectModel;
using System.Linq;
using PC_club.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PC_club.ViewModels
{
    public partial class ClientsViewModel : ViewModelBase
    {
        // Колекція, яку буде показувати таблиця
        public ObservableCollection<Client> Clients { get; set; } = new();

        public ClientsViewModel()
        {
            LoadClients();
        }

        private void LoadClients()
        {
            // Підключаємося до бази та беремо список клієнтів
            using (var db = new PcClubContext())
            {
                var clientsFromDb = db.Clients.ToList();

                Clients.Clear();
                foreach (var client in clientsFromDb)
                {
                    Clients.Add(client);
                }
            }
        }
    }
}