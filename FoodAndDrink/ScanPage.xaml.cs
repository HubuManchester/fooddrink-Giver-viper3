using CommunityToolkit.Maui.Core;
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

        public ScanPage(FoodItemService foodItemService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _hasScanned = false;
            BarcodeReader.IsDetecting = true;

            // Ensure flashlight starts off
            _isFlashlightOn = false;
            try { await Flashlight.Default.TurnOffAsync(); } catch { }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            BarcodeReader.IsDetecting = false;

            // Turn off flashlight when leaving
            try { await Flashlight.Default.TurnOffAsync(); } catch { }
        }

        /// <summary>
        /// Toggles the device flashlight on/off for barcode scanning in low light.
        /// Uses the Flashlight API — counts as mobile hardware usage.
        /// </summary>
        private async void OnFlashlightClicked(object sender, EventArgs e)
        {
            try
            {
                if (_isFlashlightOn)
                {
                    await Flashlight.Default.TurnOffAsync();
                    _isFlashlightOn = false;
                    FlashlightBtn.Text = "🔦";
                    FlashlightBtn.BackgroundColor = (Color)Application.Current!.Resources["Gray100"];
                }
                else
                {
                    await Flashlight.Default.TurnOnAsync();
                    _isFlashlightOn = true;
                    FlashlightBtn.Text = "💡";
                    FlashlightBtn.BackgroundColor = (Color)Application.Current!.Resources["Tertiary"];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScanPage] Flashlight error: {ex.Message}");
                await DisplayAlert("Not Available", "Flashlight is not supported on this device.", "OK");
            }
        }

        /// <summary>
        /// Handles barcode detection events from ZXing camera view.
        /// On first detection, vibrates, stops scanning, and shows the scanned value.
        /// </summary>
        private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (_hasScanned) return;
            _hasScanned = true;

            try
            {
                // Vibrate on successful scan
                try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); }
                catch { /* Haptic not supported */ }

                BarcodeReader.IsDetecting = false;

                var result = e.Results?.FirstOrDefault();
                if (result == null)
                {
                    await DisplayAlert("Scan Failed", "No barcode detected. Please try again.", "OK");
                    _hasScanned = false;
                    BarcodeReader.IsDetecting = true;
                    return;
                }

                // For the assignment demo: show the barcode value and try to find a matching item
                var barcodeValue = result.Value?.Trim();

                // Try to match barcode to an item (by name or ID embedded in QR)
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
                    // Show scan result and offer to browse
                    bool browse = await DisplayAlert(
                        "Barcode Scanned",
                        $"Value: {barcodeValue}\nFormat: {result.Format}\n\nNo matching item found. Browse all items?",
                        "Browse", "Close");

                    if (browse)
                    {
                        await Shell.Current.GoToAsync("ItemListPage");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScanPage] Error: {ex.Message}");
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
