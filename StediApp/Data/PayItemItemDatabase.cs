using StediApp.Models;
using SQLite;

namespace StediApp.Data
{
    public class StavkaItemDatabase
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<StavkaItemDatabase> Instance =
            new AsyncLazy<StavkaItemDatabase>(async () =>
            
            {
                var instance = new StavkaItemDatabase();
                try
                {
                    CreateTableResult expense = await Database.CreateTableAsync<Stavka>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
                return instance;
            });

        public StavkaItemDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<Stavka>> GetItemsAsync()
        {
            return Database.Table<Stavka>().ToListAsync();
        }

        public async Task<List<Stavka>> GetExpenseItems()
        {
            try
            {
                return await Database.Table<Stavka>().Where(i => i.IsExpense).ToListAsync();
            }
            catch (Exception)
            {
                return await ReturnEmptyList();
            }
        }

        public async Task<List<Stavka>> GetIncomeItems()
        {
            try
            {
                return await Database.Table<Stavka>().Where(i => i.IsIncome).ToListAsync();
            }
            catch (Exception)
            {

                return await ReturnEmptyList();
            }
        }

        public Task<Stavka> GetItemAsync(int id)
        {
            return Database.Table<Stavka>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(Stavka item)
        {
            if (item.ID != 0)
            {
                return await Database.UpdateAsync(item);
            }
            else
            {
                item.IsVisible = true;
                var res = await Database.InsertAsync(item);
                return res;
            }
        }

        public async Task<int> DeleteItemAsync(Stavka item)
        {
            try
            {
                return await Database.DeleteAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.CompletedTask;
                return 0;
            }

        }

        private async Task<List<Stavka>> ReturnEmptyList()
        {
            await Task.CompletedTask;
            List<Stavka> empty = new List<Stavka>();
            return empty;
        }
    }
}
