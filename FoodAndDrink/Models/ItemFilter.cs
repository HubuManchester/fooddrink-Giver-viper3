namespace FoodAndDrink.Models
{
    public class ItemFilter
    {
        public int? CategoryId { get; set; }
        public string? PriceTier { get; set; }
        public double? MinRating { get; set; }
        public double? MaxDistance { get; set; }
        public string? Cuisine { get; set; }
        public string? SortBy { get; set; }
    }
}
