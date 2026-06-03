using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class FavoritesPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;

        public FavoritesPage(FoodItemService foodItemService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFavoritesAsync();
        }

        private async Task LoadFavoritesAsync()
        {
            try
            {
                var favorites = await _foodItemService.GetFavoritesAsync();

                if (favorites.Count == 0)
                {
                    EmptyState.IsVisible = true;
                    BindableLayout.SetItemsSource(FavoritesListStack, new List<Models.FoodItem>());
                    return;
                }

                EmptyState.IsVisible = false;
                BindableLayout.SetItemsSource(FavoritesListStack, favorites);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FavoritesPage] Load error: {ex.Message}");
                await DisplayAlert("Error", "Failed to load favorites. Please try again.", "OK");
            }
        }

        private async void OnItemTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int itemId)
            {
                await Shell.Current.GoToAsync($"DetailPage?itemId={itemId}");
            }
        }

        private async void OnExploreDishesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ExplorePage");
        }

        private async void OnUnfavoriteTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int itemId)
            {
                try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
                catch { }

                await _foodItemService.ToggleFavoriteAsync(itemId);
                await LoadFavoritesAsync();
            }
        }
    }
}
