using InkApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InkApp.Data
{
    public class PhotoDatabase
    {
        readonly SQLiteAsyncConnection database;


        public PhotoDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<InstagramItem>().Wait();
        }

        public Task<List<InstagramItem>> GetItemsAsync()
        {
            return database.Table<InstagramItem>().ToListAsync();
        }

        public Task<InstagramItem> GetItemAsync(int id)
        {
            return database.Table<InstagramItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<InstagramItem> GetItemAsync(InstagramItem item)
        {
            return database.Table<InstagramItem>().Where(i => i.ImageLow == item.ImageLow).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(InstagramItem item)
        {
            if (item.Id != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(InstagramItem item)
        {
            return database.DeleteAsync(item);
        }

    }
}
