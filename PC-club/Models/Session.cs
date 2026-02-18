using System;
using System.Collections.Generic;

namespace PC_club.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int? ClientId { get; set; }

    public int? PlaceId { get; set; }

    public int? TariffId { get; set; }

    public DateTime? StartSession { get; set; }

    public DateTime? EndSession { get; set; }

    public string? Status { get; set; }

    public string? GameAccount { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Place? Place { get; set; }

    public virtual Tariff? Tariff { get; set; }
}
