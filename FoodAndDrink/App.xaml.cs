using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Initialize font scale resources from saved preference
            FontScaleService.InitResources();

            MainPage = new AppShell();
        }
    }
}
