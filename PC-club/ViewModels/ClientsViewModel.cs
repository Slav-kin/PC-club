using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PC_club.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PC_club.ViewModels
{
    public partial class ClientsViewModel : ViewModelBase
    {
        private ObservableCollection<Client> _clients = new();
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        // список доступних статусів для вибору
        public ObservableCollection<string> AvailableStatuses { get; } = new() { "Активний", "Не активний" };

        // список для збереження всіх клієнтів з бази для роботи пошуку
        private System.Collections.Generic.List<Client> allclients = new();

        private string _searchtext = "";
        public string searchtext
        {
            get => _searchtext;
            set
            {
                SetProperty(ref _searchtext, value);
                updatelist();
            }
        }

        private bool _ismodalopen;
        public bool ismodalopen
        {
            get => _ismodalopen;
            set => SetProperty(ref _ismodalopen, value);
        }

        private bool _IsEditClientWindowOpen;

        public bool IsEditClientWindowOpen
        {
            get => _IsEditClientWindowOpen;
            set => SetProperty(ref _IsEditClientWindowOpen, value);

        }

        private Client? _SelectedClident;
        public Client? SelectedClient
        {
            get => _SelectedClident;
            set => SetProperty(ref _SelectedClident, value);
        }



        private string _newfirstname = "";
        public string newfirstname
        {
            get => _newfirstname;
            set => SetProperty(ref _newfirstname, value);
        }

        private string _newlastname = "";
        public string newlastname
        {
            get => _newlastname;
            set => SetProperty(ref _newlastname, value);
        }

        private string _newnickname = "";
        public string newnickname
        {
            get => _newnickname;
            set => SetProperty(ref _newnickname, value);
        }

        private string _newphone = "";
        public string newphone
        {
            get => _newphone;
            set => SetProperty(ref _newphone, value);
        }

        private string _newemail = "";
        public string newemail
        {
            get => _newemail;
            set => SetProperty(ref _newemail, value);
        }

        private string _newbalance = "";
        public string newbalance
        {
            get => _newbalance;
            set => SetProperty(ref _newbalance, value);
        }

        // змінна для збереження вибраного статусу
        private string _newstatus = "Активний";
        public string newstatus
        {
            get => _newstatus;
            set => SetProperty(ref _newstatus, value);
        }


        private string _newFirstName = "";
        public string NewFirstName { get => _newFirstName; set => SetProperty(ref _newFirstName, value); }

        private string _newLastName = "";
        public string NewLastName { get => _newLastName; set => SetProperty(ref _newLastName, value); }

        private string _newNickname = "";
        public string NewNickname { get => _newNickname; set => SetProperty(ref _newNickname, value); }

        private string _newPhone = "";
        public string NewPhone { get => _newPhone; set => SetProperty(ref _newPhone, value); }

        private string _newEmail = "";
        public string NewEmail { get => _newEmail; set => SetProperty(ref _newEmail, value); }

        private string _newBalance = "";
        public string NewBalance { get => _newBalance; set => SetProperty(ref _newBalance, value); }

        private string _newStatus = "Активний";
        public string NewStatus { get => _newStatus; set => SetProperty(ref _newStatus, value); }


        public IRelayCommand openmodalcommand { get; }
        public IRelayCommand closemodalcommand { get; }
        public IRelayCommand saveclientcommand { get; }

        public IRelayCommand<Client> OpenEditClientWindow { get; }
        public IRelayCommand CloseEditClientWindow { get; }
        public IRelayCommand SaveEditClientWindow { get; }

        private readonly PcClubContext _db;

        public ClientsViewModel(PcClubContext db)
        {
            _db = db;

            openmodalcommand = new RelayCommand(openmodal);
            closemodalcommand = new RelayCommand(closemodal);
            saveclientcommand = new RelayCommand(saveclient);


            OpenEditClientWindow = new RelayCommand<Client>(OpenEditModel);
            CloseEditClientWindow = new RelayCommand(CloseEditModel);
            SaveEditClientWindow = new RelayCommand(SaveEditModel);

            loadclients();
        }

        private void loadclients()
        {
            var clientsfromdb = _db.Clients.ToList();
            allclients.Clear();
            foreach (var client in clientsfromdb)
            {
                allclients.Add(client);
            }
            updatelist();
        }
        private void updatelist()
        {
            var q = allclients.Where(c =>    
        string.IsNullOrWhiteSpace(searchtext) ||
        (c.LastName != null && c.LastName.Contains(searchtext, StringComparison.OrdinalIgnoreCase)) ||
        (c.FirstName != null && c.FirstName.Contains(searchtext, StringComparison.OrdinalIgnoreCase)) ||
        (c.Phone != null && c.Phone.Contains(searchtext, StringComparison.OrdinalIgnoreCase)) ||
        (c.Nickname != null && c.Nickname.Contains(searchtext, StringComparison.OrdinalIgnoreCase))
         ).OrderBy(c => c.LastName);

            Clients = new ObservableCollection<Client>(q);
            // сортуємо за прізвищем
            q = q.OrderBy(c => c.LastName);

            Clients.Clear();
            foreach (var item in q)
            {
                Clients.Add(item);
            }
        }
        private void openmodal()
        {
            ismodalopen = true;
        }

        private void OpenEditModel()
        {
            IsEditClientWindowOpen = true;
        }

        private void closemodal()
        {
            ismodalopen = false;
            newfirstname = "";
            newlastname = "";
            newnickname = "";
            newphone = "";
            newemail = "";
            newbalance = "";
            newstatus = "Активний";
        }
        private void ClearInputs()
        {
            NewFirstName = NewLastName = NewNickname = NewPhone = NewEmail = NewBalance = "";
            NewStatus = "Активний";
            SelectedClient = null;
        }
        private void saveclient()
        {
            if (string.IsNullOrWhiteSpace(newfirstname) || string.IsNullOrWhiteSpace(newlastname))
            {
                return;
            }

            // перетворюємо введений рядок у десяткове число. якщо не вийшло (ввели букви), буде null
            decimal? parsedbalance = null;
            if (decimal.TryParse(newbalance, out decimal result))
            {
                parsedbalance = result;
            }

            try
            {
                {
                    string dbstatus = newstatus == "Активний" ? "active" : "inactive";

                    var newclient = new Client
                    {
                        FirstName = newfirstname,
                        LastName = newlastname,
                        Nickname = string.IsNullOrWhiteSpace(newnickname) ? null : newnickname,
                        Phone = string.IsNullOrWhiteSpace(newphone) ? null : newphone,
                        Email = string.IsNullOrWhiteSpace(newemail) ? null : newemail,
                        Balance = parsedbalance,
                        Status = dbstatus // передаємо конвертований статус
                    };

                    _db.Clients.Add(newclient);
                    _db.SaveChanges();

                    allclients.Add(newclient);
                    updatelist();



                    closemodal();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("помилка: " + ex.Message);
            }
        }

        private void OpenEditModel(Client? client)
        {
            if (client == null) return;
            SelectedClient = client;

            NewFirstName = client.FirstName;
            NewLastName = client.LastName;
            NewNickname = client.Nickname ?? "";
            NewPhone = client.Phone ?? "";
            NewEmail = client.Email ?? "";
            NewBalance = client.Balance?.ToString() ?? "0";
            NewStatus = client.Status == "active" ? "Активний" : "Не активний";

            IsEditClientWindowOpen = true;
        }

        private void CloseEditModel()
        {
            IsEditClientWindowOpen = false;
            ClearInputs();
        }

        private void SaveEditModel()
        {
            if (SelectedClient == null) return;

            decimal.TryParse(NewBalance, out decimal balance);

            try
            {
                SelectedClient.FirstName = NewFirstName;
                SelectedClient.LastName = NewLastName;
                SelectedClient.Nickname = NewNickname;
                SelectedClient.Phone = NewPhone;
                SelectedClient.Email = NewEmail;
                SelectedClient.Balance = balance;
                SelectedClient.Status = NewStatus == "Активний" ? "active" : "inactive";

                _db.SaveChanges();
                loadclients();
                CloseEditModel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка редагування: {ex.Message}");
            }
        }
    }
}