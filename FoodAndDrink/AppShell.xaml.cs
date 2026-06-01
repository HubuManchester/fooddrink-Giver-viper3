namespace FoodAndDrink
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("DetailPage", typeof(DetailPage));
            Routing.RegisterRoute("ItemListPage", typeof(ItemListPage));
            Routing.RegisterRoute("ScanPage", typeof(ScanPage));
        }
    }
}
