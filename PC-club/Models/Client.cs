using System;
using System.Collections.Generic;

namespace PC_club.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Nickname { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public decimal? Balance { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
