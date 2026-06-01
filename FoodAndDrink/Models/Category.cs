using SQLite;

namespace FoodAndDrink.Models
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ColorToken { get; set; } = "Primary";
        public int SortOrder { get; set; }
        public string IconEmoji { get; set; } = string.Empty;
    }
}
