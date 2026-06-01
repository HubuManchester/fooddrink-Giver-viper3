using SQLite;

namespace FoodAndDrink.Models
{
    [Table("FoodItems")]
    public class FoodItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PriceTier { get; set; } = "$";
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        [Indexed]
        public int CategoryId { get; set; }
        public string Cuisine { get; set; } = string.Empty;
        public double Distance { get; set; }
        public int Calories { get; set; }
        public string Allergens { get; set; } = string.Empty;
        public string AvailableTime { get; set; } = string.Empty;
        public bool IsTrending { get; set; }
        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsFavorite { get; set; }

        [Ignore]
        public string IconEmoji { get; set; } = "🍽️";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ViewedAt { get; set; }
    }
}
