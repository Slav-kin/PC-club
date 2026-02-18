using System;
using System.Collections.Generic;

namespace PC_club.Models;

public partial class Tariff
{
    public int TariffId { get; set; }

    public string? TariffName { get; set; }

    public decimal? TariffPrice { get; set; }

    public string? TariffConfiguration { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
