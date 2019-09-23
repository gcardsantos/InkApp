using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InkApp.Services
{
    public class AzureService<T>
    {
        public IMobileServiceClient Client;
        public IMobileServiceTable<T> Table;

        public AzureService()
        {
            string MyAppServiceURL = "https://tattoapp.azurewebsites.net";
            Client = new MobileServiceClient(MyAppServiceURL);
            Table = Client.GetTable<T>();
        }

        public Task<List<T>> GetTable()
        {
            return Table.ToListAsync();
        }

        public bool AddItem(T item)
        {
            try
            {
                Table.InsertAsync(item);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async void UpdateItemAsync(string id, JObject item)
        {
            try
            {
                T target = await Table.LookupAsync(id);
                
                if (target != null)
                {
                    await Table.UpdateAsync(item);
                }

            }
            catch (Exception)
            {
                
            }
        }

        internal async Task DeleteItemAsync(Pessoa p, JObject item)
        {
            try
            {
                T target = await Table.LookupAsync(p.Id);

                if (target != null)
                {
                    await Table.DeleteAsync(item);
                }

            }
            catch (Exception)
            {

            }
        }
    }
}
