using FoodAndDrink.Models;
using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class MainPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private readonly CategoryService _categoryService;
        private List<FoodItem> _allItems = new();
        private const double ShakeThreshold = 1.2;
        private bool _isShakeCooldown;

        public MainPage(FoodItemService foodItemService, CategoryService categoryService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
            _categoryService = categoryService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDataAsync();
            StartShakeDetection();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopShakeDetection();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _allItems = await _foodItemService.GetAllAsync();
                var categories = await _categoryService.GetAllAsync();

                var trendingItems = _allItems.Where(i => i.IsTrending)
                    .OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
                var nearYouItems = _allItems.OrderBy(i => i.Distance).Take(3).ToList();
                var recommendedItems = _allItems.Where(i => i.IsRecommended)
                    .OrderBy(_ => Random.Shared.Next()).Take(3).ToList();

                BindableLayout.SetItemsSource(TrendingStack, trendingItems);
                BindableLayout.SetItemsSource(NearYouStack, nearYouItems);
                BindableLayout.SetItemsSource(CategoryPillsLayout, categories);
                BindableLayout.SetItemsSource(RecommendedStack, recommendedItems);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Load error: {ex.Message}");
            }
        }

        private void StartShakeDetection()
        {
            if (!Accelerometer.Default.IsSupported)
                return;

            try
            {
                Accelerometer.Default.ReadingChanged += OnAccelerometerReading;
                Accelerometer.Default.Start(SensorSpeed.UI);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Accelerometer error: {ex.Message}");
            }
        }

        private void StopShakeDetection()
        {
            try
            {
                Accelerometer.Default.ReadingChanged -= OnAccelerometerReading;
                Accelerometer.Default.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Accelerometer stop error: {ex.Message}");
            }
        }

        private async void OnAccelerometerReading(object? sender, AccelerometerChangedEventArgs e)
        {
            var reading = e.Reading.Acceleration;
            var magnitude = Math.Sqrt(
                reading.X * reading.X +
                reading.Y * reading.Y +
                reading.Z * reading.Z);

            if (magnitude > ShakeThreshold && !_isShakeCooldown)
            {
                _isShakeCooldown = true;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); }
                    catch { }

                    await ShowShakeRecommendation();
                });

                await Task.Delay(2000);
                _isShakeCooldown = false;
            }
        }

        private async Task ShowShakeRecommendation()
        {
            if (_allItems.Count == 0) return;

            var randomItem = _allItems[Random.Shared.Next(_allItems.Count)];
            bool view = await DisplayAlert(
                "Shake Detected!",
                $"How about trying:\n\n{randomItem.IconEmoji} {randomItem.Name}\n{randomItem.Subtitle}\n★ {randomItem.Rating:F1}",
                "View Details", "Maybe Later");

            if (view)
            {
                await Shell.Current.GoToAsync($"DetailPage?itemId={randomItem.Id}");
            }
        }

        private async void OnMainSearchPressed(object sender, EventArgs e)
        {
            var keyword = MainSearchBar.Text?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                Vibrate();
                return;
            }

            await Shell.Current.GoToAsync($"ItemListPage?search={Uri.EscapeDataString(keyword)}");
        }

        private async void OnItemTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int itemId)
            {
                await Shell.Current.GoToAsync($"DetailPage?itemId={itemId}");
            }
        }

        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int catId)
            {
                await Shell.Current.GoToAsync($"ItemListPage?categoryId={catId}");
            }
        }

        private async void OnFavoriteTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int itemId)
            {
                Vibrate();
                await _foodItemService.ToggleFavoriteAsync(itemId);
                await LoadDataAsync();
            }
        }

        private static void Vibrate()
        {
            try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
            catch { }
        }

        private async void OnTrendingSeeAllClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ItemListPage?sortBy=rating");
        }

        private async void OnNearYouSeeAllClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ItemListPage?sortBy=distance");
        }
    }
}
