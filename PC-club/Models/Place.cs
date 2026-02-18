using System;
using System.Collections.Generic;

namespace PC_club.Models;

public partial class Place
{
    public int PlaceId { get; set; }

    public int? PlaceNumber { get; set; }

    public int? CategoryId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
