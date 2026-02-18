using System;
using System.Collections.Generic;

namespace PC_club.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
