using CommunityToolkit.Maui;
using FoodAndDrink.Services;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace FoodAndDrink
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<FoodItemService>();
            builder.Services.AddSingleton<CategoryService>();
            builder.Services.AddSingleton<LocationService>();
            builder.Services.AddSingleton<FontScaleService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ExplorePage>();
            builder.Services.AddTransient<FavoritesPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<DetailPage>();
            builder.Services.AddTransient<ItemListPage>();
            builder.Services.AddTransient<ScanPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

