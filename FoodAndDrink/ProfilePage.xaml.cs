namespace FoodAndDrink
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
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
            // Adjust text size scaling
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
