using CommunityToolkit.Maui.Core;
using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class ExplorePage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private readonly CategoryService _categoryService;

        public ExplorePage(FoodItemService foodItemService, CategoryService categoryService)
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
                var categories = await _categoryService.GetAllAsync();
                var popularItems = await _foodItemService.GetAllAsync("rating");
                popularItems = popularItems.Where(i => i.IsPopular).OrderByDescending(i => i.Rating).Take(3).ToList();
                var recentItems = await _foodItemService.GetRecentlyViewedAsync(2);

                BindableLayout.SetItemsSource(CategoryGrid, categories);
                BindableLayout.SetItemsSource(PopularStack, popularItems);
                BindableLayout.SetItemsSource(RecentStack, recentItems);

                // Show empty state if no data at all
                EmptyState.IsVisible = categories.Count == 0 && popularItems.Count == 0 && recentItems.Count == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExplorePage] Load error: {ex.Message}");
                await DisplayAlert("Error", "Failed to load content. Please try again.", "OK");
            }
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

        /// <summary>
        /// Opens the device camera to capture a photo of a dish.
        /// Uses MediaPicker API with permission handling.
        /// </summary>
        private async void OnCameraScanClicked(object sender, EventArgs e)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlert("Not Supported", "Camera capture is not supported on this device.", "OK");
                    return;
                }

                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permission Required", "Camera permission is needed for Smart Scan.", "OK");
                        return;
                    }
                }

                // Vibrate on capture
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
                    catch { /* Haptic not supported */ }

                    await DisplayAlert("Photo Captured",
                        "Photo saved! In a production app, this would use AI image recognition to identify the dish and show its details.",
                        "OK");
                }
            }
            catch (PermissionException)
            {
                await DisplayAlert("Permission Required", "Camera permission is needed for Smart Scan.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExplorePage] Camera error: {ex.Message}");
                await DisplayAlert("Error", "Could not open camera. Please try again.", "OK");
            }
        }

        /// <summary>
        /// Navigates to the barcode scanner page which uses ZXing
        /// to read barcodes/QR codes via the device camera.
        /// </summary>
        private async void OnBarcodeScanClicked(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permission Required",
                            "Camera permission is needed for barcode scanning.", "OK");
                        return;
                    }
                }

                await Shell.Current.GoToAsync("ScanPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExplorePage] Barcode error: {ex.Message}");
                await DisplayAlert("Error", "Could not open barcode scanner. Please try again.", "OK");
            }
        }

        /// <summary>
        /// Navigates to the full item list sorted by rating when "See All" is tapped
        /// in the Popular This Week section.
        /// </summary>
        private async void OnPopularSeeAllClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ItemListPage?sortBy=rating");
        }

        /// <summary>
        /// Clears all recently viewed items from the tracked list.
        /// </summary>
        private async void OnClearRecentClicked(object sender, EventArgs e)
        {
            try
            {
                await _foodItemService.ClearRecentlyViewedAsync();
                var empty = new List<Models.FoodItem>();
                BindableLayout.SetItemsSource(RecentStack, empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExplorePage] Clear recent error: {ex.Message}");
                await DisplayAlert("Error", "Failed to clear recently viewed items.", "OK");
            }
        }

        private async void OnExploreSearchPressed(object sender, EventArgs e)
        {
            var keyword = ExploreSearchBar.Text?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                await DisplayAlert("Search", "Please enter a search term.", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"ItemListPage?search={Uri.EscapeDataString(keyword)}");
        }
    }
}
