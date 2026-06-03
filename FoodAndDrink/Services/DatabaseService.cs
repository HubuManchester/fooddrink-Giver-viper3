using FoodAndDrink.Models;
using SQLite;

namespace FoodAndDrink.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _db;
        private readonly string _dbPath;
        private static readonly SemaphoreSlim _initLock = new(1, 1);

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "foodanddrink.db");
        }

        private async Task<SQLiteAsyncConnection> GetDbAsync()
        {
            if (_db is not null)
                return _db;

            await _initLock.WaitAsync();
            try
            {
                if (_db is not null)
                    return _db;

                _db = new SQLiteAsyncConnection(_dbPath);
                await _db.CreateTableAsync<Category>();
                await _db.CreateTableAsync<FoodItem>();

                var count = await _db.Table<Category>().CountAsync();
                if (count == 0)
                {
                    await SeedDataAsync(_db);
                }

                return _db;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            return await GetDbAsync();
        }

        private static async Task SeedDataAsync(SQLiteAsyncConnection db)
        {
            var categories = new List<Category>
            {
                new() { Id = 1, Name = "Noodles",  DisplayName = "🍜 Noodles",  ColorToken = "Primary",   SortOrder = 1, IconEmoji = "🍜" },
                new() { Id = 2, Name = "Drinks",   DisplayName = "🧋 Drinks",   ColorToken = "Secondary", SortOrder = 2, IconEmoji = "🧋" },
                new() { Id = 3, Name = "Sushi",    DisplayName = "🍣 Sushi",    ColorToken = "Tertiary",  SortOrder = 3, IconEmoji = "🍣" },
                new() { Id = 4, Name = "Pizza",    DisplayName = "🍕 Pizza",    ColorToken = "Primary",   SortOrder = 4, IconEmoji = "🍕" },
                new() { Id = 5, Name = "Desserts", DisplayName = "🍰 Desserts", ColorToken = "Secondary", SortOrder = 5, IconEmoji = "🍰" },
                new() { Id = 6, Name = "Coffee",   DisplayName = "☕ Coffee",   ColorToken = "Tertiary",  SortOrder = 6, IconEmoji = "☕" },
                new() { Id = 7, Name = "Chinese",  DisplayName = "🥡 Chinese",  ColorToken = "Primary",   SortOrder = 7, IconEmoji = "🥡" },
            };

            var now = DateTime.Now;
            var items = new List<FoodItem>
            {
                new() { Name = "Lanzhou Beef Noodles",  Subtitle = "Gansu", ImageUrl = "lanzhou_beef_noodles.png", Description = "Hand-pulled wheat noodles in a clear beef broth, topped with sliced beef, radish, chili oil, and fresh coriander. A Northwest Chinese classic.", Price = 18.00m, PriceTier = "$", Rating = 4.8, ReviewCount = 512, CategoryId = 1, Cuisine = "Chinese", Distance = 0.4, Calories = 520, Allergens = "Wheat", AvailableTime = "6:00 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Zhajiang Noodles",      Subtitle = "Beijing", ImageUrl = "zhajiang_noodles.png", Description = "Thick wheat noodles topped with a rich, savory soybean paste sauce stir-fried with minced pork, julienned cucumber and bean sprouts.", Price = 18.00m, PriceTier = "$", Rating = 4.6, ReviewCount = 389, CategoryId = 1, Cuisine = "Chinese", Distance = 0.6, Calories = 480, Allergens = "Wheat,Soy", AvailableTime = "10:00 AM - 9:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Chongqing Xiaomian",     Subtitle = "Chongqing", ImageUrl = "chongqing_xiaomian.png", Description = "Spicy Sichuan-style wheat noodles in a fiery chili and Sichuan peppercorn broth, topped with minced pork, pickled vegetables, and scallions.", Price = 15.00m, PriceTier = "$", Rating = 4.7, ReviewCount = 298, CategoryId = 1, Cuisine = "Chinese", Distance = 0.8, Calories = 450, Allergens = "Wheat", AvailableTime = "7:00 AM - 9:00 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Biang Biang Noodles",    Subtitle = "Shaanxi", ImageUrl = "biang_biang_noodles.png", Description = "Legendary wide hand-pulled belt noodles from Xi'an, drenched in sizzling chili oil, garlic, and black vinegar. Known as the 'belt noodles' for their width.", Price = 22.00m, PriceTier = "$$", Rating = 4.7, ReviewCount = 432, CategoryId = 1, Cuisine = "Chinese", Distance = 0.5, Calories = 490, Allergens = "Wheat", AvailableTime = "8:00 AM - 9:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Dandan Noodles",         Subtitle = "Sichuan", ImageUrl = "dandan_noodles.png", Description = "Classic Sichuan street food — springy noodles in a fiery sesame-peanut sauce with minced pork, pickled mustard greens, and roasted peanuts.", Price = 16.00m, PriceTier = "$", Rating = 4.6, ReviewCount = 367, CategoryId = 1, Cuisine = "Chinese", Distance = 0.7, Calories = 410, Allergens = "Wheat,Peanut,Sesame", AvailableTime = "8:00 AM - 9:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },

                new() { Name = "Brown Sugar Milk Tea",   Subtitle = "Guangdong", ImageUrl = "brown_sugar_milk_tea.png", Description = "Freshly brewed black tea swirled with hand-poured brown sugar syrup, chewy tapioca pearls, and a creamy milk cap. The classic bubble tea.", Price = 18.00m, PriceTier = "$", Rating = 4.9, ReviewCount = 467, CategoryId = 2, Cuisine = "Chinese", Distance = 0.2, Calories = 380, Allergens = "Milk", AvailableTime = "10:00 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Matcha Latte",           Subtitle = "Zhejiang", ImageUrl = "matcha_latte.png", Description = "Stone-ground ceremonial matcha from Hangzhou, whisked with steamed oat milk and touched with osmanthus honey.", Price = 28.00m, PriceTier = "$$", Rating = 4.6, ReviewCount = 312, CategoryId = 2, Cuisine = "Chinese", Distance = 0.5, Calories = 220, Allergens = "Milk", AvailableTime = "8:00 AM - 8:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Mango Pomelo Sago",      Subtitle = "Hong Kong", ImageUrl = "mango_pomelo_sago.png", Description = "A refreshing Cantonese dessert drink with ripe mango, pomelo sacs, coconut milk, and tiny sago pearls over crushed ice.", Price = 28.00m, PriceTier = "$$", Rating = 4.8, ReviewCount = 356, CategoryId = 2, Cuisine = "Chinese", Distance = 0.7, Calories = 280, Allergens = "Milk", AvailableTime = "11:00 AM - 9:30 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Osmanthus Oolong Tea",   Subtitle = "Fujian", ImageUrl = "osmanthus_oolong_tea.png", Description = "Fragrant Tieguanyin oolong from Anxi blended with golden osmanthus blossoms, served in a traditional gaiwan. Floral, smooth, and endlessly aromatic.", Price = 35.00m, PriceTier = "$$", Rating = 4.8, ReviewCount = 289, CategoryId = 2, Cuisine = "Chinese", Distance = 0.4, Calories = 10, Allergens = "", AvailableTime = "9:00 AM - 9:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Kumquat Lemon Tea",      Subtitle = "Guangdong", ImageUrl = "kumquat_lemon_tea.png", Description = "Bright and zesty iced tea made with honey-pickled kumquats, fresh lemon slices, and jasmine green tea. A southern Chinese summer staple.", Price = 15.00m, PriceTier = "$", Rating = 4.5, ReviewCount = 223, CategoryId = 2, Cuisine = "Chinese", Distance = 0.3, Calories = 90, Allergens = "", AvailableTime = "10:00 AM - 10:00 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },

                new() { Name = "Omakase Sushi Platter",  Subtitle = "Shanghai", ImageUrl = "omakase_sushi_platter.png", Description = "Chef's seasonal selection of 12 pieces of premium nigiri, featuring fresh seafood air-flown daily. Served with house-made soy and wasabi.", Price = 388.00m, PriceTier = "$$$", Rating = 4.9, ReviewCount = 478, CategoryId = 3, Cuisine = "Japanese", Distance = 1.5, Calories = 480, Allergens = "Fish,Soy", AvailableTime = "11:30 AM - 9:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Dragon Roll",            Subtitle = "Guangzhou", ImageUrl = "dragon_roll.png", Description = "Shrimp tempura and cucumber wrapped inside, layered with sliced avocado and drizzled with house-made spicy unagi sauce. 8 pieces.", Price = 68.00m, PriceTier = "$$", Rating = 4.7, ReviewCount = 334, CategoryId = 3, Cuisine = "Japanese", Distance = 0.5, Calories = 520, Allergens = "Shellfish,Wheat,Soy", AvailableTime = "11:00 AM - 10:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Salmon Poke Bowl",       Subtitle = "Shenzhen", ImageUrl = "salmon_poke_bowl.png", Description = "Fresh Norwegian salmon cubes over seasoned sushi rice with avocado, edamame, cucumber, seaweed salad, and sesame dressing.", Price = 52.00m, PriceTier = "$$", Rating = 4.5, ReviewCount = 256, CategoryId = 3, Cuisine = "Japanese", Distance = 0.8, Calories = 580, Allergens = "Fish,Soy,Sesame", AvailableTime = "10:30 AM - 9:00 PM", IsTrending = false, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Spicy Tuna Roll",        Subtitle = "Tokyo", ImageUrl = "spicy_tuna_roll.png", Description = "Fresh bluefin tuna mixed with spicy mayo and scallions, rolled in seasoned sushi rice and nori, topped with a drizzle of house-made chili oil. 8 pieces.", Price = 58.00m, PriceTier = "$$", Rating = 4.6, ReviewCount = 312, CategoryId = 3, Cuisine = "Japanese", Distance = 0.9, Calories = 450, Allergens = "Fish,Soy,Wheat", AvailableTime = "11:00 AM - 10:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },

                new() { Name = "Margherita DOP",         Subtitle = "Shanghai", ImageUrl = "margherita.png", Description = "San Marzano DOP tomato sauce, fresh buffalo mozzarella, garden basil, and extra virgin olive oil on a Neapolitan wood-fired crust.", Price = 78.00m, PriceTier = "$$", Rating = 4.7, ReviewCount = 378, CategoryId = 4, Cuisine = "Italian", Distance = 0.4, Calories = 780, Allergens = "Wheat,Milk", AvailableTime = "11:00 AM - 10:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Sichuan Spicy Pizza",     Subtitle = "Chengdu", ImageUrl = "sichuan_spicy_pizza.png", Description = "A bold fusion pizza topped with spicy Sichuan peppercorn sausage, roasted bell peppers, red onions, and mozzarella on a thin crispy base.", Price = 68.00m, PriceTier = "$$", Rating = 4.5, ReviewCount = 201, CategoryId = 4, Cuisine = "Fusion", Distance = 0.9, Calories = 850, Allergens = "Wheat,Milk", AvailableTime = "11:30 AM - 10:00 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Truffle Mushroom Pizza",  Subtitle = "Beijing", ImageUrl = "truffle_mushroom_pizza.png", Description = "Wild mushroom medley with black truffle cream sauce, fontina cheese, and fresh thyme on an artisan sourdough base. Earthy and luxurious.", Price = 128.00m, PriceTier = "$$$", Rating = 4.8, ReviewCount = 245, CategoryId = 4, Cuisine = "Italian", Distance = 1.2, Calories = 820, Allergens = "Wheat,Milk", AvailableTime = "5:00 PM - 10:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Peking Duck Pizza",       Subtitle = "Beijing", ImageUrl = "peking_duck_pizza.png", Description = "East-meets-West fusion — thin crispy pizza base layered with hoisin sauce, shredded Peking duck, julienned cucumber, scallions, and mozzarella.", Price = 98.00m, PriceTier = "$$$", Rating = 4.7, ReviewCount = 278, CategoryId = 4, Cuisine = "Fusion", Distance = 1.0, Calories = 880, Allergens = "Wheat,Milk,Soy", AvailableTime = "11:30 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },

                new() { Name = "Osmanthus Cake",          Subtitle = "Guilin", ImageUrl = "osmanthus_cake.png", Description = "Delicate steamed sponge cake infused with sweet osmanthus flower syrup and osmanthus honey. Light, floral, and beloved across Guangxi.", Price = 15.00m, PriceTier = "$", Rating = 4.7, ReviewCount = 312, CategoryId = 5, Cuisine = "Chinese", Distance = 0.5, Calories = 220, Allergens = "Wheat,Egg", AvailableTime = "9:00 AM - 8:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Candied Hawthorn Skewers",Subtitle = "Tianjin", ImageUrl = "candied_hawthorn_skewers.png", Description = "Crispy sugar-coated hawthorn berries threaded on bamboo skewers — a beloved northern Chinese street sweet balancing tart fruit with a glassy candy shell.", Price = 8.00m, PriceTier = "$", Rating = 4.8, ReviewCount = 398, CategoryId = 5, Cuisine = "Chinese", Distance = 0.3, Calories = 150, Allergens = "", AvailableTime = "10:00 AM - 9:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Dragon's Beard Candy",    Subtitle = "Beijing", ImageUrl = "dragons_beard_candy.png", Description = "Traditional imperial hand-pulled sugar candy — thousands of fine spun-sugar threads wrapped around a crushed peanut and sesame filling. A vanishing art.", Price = 18.00m, PriceTier = "$", Rating = 4.9, ReviewCount = 267, CategoryId = 5, Cuisine = "Chinese", Distance = 0.6, Calories = 180, Allergens = "Peanut,Sesame", AvailableTime = "10:00 AM - 8:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Deep-Fried Sesame Balls",  Subtitle = "Guangdong", ImageUrl = "deep_fried_sesame_balls.png", Description = "Crispy golden glutinous rice balls coated in sesame seeds, filled with sweet red bean paste. A classic Cantonese dim sum dessert with a satisfying crunch.", Price = 12.00m, PriceTier = "$", Rating = 4.6, ReviewCount = 334, CategoryId = 5, Cuisine = "Chinese", Distance = 0.4, Calories = 260, Allergens = "Wheat,Sesame", AvailableTime = "8:00 AM - 8:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },


                new() { Name = "Yunnan Pour Over",        Subtitle = "Yunnan", ImageUrl = "yunnan_pour_over.png", Description = "Single-origin Arabica from Pu'er, Yunnan — hand-poured V60. Notes of dark chocolate, roasted nuts, and a hint of stone fruit.", Price = 35.00m, PriceTier = "$$", Rating = 4.5, ReviewCount = 234, CategoryId = 6, Cuisine = "Chinese", Distance = 0.3, Calories = 5, Allergens = "", AvailableTime = "7:00 AM - 7:00 PM", IsTrending = false, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Coconut Cold Brew",       Subtitle = "Hainan", ImageUrl = "coconut_cold_brew.png", Description = "Smooth cold-brewed coffee blended with fresh Hainan coconut water, served over ice. A refreshing tropical pick-me-up.", Price = 28.00m, PriceTier = "$$", Rating = 4.7, ReviewCount = 345, CategoryId = 6, Cuisine = "Chinese", Distance = 0.5, Calories = 80, Allergens = "", AvailableTime = "8:00 AM - 8:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Spanish Latte",           Subtitle = "Shanghai", ImageUrl = "spanish_latte.png", Description = "Rich double espresso with condensed milk and steamed whole milk — a sweet, creamy, indulgent latte crafted in Shanghai's coffee scene.", Price = 30.00m, PriceTier = "$$", Rating = 4.6, ReviewCount = 298, CategoryId = 6, Cuisine = "Fusion", Distance = 0.4, Calories = 190, Allergens = "Milk", AvailableTime = "7:30 AM - 8:30 PM", IsTrending = false, IsPopular = true, IsRecommended = false, CreatedAt = now },
                new() { Name = "Osmanthus Latte",         Subtitle = "Hangzhou", ImageUrl = "osmanthus_latte.png", Description = "Velvety espresso blended with house-made osmanthus flower syrup and steamed milk, finished with a sprinkle of dried osmanthus petals. Floral and comforting.", Price = 32.00m, PriceTier = "$$", Rating = 4.7, ReviewCount = 256, CategoryId = 6, Cuisine = "Chinese", Distance = 0.5, Calories = 180, Allergens = "Milk", AvailableTime = "7:30 AM - 8:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },

                new() { Name = "Mapo Tofu",              Subtitle = "Sichuan", ImageUrl = "mapo_tofu.png", Description = "Silky soft tofu cubes in a fiery, numbing sauce of minced beef, fermented broad bean paste, Sichuan peppercorns, and chili oil. A Chengdu icon.", Price = 28.00m, PriceTier = "$$", Rating = 4.8, ReviewCount = 456, CategoryId = 7, Cuisine = "Chinese", Distance = 0.6, Calories = 420, Allergens = "Soy", AvailableTime = "10:00 AM - 9:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Dongpo Braised Pork",     Subtitle = "Zhejiang", ImageUrl = "dongpo_braised_pork.png", Description = "Melt-in-your-mouth pork belly slow-braised in soy sauce, Shaoxing wine, and rock sugar until caramelized and tender. A Hangzhou masterpiece.", Price = 68.00m, PriceTier = "$$$", Rating = 4.9, ReviewCount = 398, CategoryId = 7, Cuisine = "Chinese", Distance = 1.0, Calories = 650, Allergens = "Soy", AvailableTime = "11:00 AM - 9:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Crispy Roast Duck",       Subtitle = "Beijing", ImageUrl = "crispy_roast_duck.png", Description = "Legendary Beijing roast duck with crackling golden skin, carved tableside, served with thin pancakes, cucumber, scallion, and sweet bean sauce.", Price = 198.00m, PriceTier = "$$$", Rating = 4.9, ReviewCount = 567, CategoryId = 7, Cuisine = "Chinese", Distance = 1.2, Calories = 720, Allergens = "Wheat,Soy", AvailableTime = "11:00 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Kung Pao Chicken",        Subtitle = "Sichuan", ImageUrl = "kung_pao_chicken.png", Description = "Tender diced chicken wok-fried with golden peanuts, dried chilies, and Sichuan peppercorns in a savory-sweet soy-vinegar sauce. A global Chinese food icon.", Price = 42.00m, PriceTier = "$$", Rating = 4.8, ReviewCount = 445, CategoryId = 7, Cuisine = "Chinese", Distance = 0.8, Calories = 480, Allergens = "Peanut,Soy", AvailableTime = "10:30 AM - 9:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
            };

            for (int i = 0; i < items.Count; i++)
                items[i].CreatedAt = now.AddMinutes(-300 + i * 10);

            await db.InsertAllAsync(categories);
            await db.InsertAllAsync(items);
        }
    }
}
