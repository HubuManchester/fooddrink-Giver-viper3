# Food & Drink

A food and drink discovery app for Android. Browse dishes, scan barcodes, save favorites, get directions. Built with .NET MAUI, targeting phones and tablets.

## App overview

The app has four tabs:

**Home** shows trending items, nearby picks, categories, and recommendations. Shaking the phone triggers a random food suggestion with haptic feedback.

**Explore** has a search bar, a Smart Scan section (camera capture + barcode scanner), a category grid, popular items, and a recently viewed list.

**Favorites** displays saved items. Tapping the heart removes one. If the list is empty, there's a button to jump to Explore.

**Profile** shows stats (favorites count, recently viewed count), a dark mode toggle, a text size slider (0.8x to 1.4x), and a logout button.

Tapping any item opens its detail page — rating, price, distance, calories, allergens, available hours. From there you can read the description aloud (TTS), share it, open it in a delivery app (Meituan deep link with browser fallback), or get directions (native maps with Google Maps fallback).

The item list page supports search, category filtering, price tier filtering ($/$$/$$$), and sorting by rating, price, distance, or newest.

## Mobile hardware

The app uses seven device features:

1. **Camera** — MediaPicker for photo capture on the Explore page, with permission handling
2. **Barcode scanning** — ZXing camera reader with torch toggle on the Scan page
3. **GPS/Geolocation** — retrieves the device location and calculates distance to items using the Haversine formula
4. **Accelerometer** — detects shake gestures on the Home page to trigger random recommendations
5. **Haptic feedback** — vibration on favorite toggles, shake detection, barcode scans, and order/direction actions
6. **Text-to-Speech** — reads item descriptions and allergen info aloud on the Detail page
7. **Flashlight** — torch on/off during barcode scanning via ZXing's built-in torch API

## Tech stack

- .NET 8 MAUI (Android only, `net8.0-android`)
- SQLite via `sqlite-net-pcl` (v1.9.172) + `SQLitePCLRaw.bundle_green`
- CommunityToolkit.Maui (v8.0.0)
- ZXing.Net.Maui.Controls (v0.4.0)

## Data and algorithms

SQLite database at `{AppDataDirectory}/foodanddrink.db`. Auto-seeds 7 categories and 32 food items on first launch (Chinese regional cuisine across noodles, drinks, sushi, pizza, desserts, coffee, and Chinese dishes). Data persists across restarts.

Algorithms used:
- **Haversine formula** — calculates great-circle distance between GPS coordinates in miles
- **Shake detection** — computes accelerometer magnitude (`sqrt(x² + y² + z²)`) against a threshold (1.2g), with a 2-second cooldown to prevent repeated triggers
- **Dynamic font scaling** — 7 base font sizes multiplied by a user-controlled scale factor (0.8x–1.4x), persisted via MAUI Preferences

## Validation and error handling

Every service method has try-catch with graceful degradation — returns empty collections or null instead of crashing. The UI shows empty states (e.g., "No favorites yet" with a button to Explore) rather than error screens.

Input validation: search bars reject empty input with haptic feedback. Permission checks run before camera and location access. The barcode scanner has a `_hasScanned` flag to prevent duplicate scans.

The database uses double-check locking with `SemaphoreSlim` to prevent race conditions during initialization.

## Responsive layout

Uses `OnIdiom` to adjust layouts for phone vs. tablet:
- Category grid: 2 columns on phone, 4 on tablet
- Item list: 1 column on phone, 2 on tablet
- Horizontal scroll sections on Home and Explore for smaller screens

## Accessibility

- Dark mode via `AppThemeBinding` with full light/dark color variants
- Text size slider on Profile page (0.8x–1.4x) that updates all font sizes globally
- `AutomationProperties.Name` and `HelpText` on interactive elements
- Minimum 44dp touch targets
- Color-coded allergen display (green for none, warning color for allergens)

## Building

```bash
dotnet workload install maui
dotnet build -f net8.0-android
```

Run on Android emulator (Pixel 6 Pro, API 34) or a physical device.

## Project structure

```
FoodAndDrink/
├── Models/              FoodItem.cs, Category.cs, ItemFilter.cs
├── Services/            DatabaseService.cs, FoodItemService.cs,
│                        CategoryService.cs, LocationService.cs,
│                        FontScaleService.cs
├── MainPage.xaml/.cs    Home — trending, nearby, categories, recommended, shake
├── ExplorePage.xaml/.cs Camera, barcode, categories, popular, recent
├── FavoritesPage.xaml   Saved items with empty state
├── ProfilePage.xaml     Dark mode, text size, stats, logout
├── DetailPage.xaml      Item detail with TTS, share, order, directions
├── ItemListPage.xaml    Search, filters, sort
├── ScanPage.xaml        ZXing barcode scanner with flashlight
├── AppShell.xaml        4-tab TabBar navigation
├── MauiProgram.cs       DI container setup
├── Resources/
│   ├── Styles/          Colors.xaml (Warm Kitchen palette), Styles.xaml
│   ├── Fonts/           OpenSans
│   └── Images/          App assets
└── Platforms/Android/
    └── AndroidManifest.xml   Permissions declaration
```

