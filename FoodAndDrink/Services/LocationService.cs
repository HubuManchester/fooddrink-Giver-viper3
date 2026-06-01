namespace FoodAndDrink.Services
{
    public class LocationService
    {
        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                        return null;
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                return await Geolocation.Default.GetLocationAsync(request);
            }
            catch (FeatureNotSupportedException)
            {
                System.Diagnostics.Debug.WriteLine("[LocationService] GPS not supported on this device.");
                return null;
            }
            catch (PermissionException)
            {
                System.Diagnostics.Debug.WriteLine("[LocationService] Location permission denied.");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LocationService] Unexpected: {ex.Message}");
                return null;
            }
        }

        // Haversine formula for distance between two coordinates (in miles)
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double radLat1 = lat1 * (Math.PI / 180.0);
            double radLat2 = lat2 * (Math.PI / 180.0);
            double deltaLat = (lat2 - lat1) * (Math.PI / 180.0);
            double deltaLon = (lon2 - lon1) * (Math.PI / 180.0);

            double a = Math.Sin(deltaLat / 2.0) * Math.Sin(deltaLat / 2.0) +
                       Math.Cos(radLat1) * Math.Cos(radLat2) *
                       Math.Sin(deltaLon / 2.0) * Math.Sin(deltaLon / 2.0);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            const double earthRadiusMiles = 3958.8;
            return earthRadiusMiles * c;
        }
    }
}
