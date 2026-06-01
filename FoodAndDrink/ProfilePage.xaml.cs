using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class ProfilePage : ContentPage
    {
        private readonly FoodItemService _foodItemService;

        public ProfilePage(FoodItemService foodItemService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStatsAsync();
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                StatFavorites.Text = (await _foodItemService.GetFavoriteCountAsync()).ToString();
                StatReviews.Text = (await _foodItemService.GetViewedCountAsync()).ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProfilePage] Stats error: {ex.Message}");
            }
        }

        private void OnDietaryToggleChanged(object sender, ToggledEventArgs e)
        {
            // Save dietary preference
        }

        private void OnNotificationToggleChanged(object sender, ToggledEventArgs e)
        {
            // Save notification preference
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }

        private void OnTextSizeChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue < 0.33)
                TextSizeLabel.Text = "Small";
            else if (e.NewValue < 0.66)
                TextSizeLabel.Text = "Medium";
            else
                TextSizeLabel.Text = "Large";
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Log Out", "Cancel");
            if (confirm)
            {
                // Perform logout
            }
        }
    }
}
