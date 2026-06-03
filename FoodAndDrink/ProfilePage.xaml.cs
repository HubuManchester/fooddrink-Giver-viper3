using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class ProfilePage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private readonly FontScaleService _fontScale;

        public ProfilePage(FoodItemService foodItemService, FontScaleService fontScale)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
            _fontScale = fontScale;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStatsAsync();

            TextSizeSlider.Value = _fontScale.GetSliderValue();
            TextSizeLabel.Text = _fontScale.GetScaleLabel();

            DarkModeSwitch.IsToggled = Application.Current?.UserAppTheme == AppTheme.Dark;
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                StatFavorites.Text = (await _foodItemService.GetFavoriteCountAsync()).ToString();
                StatScans.Text = "—";
                StatReviews.Text = (await _foodItemService.GetViewedCountAsync()).ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProfilePage] Stats error: {ex.Message}");
            }
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }

        private void OnTextSizeChanged(object sender, ValueChangedEventArgs e)
        {
            _fontScale.SetFromSlider(e.NewValue);
            TextSizeLabel.Text = _fontScale.GetScaleLabel();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Log Out", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}
