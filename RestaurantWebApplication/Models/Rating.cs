using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantWebApplication.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public int ClientId { get; set; }

    [Range(1, 10, ErrorMessage = "Рейтинг повинен бути від 1 до 10")]
    public int Score { get; set; }
    [JsonIgnore]
    public virtual Client Client { get; set; } = null!;
    [JsonIgnore]
    public virtual Place Place { get; set; } = null!;
}
