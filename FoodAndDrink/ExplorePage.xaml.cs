namespace FoodAndDrink
{
    public partial class ExplorePage : ContentPage
    {
        public ExplorePage()
        {
            InitializeComponent();
        }

        private async void OnCameraScanClicked(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // Navigate to detail with captured image
                    await Shell.Current.GoToAsync(nameof(DetailPage));
                }
            }
        }

        private async void OnBarcodeScanClicked(object sender, EventArgs e)
        {
            // Navigate to barcode scanner or use ZXing scanner
            await DisplayAlert("Scan Barcode", "Barcode scanner will open here.", "OK");
        }
    }
}
