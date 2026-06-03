using FoodAndDrink.Services;

namespace FoodAndDrink
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            FontScaleService.InitResources();

            MainPage = new AppShell();
        }
    }
}
