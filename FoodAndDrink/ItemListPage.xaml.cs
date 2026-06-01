using CommunityToolkit.Maui.Core;
using FoodAndDrink.Models;
using FoodAndDrink.Services;

namespace FoodAndDrink
{
    [QueryProperty(nameof(CategoryIdParam), "categoryId")]
    [QueryProperty(nameof(SortByParam), "sortBy")]
    [QueryProperty(nameof(SearchParam), "search")]
    public partial class ItemListPage : ContentPage
    {
        private readonly FoodItemService _foodItemService;
        private readonly CategoryService _categoryService;
        private List<FoodItem> _allItems = new();
        private int? _selectedCategoryId;
        private string _sortBy = "rating";
        private string _priceFilter = "All";

        public string? CategoryIdParam
        {
            set
            {
                if (int.TryParse(value, out var id))
                    _selectedCategoryId = id;
            }
        }

        public string? SortByParam
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _sortBy = value;
                    // Sync the Picker UI with the actual sort order
                    SortPicker.SelectedIndex = value switch
                    {
                        "distance" => 2,
                        "newest" => 3,
                        "price" => 1,
                        _ => 0
                    };
                }
            }
        }

        public string? SearchParam
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var keyword = Uri.UnescapeDataString(value);
                    ListSearchBar.Text = keyword;
                }
            }
        }

        public ItemListPage(FoodItemService foodItemService, CategoryService categoryService)
        {
            InitializeComponent();
            _foodItemService = foodItemService;
            _categoryService = categoryService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCategories();
            await LoadItems();
        }

        private async Task LoadCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            CategoryChips.Children.Clear();

            // "All" chip
            var allChip = CreateChip("All", isSelected: _selectedCategoryId == null);
            var tapAll = new TapGestureRecognizer();
            tapAll.Tapped += async (s, e) =>
            {
                _selectedCategoryId = null;
                await LoadCategories();
                await LoadItems();
            };
            allChip.GestureRecognizers.Add(tapAll);
            CategoryChips.Children.Add(allChip);

            foreach (var cat in categories)
            {
                var chip = CreateChip(cat.DisplayName, isSelected: _selectedCategoryId == cat.Id);
                var tap = new TapGestureRecognizer();
                var catId = cat.Id;
                tap.Tapped += async (s, e) =>
                {
                    _selectedCategoryId = catId;
                    await LoadCategories();
                    await LoadItems();
                };
                chip.GestureRecognizers.Add(tap);
                CategoryChips.Children.Add(chip);
            }
        }

        private static Frame CreateChip(string text, bool isSelected = false)
        {
            return new Frame
            {
                BackgroundColor = isSelected
                    ? (Color)Application.Current!.Resources["Primary"]
                    : (Color)Application.Current!.Resources["BorderLight"],
                BorderColor = Colors.Transparent,
                CornerRadius = 20,
                Padding = new Thickness(18, 10),
                HasShadow = false,
                Content = new Label
                {
                    Text = text,
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = isSelected ? Colors.White : (Color)Application.Current!.Resources["TextLight"]
                }
            };
        }

        private async Task LoadItems()
        {
            var filter = new ItemFilter
            {
                CategoryId = _selectedCategoryId,
                SortBy = _sortBy,
                PriceTier = _priceFilter == "All" ? null : _priceFilter
            };

            if (!string.IsNullOrWhiteSpace(ListSearchBar.Text))
            {
                filter.Cuisine = null;
                _allItems = await _foodItemService.SearchAsync(ListSearchBar.Text.Trim());

                // Apply in-memory filters on search results
                if (filter.CategoryId.HasValue)
                    _allItems = _allItems.Where(i => i.CategoryId == filter.CategoryId.Value).ToList();
                if (!string.IsNullOrEmpty(filter.PriceTier))
                    _allItems = _allItems.Where(i => i.PriceTier == filter.PriceTier).ToList();
                if (filter.MinRating.HasValue)
                    _allItems = _allItems.Where(i => i.Rating >= filter.MinRating.Value).ToList();
                if (filter.MaxDistance.HasValue)
                    _allItems = _allItems.Where(i => i.Distance <= filter.MaxDistance.Value).ToList();

                _allItems = _sortBy switch
                {
                    "price" => _allItems.OrderBy(i => i.Price).ToList(),
                    "distance" => _allItems.OrderBy(i => i.Distance).ToList(),
                    "newest" => _allItems.OrderByDescending(i => i.CreatedAt).ToList(),
                    _ => _allItems.OrderByDescending(i => i.Rating).ToList(),
                };
            }
            else
            {
                _allItems = await _foodItemService.QueryAsync(filter);
            }

            ItemsCollection.ItemsSource = null;
            ItemsCollection.ItemsSource = _allItems;
            EmptyState.IsVisible = _allItems.Count == 0;
        }

        private async void OnSearchPressed(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ListSearchBar.Text))
            {
                await DisplayAlert("Search", "Please enter a search term.", "OK");
                return;
            }
            await LoadItems();
        }

        private async void OnSortChanged(object? sender, EventArgs e)
        {
            if (SortPicker.SelectedIndex == 1)
                _sortBy = "price";
            else if (SortPicker.SelectedIndex == 2)
                _sortBy = "distance";
            else if (SortPicker.SelectedIndex == 3)
                _sortBy = "newest";
            else
                _sortBy = "rating";
            await LoadItems();
        }

        private async void OnPriceFilterChanged(object? sender, EventArgs e)
        {
            _priceFilter = PricePicker.SelectedItem?.ToString() ?? "All";
            await LoadItems();
        }

        private async void OnItemTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is int itemId)
            {
                await Shell.Current.GoToAsync($"DetailPage?itemId={itemId}");
            }
        }

        private async void OnFavoriteTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is int itemId)
            {
                try
                {
                    HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
                catch { }

                await _foodItemService.ToggleFavoriteAsync(itemId);
                await LoadItems();
            }
        }
    }
}
