using CommunityToolkit.Maui.Core;
using FoodAndDrink.Models;
using FoodAndDrink.Services;

namespace FoodAndDrink
{
    [QueryProperty(nameof(ItemIdParam), "itemId")]
    public partial class DetailPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private FoodItem? _item;

        public string? ItemIdParam
        {
            set
            {
                if (int.TryParse(value, out var id))
                    _ = LoadItemAsync(id);
            }
        }

        public DetailPage(FoodItemService foodItemService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
        }

        private async Task LoadItemAsync(int id)
        {
            try
            {
                _item = await _foodItemService.GetByIdAsync(id);
                if (_item is null) return;

                ItemName.Text = _item.Name;
                ItemSubtitle.Text = _item.Subtitle;
                DescriptionText.Text = _item.Description;
                RatingLabel.Text = $"★ {_item.Rating:F1} ({_item.ReviewCount} reviews)";
                PriceChipLabel.Text = _item.PriceTier;
                DistanceChipLabel.Text = $"{_item.Distance:F1} mi";
                CuisineValue.Text = _item.Cuisine;
                CaloriesValue.Text = $"{_item.Calories} kcal";
                AllergensValue.Text = string.IsNullOrEmpty(_item.Allergens) ? "None" : _item.Allergens;
                AvailableValue.Text = _item.AvailableTime;

                // Hero content
                HeroEmoji.Text = _item.IconEmoji;
                HeroCategory.Text = _item.Cuisine;
                var colors = new[] { "PrimaryLight", "SecondaryLight", "TertiaryLight" };
                HeroImage.BackgroundColor = (Color)Application.Current!.Resources[colors[_item.CategoryId % colors.Length]];

                UpdateFavoriteButton();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] Load error: {ex.Message}");
            }
        }

        private void UpdateFavoriteButton()
        {
            if (_item is null) return;

            FavoriteBtnContent.Text = _item.IsFavorite ? "♥" : "♡";
            FavoriteBtn.BackgroundColor = _item.IsFavorite
                ? (Color)Application.Current!.Resources["Primary"]
                : (Color)Application.Current!.Resources["PrimaryLight"];
        }

        private async void OnFavoriteClicked(object sender, EventArgs e)
        {
            if (_item is null) return;

            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch { }

            await _foodItemService.ToggleFavoriteAsync(_item.Id);
            _item.IsFavorite = !_item.IsFavorite;
            UpdateFavoriteButton();
            await DisplayAlert("Favorites", _item.IsFavorite ? "Added to favorites!" : "Removed from favorites.", "OK");
        }

        private async void OnOrderClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Order", $"Ordering {_item?.Name}...", "OK");
        }

        private async void OnDirectionsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Directions", $"Getting directions for {_item?.Name}...", "OK");
        }
    }
}
