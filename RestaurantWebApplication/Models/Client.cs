using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantWebApplication.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
