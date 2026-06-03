using FoodAndDrink.Services;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace FoodAndDrink
{
    public partial class ScanPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private bool _hasScanned;
        private bool _isFlashlightOn;
        private bool _cameraStarted;

        public ScanPage(FoodItemService foodItemService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _hasScanned = false;

            await Task.Delay(500);
            StartCamera();

            _isFlashlightOn = false;
            BarcodeReader.IsTorchOn = false;
            FlashlightBtn.Text = "🔦 Flashlight";
            FlashlightBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            StopCamera();

            try { BarcodeReader.IsTorchOn = false; } catch { }
        }

        private void StartCamera()
        {
            if (_cameraStarted) return;
            try
            {
                BarcodeReader.IsDetecting = true;
                _cameraStarted = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScanPage] Camera start error: {ex.Message}");
            }
        }

        private void StopCamera()
        {
            try
            {
                BarcodeReader.IsDetecting = false;
                _cameraStarted = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScanPage] Camera stop error: {ex.Message}");
            }
        }

        private void OnFlashlightClicked(object sender, EventArgs e)
        {
            try
            {
                if (_isFlashlightOn)
                {
                    BarcodeReader.IsTorchOn = false;
                    _isFlashlightOn = false;
                    FlashlightBtn.Text = "🔦 Flashlight";
                    FlashlightBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
                }
                else
                {
                    BarcodeReader.IsTorchOn = true;
                    _isFlashlightOn = true;
                    FlashlightBtn.Text = "💡 Flashlight On";
                    FlashlightBtn.BackgroundColor = Color.FromArgb("#E8A317");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScanPage] Flashlight error: {ex.Message}");
                DisplayAlert("Not Available", "Flashlight is not supported on this device.", "OK");
            }
        }

        private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (_hasScanned) return;
            _hasScanned = true;

            try
            {
                try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); }
                catch { }

                var detected = e.Results?.FirstOrDefault();
                if (detected == null)
                {
                    _hasScanned = false;
                    return;
                }

                BarcodeReader.IsDetecting = false;

                var barcodeValue = detected.Value?.Trim();
                if (string.IsNullOrEmpty(barcodeValue))
                {
                    await DisplayAlert("Scan Result", "Empty barcode. Please try again.", "OK");
                    _hasScanned = false;
                    BarcodeReader.IsDetecting = true;
                    return;
                }

                var allItems = await _foodItemService.GetAllAsync();
                var matched = allItems.FirstOrDefault(i =>
                    i.Name.Contains(barcodeValue, StringComparison.OrdinalIgnoreCase) ||
                    i.Id.ToString() == barcodeValue);

                if (matched != null)
                {
                    await Shell.Current.GoToAsync($"DetailPage?itemId={matched.Id}");
                }
                else
                {
                    bool browse = await DisplayAlert(
                        "Barcode Scanned",
                        $"Value: {barcodeValue}\nFormat: {detected.Format}\n\nNo matching item found. Browse all?",
                        "Browse", "Close");

                    if (browse)
                        await Shell.Current.GoToAsync("ItemListPage");
                    else
                        await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScanPage] Scan error: {ex.Message}");
                await DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
