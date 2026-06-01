using FoodAndDrink.Models;
using SQLite;

namespace FoodAndDrink.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _db;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "foodanddrink.db");
        }

        private async Task<SQLiteAsyncConnection> GetDbAsync()
        {
            if (_db is not null)
                return _db;

            _db = new SQLiteAsyncConnection(_dbPath);
            await _db.CreateTableAsync<Category>();
            await _db.CreateTableAsync<FoodItem>();

            // Seed if empty
            var count = await _db.Table<Category>().CountAsync();
            if (count == 0)
            {
                await SeedDataAsync(_db);
            }

            return _db;
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
                new() { Id = 5, Name = "Healthy",  DisplayName = "🥗 Healthy",  ColorToken = "Secondary", SortOrder = 5, IconEmoji = "🥗" },
                new() { Id = 6, Name = "Desserts", DisplayName = "🍰 Desserts", ColorToken = "Tertiary",  SortOrder = 6, IconEmoji = "🍰" },
                new() { Id = 7, Name = "Coffee",   DisplayName = "☕ Coffee",   ColorToken = "Primary",   SortOrder = 7, IconEmoji = "☕" },
                new() { Id = 8, Name = "Chinese",  DisplayName = "🥡 Chinese",  ColorToken = "Secondary", SortOrder = 8, IconEmoji = "🥡" },
            };

            var now = DateTime.Now;
            var items = new List<FoodItem>
            {
                // Noodles (3) — 中国各地面食
                new() { Name = "Lanzhou Beef Noodles",  Subtitle = "Gansu", Description = "Hand-pulled wheat noodles in a clear beef broth, topped with sliced beef, radish, chili oil, and fresh coriander. A Northwest Chinese classic.", Price = 10.99m, PriceTier = "$", Rating = 4.8, ReviewCount = 512, CategoryId = 1, Cuisine = "Chinese", Distance = 0.4, Calories = 520, Allergens = "Wheat", AvailableTime = "6:00 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Zhajiang Noodles",      Subtitle = "Beijing", Description = "Thick wheat noodles topped with a rich, savory soybean paste sauce stir-fried with minced pork, julienned cucumber and bean sprouts.", Price = 9.99m, PriceTier = "$", Rating = 4.6, ReviewCount = 389, CategoryId = 1, Cuisine = "Chinese", Distance = 0.6, Calories = 480, Allergens = "Wheat,Soy", AvailableTime = "10:00 AM - 9:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Chongqing Xiaomian",     Subtitle = "Chongqing", Description = "Spicy Sichuan-style wheat noodles in a fiery chili and Sichuan peppercorn broth, topped with minced pork, pickled vegetables, and scallions.", Price = 8.99m, PriceTier = "$", Rating = 4.7, ReviewCount = 298, CategoryId = 1, Cuisine = "Chinese", Distance = 0.8, Calories = 450, Allergens = "Wheat", AvailableTime = "7:00 AM - 9:00 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },

                // Drinks (3) — 茶饮果饮保留奶茶
                new() { Name = "Brown Sugar Milk Tea",   Subtitle = "Guangdong", Description = "Freshly brewed black tea swirled with hand-poured brown sugar syrup, chewy tapioca pearls, and a creamy milk cap. The classic bubble tea.", Price = 5.99m, PriceTier = "$", Rating = 4.9, ReviewCount = 467, CategoryId = 2, Cuisine = "Chinese", Distance = 0.2, Calories = 380, Allergens = "Milk", AvailableTime = "10:00 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Matcha Latte",           Subtitle = "Zhejiang", Description = "Stone-ground ceremonial matcha from Hangzhou, whisked with steamed oat milk and touched with osmanthus honey.", Price = 5.99m, PriceTier = "$", Rating = 4.6, ReviewCount = 312, CategoryId = 2, Cuisine = "Chinese", Distance = 0.5, Calories = 220, Allergens = "Milk", AvailableTime = "8:00 AM - 8:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Mango Pomelo Sago",      Subtitle = "Hong Kong", Description = "A refreshing Cantonese dessert drink with ripe mango, pomelo sacs, coconut milk, and tiny sago pearls over crushed ice.", Price = 6.99m, PriceTier = "$$", Rating = 4.8, ReviewCount = 356, CategoryId = 2, Cuisine = "Chinese", Distance = 0.7, Calories = 280, Allergens = "Milk", AvailableTime = "11:00 AM - 9:30 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },

                // Sushi (3) — 保留日料寿司
                new() { Name = "Omakase Sushi Platter",  Subtitle = "Shanghai", Description = "Chef's seasonal selection of 12 pieces of premium nigiri, featuring fresh seafood air-flown daily. Served with house-made soy and wasabi.", Price = 38.99m, PriceTier = "$$$", Rating = 4.9, ReviewCount = 478, CategoryId = 3, Cuisine = "Japanese", Distance = 1.5, Calories = 480, Allergens = "Fish,Soy", AvailableTime = "11:30 AM - 9:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Dragon Roll",            Subtitle = "Guangzhou", Description = "Shrimp tempura and cucumber wrapped inside, layered with sliced avocado and drizzled with house-made spicy unagi sauce. 8 pieces.", Price = 15.99m, PriceTier = "$$", Rating = 4.7, ReviewCount = 334, CategoryId = 3, Cuisine = "Japanese", Distance = 0.5, Calories = 520, Allergens = "Shellfish,Wheat,Soy", AvailableTime = "11:00 AM - 10:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Salmon Poke Bowl",       Subtitle = "Shenzhen", Description = "Fresh Norwegian salmon cubes over seasoned sushi rice with avocado, edamame, cucumber, seaweed salad, and sesame dressing.", Price = 13.99m, PriceTier = "$$", Rating = 4.5, ReviewCount = 256, CategoryId = 3, Cuisine = "Japanese", Distance = 0.8, Calories = 580, Allergens = "Fish,Soy,Sesame", AvailableTime = "10:30 AM - 9:00 PM", IsTrending = false, IsPopular = false, IsRecommended = true, CreatedAt = now },

                // Pizza (3) — 保留披萨
                new() { Name = "Margherita DOP",         Subtitle = "Shanghai", Description = "San Marzano DOP tomato sauce, fresh buffalo mozzarella, garden basil, and extra virgin olive oil on a Neapolitan wood-fired crust.", Price = 12.99m, PriceTier = "$$", Rating = 4.7, ReviewCount = 378, CategoryId = 4, Cuisine = "Italian", Distance = 0.4, Calories = 780, Allergens = "Wheat,Milk", AvailableTime = "11:00 AM - 10:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Sichuan Spicy Pizza",     Subtitle = "Chengdu", Description = "A bold fusion pizza topped with spicy Sichuan peppercorn sausage, roasted bell peppers, red onions, and mozzarella on a thin crispy base.", Price = 13.99m, PriceTier = "$$", Rating = 4.5, ReviewCount = 201, CategoryId = 4, Cuisine = "Fusion", Distance = 0.9, Calories = 850, Allergens = "Wheat,Milk", AvailableTime = "11:30 AM - 10:00 PM", IsTrending = true, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Truffle Mushroom Pizza",  Subtitle = "Beijing", Description = "Wild mushroom medley with black truffle cream sauce, fontina cheese, and fresh thyme on an artisan sourdough base. Earthy and luxurious.", Price = 18.99m, PriceTier = "$$$", Rating = 4.8, ReviewCount = 245, CategoryId = 4, Cuisine = "Italian", Distance = 1.2, Calories = 820, Allergens = "Wheat,Milk", AvailableTime = "5:00 PM - 10:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },

                // Healthy (3) — 中式健康菜
                new() { Name = "Buddha's Delight",       Subtitle = "Guangdong", Description = "A traditional Cantonese vegetarian medley of braised mushrooms, bok choy, bamboo shoots, tofu skin, and glass noodles in a light oyster sauce.", Price = 10.99m, PriceTier = "$$", Rating = 4.3, ReviewCount = 178, CategoryId = 5, Cuisine = "Chinese", Distance = 0.5, Calories = 320, Allergens = "Soy", AvailableTime = "10:00 AM - 9:00 PM", IsTrending = false, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Steamed Ginger Fish",     Subtitle = "Fujian", Description = "Fresh whole sea bass steamed to perfection with ginger slivers, scallions, and a splash of hot soy sauce. Light, clean, and full of umami.", Price = 16.99m, PriceTier = "$$$", Rating = 4.7, ReviewCount = 289, CategoryId = 5, Cuisine = "Chinese", Distance = 0.7, Calories = 280, Allergens = "Fish,Soy", AvailableTime = "11:00 AM - 9:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Cold Sesame Noodles",     Subtitle = "Sichuan", Description = "Chilled wheat noodles tossed with shredded chicken, cucumber, bean sprouts, and a nutty sesame-peanut sauce with a hint of chili oil.", Price = 8.99m, PriceTier = "$", Rating = 4.5, ReviewCount = 312, CategoryId = 5, Cuisine = "Chinese", Distance = 0.4, Calories = 380, Allergens = "Wheat,Sesame,Peanut", AvailableTime = "10:00 AM - 8:30 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },

                // Desserts (3) — 中式甜品为主
                new() { Name = "Mango Sticky Rice",      Subtitle = "Yunnan", Description = "Sweet ripe mango served over warm coconut-infused sticky rice, drizzled with creamy coconut sauce and topped with crispy mung beans.", Price = 5.99m, PriceTier = "$", Rating = 4.8, ReviewCount = 423, CategoryId = 6, Cuisine = "Chinese", Distance = 0.6, Calories = 360, Allergens = "Milk", AvailableTime = "10:00 AM - 9:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Glutinous Rice Tangyuan", Subtitle = "Jiangsu", Description = "Hand-rolled glutinous rice balls filled with sweet black sesame paste, served in a warm osmanthus-infused syrup. A beloved festival dessert.", Price = 4.99m, PriceTier = "$", Rating = 4.6, ReviewCount = 267, CategoryId = 6, Cuisine = "Chinese", Distance = 0.3, Calories = 300, Allergens = "Sesame", AvailableTime = "9:00 AM - 9:00 PM", IsTrending = false, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Portuguese Egg Tart",     Subtitle = "Macau", Description = "Crispy, flaky layered pastry shell filled with a silky smooth caramelized egg custard, still warm from the oven. A Macau bakery classic.", Price = 3.99m, PriceTier = "$", Rating = 4.9, ReviewCount = 489, CategoryId = 6, Cuisine = "Chinese", Distance = 0.2, Calories = 220, Allergens = "Wheat,Milk,Egg", AvailableTime = "7:00 AM - 8:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },

                // Coffee (3) — 保留咖啡
                new() { Name = "Yunnan Pour Over",        Subtitle = "Yunnan", Description = "Single-origin Arabica from Pu'er, Yunnan — hand-poured V60. Notes of dark chocolate, roasted nuts, and a hint of stone fruit.", Price = 5.99m, PriceTier = "$$", Rating = 4.5, ReviewCount = 234, CategoryId = 7, Cuisine = "Chinese", Distance = 0.3, Calories = 5, Allergens = "", AvailableTime = "7:00 AM - 7:00 PM", IsTrending = false, IsPopular = false, IsRecommended = true, CreatedAt = now },
                new() { Name = "Coconut Cold Brew",       Subtitle = "Hainan", Description = "Smooth cold-brewed coffee blended with fresh Hainan coconut water, served over ice. A refreshing tropical pick-me-up.", Price = 5.49m, PriceTier = "$$", Rating = 4.7, ReviewCount = 345, CategoryId = 7, Cuisine = "Chinese", Distance = 0.5, Calories = 80, Allergens = "", AvailableTime = "8:00 AM - 8:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Spanish Latte",           Subtitle = "Shanghai", Description = "Rich double espresso with condensed milk and steamed whole milk — a sweet, creamy, indulgent latte crafted in Shanghai's coffee scene.", Price = 5.99m, PriceTier = "$$", Rating = 4.6, ReviewCount = 298, CategoryId = 7, Cuisine = "Fusion", Distance = 0.4, Calories = 190, Allergens = "Milk", AvailableTime = "7:30 AM - 8:30 PM", IsTrending = false, IsPopular = true, IsRecommended = false, CreatedAt = now },

                // Chinese (3) — 中华地方名菜
                new() { Name = "Mapo Tofu",              Subtitle = "Sichuan", Description = "Silky soft tofu cubes in a fiery, numbing sauce of minced beef, fermented broad bean paste, Sichuan peppercorns, and chili oil. A Chengdu icon.", Price = 9.99m, PriceTier = "$$", Rating = 4.8, ReviewCount = 456, CategoryId = 8, Cuisine = "Chinese", Distance = 0.6, Calories = 420, Allergens = "Soy", AvailableTime = "10:00 AM - 9:30 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Dongpo Braised Pork",     Subtitle = "Zhejiang", Description = "Melt-in-your-mouth pork belly slow-braised in soy sauce, Shaoxing wine, and rock sugar until caramelized and tender. A Hangzhou masterpiece.", Price = 14.99m, PriceTier = "$$$", Rating = 4.9, ReviewCount = 398, CategoryId = 8, Cuisine = "Chinese", Distance = 1.0, Calories = 650, Allergens = "Soy", AvailableTime = "11:00 AM - 9:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
                new() { Name = "Crispy Roast Duck",       Subtitle = "Beijing", Description = "Legendary Beijing roast duck with crackling golden skin, carved tableside, served with thin pancakes, cucumber, scallion, and sweet bean sauce.", Price = 28.99m, PriceTier = "$$$", Rating = 4.9, ReviewCount = 567, CategoryId = 8, Cuisine = "Chinese", Distance = 1.2, Calories = 720, Allergens = "Wheat,Soy", AvailableTime = "11:00 AM - 10:00 PM", IsTrending = true, IsPopular = true, IsRecommended = true, CreatedAt = now },
            };

            // Stagger CreatedAt timestamps (10 min apart, spanning ~4 hours)
            for (int i = 0; i < items.Count; i++)
                items[i].CreatedAt = now.AddMinutes(-240 + i * 10);

            await db.InsertAllAsync(categories);
            await db.InsertAllAsync(items);
        }
    }
}
