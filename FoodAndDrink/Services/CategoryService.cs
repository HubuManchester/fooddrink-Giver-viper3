using FoodAndDrink.Models;
using SQLite;

namespace FoodAndDrink.Services
{
    /// <summary>
    /// Provides read access to the Categories table. Categories are seeded once
    /// at database creation and are read-only at runtime.
    /// </summary>
    public class CategoryService
    {
        private readonly DatabaseService _dbService;

        public CategoryService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        private async Task<SQLiteAsyncConnection> GetDbAsync()
            => await _dbService.GetConnectionAsync();

        public async Task<List<Category>> GetAllAsync()
        {
            try
            {
                var db = await GetDbAsync();
                return await db.Table<Category>().OrderBy(c => c.SortOrder).ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategoryService] Error: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                var db = await GetDbAsync();
                return await db.Table<Category>().Where(c => c.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategoryService] Error: {ex.Message}");
                return null;
            }
        }
    }
}
