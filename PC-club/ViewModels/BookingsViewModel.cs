using CommunityToolkit.Mvvm.ComponentModel;
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
        private ObservableCollection<Booking> _booking = new();

        #region ShortStats

        [ObservableProperty]
        private int _countActiveBookings;

        [ObservableProperty]
        private int _countWaitingAcsess;

        #endregion
    }
}
