using System;
using System.Collections.Generic;

namespace PC_club.Models;

public partial class Booking
{
    public int BookId { get; set; }

    public int ClientId { get; set; }

    public int PlaceId { get; set; }

    public DateTime BookTime { get; set; }

    public int BookLengthMinutes { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Place Place { get; set; } = null!;
}
