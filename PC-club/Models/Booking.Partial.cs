using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_club.Models
{
    public partial class Booking
    {
        // Ця властивість буде повертати "Сьогодні" або дату
        public string DisplayDate
        {
            get
            {
                if (BookTime.Date == DateTime.Today)
                {
                    return "Сьогодні";
                }
                if (BookTime.Date == DateTime.Today.AddDays(1))
                {
                    return "Завтра"; // Бонусом можна додати і "Завтра"
                }
                return BookTime.ToString("dd.MM.yyyy");
            }
        }

        // Також зручно мати окремо час
        public string DisplayTime => BookTime.ToString("HH:mm");

        public bool CanStartSessionFromBooking
        {
            get
            {
                if (Status != "active") return false;
                var now = DateTime.Now;
                return now >= BookTime.AddMinutes(-5) && now <= BookTime.AddMinutes(15);
            }
        }
    }
}
