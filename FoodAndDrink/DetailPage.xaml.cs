using FoodAndDrink.Models;
using FoodAndDrink.Services;

namespace FoodAndDrink
{
    [QueryProperty(nameof(ItemIdParam), "itemId")]
    public partial class DetailPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private FoodItem? _item;
        private CancellationTokenSource? _loadCts;

        public string? ItemIdParam
        {
            set
            {
                if (int.TryParse(value, out var id))
                {
                    _loadCts?.Cancel();
                    _loadCts = new CancellationTokenSource();
                    _ = LoadItemAsync(id, _loadCts.Token);
                }
            }
        }

        public DetailPage(FoodItemService foodItemService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _loadCts?.Cancel();
        }

        private async Task LoadItemAsync(int id, CancellationToken ct)
        {
            try
            {
                _item = await _foodItemService.GetByIdAsync(id);
                if (_item is null || ct.IsCancellationRequested) return;

                ItemName.Text = _item.Name;
                ItemSubtitle.Text = _item.Subtitle;
                DescriptionText.Text = _item.Description;
                RatingLabel.Text = $"★ {_item.Rating:F1} ({_item.ReviewCount} reviews)";
                PriceChipLabel.Text = _item.PriceTier;
                DistanceChipLabel.Text = $"{_item.Distance:F1} mi";
                CuisineValue.Text = _item.Cuisine;
                CaloriesValue.Text = $"{_item.Calories} kcal";
                AllergensValue.Text = string.IsNullOrEmpty(_item.Allergens) ? "None" : _item.Allergens;
                var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
                AllergensValue.TextColor = string.IsNullOrEmpty(_item.Allergens)
                    ? (Color)Application.Current!.Resources[isDark ? "SecondaryDark" : "Secondary"]
                    : (Color)Application.Current!.Resources[isDark ? "TertiaryDark" : "Warning"];
                AvailableValue.Text = _item.AvailableTime;

                if (ct.IsCancellationRequested) return;

                HeroImageContent.Source = _item.ImageUrl;
                var colors = new[] { "PrimaryLight", "SecondaryLight", "TertiaryLight" };
                HeroImage.BackgroundColor = (Color)Application.Current!.Resources[colors[_item.CategoryId % colors.Length]];

                UpdateFavoriteButton();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] Load error: {ex.Message}");
                await DisplayAlert("Error", "Failed to load item details. Please go back and try again.", "OK");
            }
        }

        private void UpdateFavoriteButton()
        {
            if (_item is null) return;

            var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
            FavoriteBtnContent.Text = _item.IsFavorite ? "♥" : "♡";
            FavoriteBtn.BackgroundColor = _item.IsFavorite
                ? (Color)Application.Current!.Resources[isDark ? "PrimaryDark" : "Primary"]
                : (Color)Application.Current!.Resources["PrimaryLight"];
        }

        private async void OnFavoriteClicked(object sender, EventArgs e)
        {
            if (_item is null) return;

            try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
            catch { }

            await _foodItemService.ToggleFavoriteAsync(_item.Id);
            _item.IsFavorite = !_item.IsFavorite;
            UpdateFavoriteButton();
        }

        private async void OnReadAloudClicked(object sender, EventArgs e)
        {
            if (_item is null) return;

            try
            {
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                if (locales is null || !locales.Any())
                {
                    await DisplayAlert("Not Available",
                        "No text-to-speech engine found on this device.\n\n" +
                        "Please install a TTS engine (e.g. Google Text-to-Speech) from your app store, " +
                        "then try again.", "OK");
                    return;
                }

                var textToRead = $"{_item.Name}. {_item.Description}. ";

                if (!string.IsNullOrEmpty(_item.Allergens))
                    textToRead += $"Allergens: {_item.Allergens.Replace(",", ", ")}. ";
                else
                    textToRead += "No common allergens. ";

                textToRead += $"Price: ¥{_item.Price:F0}. Calories: {_item.Calories}.";

                ReadAloudBtn.Text = "🔊 Speaking...";
                ReadAloudBtn.IsEnabled = false;

                await TextToSpeech.Default.SpeakAsync(textToRead);

                ReadAloudBtn.Text = "🔊 Read Aloud";
                ReadAloudBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] TTS error: {ex.Message}");
                ReadAloudBtn.Text = "🔊 Read Aloud";
                ReadAloudBtn.IsEnabled = true;
                await DisplayAlert("Error", "Text-to-speech is not available on this device.", "OK");
            }
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (_item is null) return;

            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = $"Check out {_item.Name} - {_item.Subtitle}! ★ {_item.Rating:F1} | ¥{_item.Price:F0}",
                    Title = $"Share {_item.Name}"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] Share error: {ex.Message}");
                await DisplayAlert("Error", "Sharing is not available on this device.", "OK");
            }
        }

        private async void OnOrderClicked(object sender, EventArgs e)
        {
            if (_item is null) return;

            try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); }
            catch { }

            bool confirm = await DisplayAlert("Order Now",
                $"Order {_item.Name}?\nPrice: ¥{_item.Price:F0}",
                "Order", "Cancel");

            if (!confirm) return;

            try
            {
                try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
                catch { }

                var deepLink = $"meituanwaimai://waimai.meituan.com/search?keyword={Uri.EscapeDataString(_item.Name)}";
                var opened = await Launcher.Default.TryOpenAsync(deepLink);

                if (!opened)
                {
                    var searchUrl = $"https://www.google.com/search?q={Uri.EscapeDataString(_item.Name + " order delivery")}";
                    await Browser.Default.OpenAsync(searchUrl, BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] Order error: {ex.Message}");
                await DisplayAlert("Order",
                    $"Could not open ordering service.\nSearch for \"{_item.Name}\" in your food delivery app.",
                    "OK");
            }
        }

        private async void OnDirectionsClicked(object sender, EventArgs e)
        {
            if (_item is null) return;

            try
            {
                try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
                catch { }

                var placeName = $"{_item.Name} {_item.Subtitle}";
                var placeLocation = new Placemark { Thoroughfare = placeName };

                try
                {
                    await Map.Default.OpenAsync(placeLocation, new MapLaunchOptions
                    {
                        Name = placeName,
                        NavigationMode = NavigationMode.Driving
                    });
                }
                catch (Exception)
                {
                    var mapsUrl = $"https://www.google.com/maps/search/{Uri.EscapeDataString(placeName)}";
                    await Browser.Default.OpenAsync(mapsUrl, BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] Maps error: {ex.Message}");
                await DisplayAlert("Directions",
                    $"Could not open maps.\nSearch for \"{_item.Name} {_item.Subtitle}\" in your maps app.",
                    "OK");
            }
        }
    }
}
