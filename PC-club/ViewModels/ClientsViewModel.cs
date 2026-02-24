using System.Collections.ObjectModel;
using System.Linq;
using PC_club.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PC_club.ViewModels
{
    public partial class ClientsViewModel : ViewModelBase
    {
        public ObservableCollection<Client> Clients { get; set; } = new();

        // список доступних статусів для вибору
        public ObservableCollection<string> available_statuses { get; } = new() { "Активний", "Не активний" };

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

        public IRelayCommand openmodalcommand { get; }
        public IRelayCommand closemodalcommand { get; }
        public IRelayCommand saveclientcommand { get; }


        private readonly PcClubContext _db;

        public ClientsViewModel(PcClubContext db)
        {
            _db = db;

            openmodalcommand = new RelayCommand(openmodal);
            closemodalcommand = new RelayCommand(closemodal);
            saveclientcommand = new RelayCommand(saveclient);

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
                (c.LastName != null && c.LastName.Contains(searchtext, System.StringComparison.OrdinalIgnoreCase)) ||
                (c.FirstName != null && c.FirstName.Contains(searchtext, System.StringComparison.OrdinalIgnoreCase)) ||
                (c.Phone != null && c.Phone.Contains(searchtext, System.StringComparison.OrdinalIgnoreCase)) ||
                (c.Nickname != null && c.Nickname.Contains(searchtext, System.StringComparison.OrdinalIgnoreCase))
            );

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
    }
}