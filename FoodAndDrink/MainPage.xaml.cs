using CommunityToolkit.Maui.Core;
using FoodAndDrink.Models;
using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class MainPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private readonly CategoryService _categoryService;

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
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var allItems = await _foodItemService.GetAllAsync();
                var categories = await _categoryService.GetAllAsync();

                var trendingItems = allItems.Where(i => i.IsTrending)
                    .OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
                var nearYouItems = allItems.Where(i => !i.IsTrending)
                    .OrderBy(i => i.Distance).Take(3).ToList();
                var recommendedItems = allItems.Where(i => i.IsRecommended)
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

        private async void OnMainSearchPressed(object sender, EventArgs e)
        {
            var keyword = MainSearchBar.Text?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
                return;

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
                try
                {
                    HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
                catch
                {
                    // haptic not supported
                }

                await _foodItemService.ToggleFavoriteAsync(itemId);
                await DisplayAlert("Favorites", "Your favorites have been updated.", "OK");
                await LoadDataAsync();
            }
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
