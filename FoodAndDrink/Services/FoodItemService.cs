using System.Diagnostics;
using FoodAndDrink.Models;
using SQLite;

namespace FoodAndDrink.Services
{
    public class FoodItemService
    {
        private readonly DatabaseService _dbService;

        public FoodItemService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        private async Task<SQLiteAsyncConnection> GetDbAsync()
        {
            return await _dbService.GetConnectionAsync();
        }

        public async Task<List<FoodItem>> GetRandomAsync(int count)
        {
            try
            {
                var db = await GetDbAsync();
                var all = await db.Table<FoodItem>().ToListAsync();
                var result = all.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
                await FillIconEmojiAsync(result);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetRandomAsync failed: {ex.Message}");
                return new();
            }
        }

        public async Task<List<FoodItem>> GetByCategoryAsync(int categoryId, string sortBy = "rating")
        {
            var db = await GetDbAsync();
            var query = db.Table<FoodItem>().Where(i => i.CategoryId == categoryId);
            query = ApplySorting(query, sortBy);
            var result = await query.ToListAsync();
            await FillIconEmojiAsync(result);
            return result;
        }

        public async Task<List<FoodItem>> QueryAsync(ItemFilter filter)
        {
            try
            {
                var db = await GetDbAsync();
                var query = db.Table<FoodItem>();

                if (filter.CategoryId.HasValue)
                    query = query.Where(i => i.CategoryId == filter.CategoryId.Value);
                if (filter.MinRating.HasValue)
                    query = query.Where(i => i.Rating >= filter.MinRating.Value);
                if (filter.MaxDistance.HasValue)
                    query = query.Where(i => i.Distance <= filter.MaxDistance.Value);
                if (!string.IsNullOrEmpty(filter.PriceTier))
                    query = query.Where(i => i.PriceTier == filter.PriceTier);
                if (!string.IsNullOrEmpty(filter.Cuisine))
                    query = query.Where(i => i.Cuisine == filter.Cuisine);

                query = ApplySorting(query, filter.SortBy ?? "rating");
                var result = await query.ToListAsync();
                await FillIconEmojiAsync(result);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] QueryAsync: {ex.Message}");
                return new List<FoodItem>();
            }
        }

        public async Task<List<FoodItem>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            try
            {
                var db = await GetDbAsync();
                var result = await db.Table<FoodItem>()
                    .Where(i => i.Name.Contains(keyword) || i.Cuisine.Contains(keyword) || i.Subtitle.Contains(keyword))
                    .ToListAsync();
                await FillIconEmojiAsync(result);
                return result;
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] DB Error: {ex.Message}");
                return new List<FoodItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] Unexpected: {ex.Message}");
                return new List<FoodItem>();
            }
        }

        public async Task<List<FoodItem>> GetAllAsync(string sortBy = "rating")
        {
            var db = await GetDbAsync();
            var query = ApplySorting(db.Table<FoodItem>(), sortBy);
            var result = await query.ToListAsync();
            await FillIconEmojiAsync(result);
            return result;
        }

        public async Task<List<FoodItem>> GetFavoritesAsync()
        {
            try
            {
                var db = await GetDbAsync();
                var result = await db.Table<FoodItem>().Where(i => i.IsFavorite).ToListAsync();
                await FillIconEmojiAsync(result);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetFavorites: {ex.Message}");
                return new();
            }
        }

        public async Task ToggleFavoriteAsync(int itemId)
        {
            try
            {
                var db = await GetDbAsync();
                var item = await db.Table<FoodItem>().Where(i => i.Id == itemId).FirstOrDefaultAsync();
                if (item is not null)
                {
                    item.IsFavorite = !item.IsFavorite;
                    await db.UpdateAsync(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] ToggleFavorite: {ex.Message}");
            }
        }

        public async Task<FoodItem?> GetByIdAsync(int id)
        {
            try
            {
                var db = await GetDbAsync();
                var item = await db.Table<FoodItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (item is not null)
                {
                    item.ViewedAt = DateTime.Now;
                    await db.UpdateAsync(item);
                    var cat = await db.Table<Category>().Where(c => c.Id == item.CategoryId).FirstOrDefaultAsync();
                    item.IconEmoji = cat?.IconEmoji ?? "🍽️";
                }
                return item;
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] DB Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] Unexpected: {ex.Message}");
                return null;
            }
        }

        public async Task<List<FoodItem>> GetRecentlyViewedAsync(int count = 5)
        {
            try
            {
                var db = await GetDbAsync();
                var result = await db.Table<FoodItem>()
                    .Where(i => i.ViewedAt != null)
                    .OrderByDescending(i => i.ViewedAt)
                    .Take(count)
                    .ToListAsync();
                await FillIconEmojiAsync(result);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] GetRecentlyViewed: {ex.Message}");
                return new List<FoodItem>();
            }
        }

        public async Task ClearRecentlyViewedAsync()
        {
            try
            {
                var db = await GetDbAsync();
                var items = await db.Table<FoodItem>().Where(i => i.ViewedAt != null).ToListAsync();
                foreach (var item in items)
                {
                    item.ViewedAt = null;
                    await db.UpdateAsync(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodItemService] ClearViewed error: {ex.Message}");
            }
        }

        public async Task<int> GetFavoriteCountAsync()
        {
            try
            {
                var db = await GetDbAsync();
                return await db.Table<FoodItem>().Where(i => i.IsFavorite).CountAsync();
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> GetViewedCountAsync()
        {
            try
            {
                var db = await GetDbAsync();
                return await db.Table<FoodItem>().Where(i => i.ViewedAt != null).CountAsync();
            }
            catch
            {
                return 0;
            }
        }

        private async Task FillIconEmojiAsync(List<FoodItem> items)
        {
            if (items.Count == 0)
                return;
            try
            {
                var db = await GetDbAsync();
                var categories = await db.Table<Category>().ToListAsync();
                var catDict = new Dictionary<int, string>();
                foreach (var cat in categories)
                    catDict[cat.Id] = cat.IconEmoji;
                foreach (var item in items)
                {
                    if (catDict.TryGetValue(item.CategoryId, out var emoji))
                        item.IconEmoji = emoji;
                }
            }
            catch
            {
            }
        }

        private static AsyncTableQuery<FoodItem> ApplySorting(AsyncTableQuery<FoodItem> query, string sortBy)
        {
            if (sortBy == "price")
                return query.OrderBy(i => i.Price);
            if (sortBy == "distance")
                return query.OrderBy(i => i.Distance);
            if (sortBy == "newest")
                return query.OrderByDescending(i => i.CreatedAt);
            return query.OrderByDescending(i => i.Rating);
        }
    }
}
