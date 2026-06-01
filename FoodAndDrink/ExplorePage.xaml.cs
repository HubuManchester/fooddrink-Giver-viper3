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
                popularItems = popularItems.Where(i => i.IsPopular).Take(3).ToList();
                var recentItems = await _foodItemService.GetRecentlyViewedAsync(2);

                BindableLayout.SetItemsSource(CategoryGrid, categories.Take(6).ToList());
                BindableLayout.SetItemsSource(PopularStack, popularItems);
                BindableLayout.SetItemsSource(RecentStack, recentItems);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExplorePage] Load error: {ex.Message}");
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

        private async void OnCameraScanClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
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

                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        await Shell.Current.GoToAsync(nameof(DetailPage));
                    }
                }
                else
                {
                    await DisplayAlert("Not Supported", "Camera capture is not supported on this device.", "OK");
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

        private async void OnBarcodeScanClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Scan Barcode", "Barcode scanner will be available in a future update.", "OK");
        }
    }
}
