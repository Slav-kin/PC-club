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

        public ClientsViewModel()
        {
            openmodalcommand = new RelayCommand(openmodal);
            closemodalcommand = new RelayCommand(closemodal);
            saveclientcommand = new RelayCommand(saveclient);
            loadclients();
        }

        private void loadclients()
        {
            using (var db = new PcClubContext())
            {
                var clientsfromdb = db.Clients.ToList();

                Clients.Clear();
                foreach (var client in clientsfromdb)
                {
                    Clients.Add(client);
                }
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
                using (var db = new PcClubContext())
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

                    db.Clients.Add(newclient);
                    db.SaveChanges();

                    Clients.Add(newclient);
                }

                closemodal();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("помилка збереження: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("деталі: " + ex.InnerException.Message);
                }
            }
        }
    }
}