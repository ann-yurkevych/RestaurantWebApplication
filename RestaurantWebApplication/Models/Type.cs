using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantWebApplication.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
