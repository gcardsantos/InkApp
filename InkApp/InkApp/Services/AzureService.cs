﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InkApp.Services
{
    public class AzureService<T>
    {
        IMobileServiceClient Client;
        IMobileServiceTable<T> Table;

        public AzureService()
        {
            string MyAppServiceURL = "https://tattoapp.azurewebsites.net";
            Client = new MobileServiceClient(MyAppServiceURL);
            Table = Client.GetTable<T>();
        }

        public Task<IEnumerable<T>> GetTable()
        {
            return Table.ToEnumerableAsync();
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

        public async Task<bool> UpdateItemAsync(int id, T item)
        {
            try
            {
                T target = await Table.LookupAsync(id);

                if (target != null)
                {
                    await Table.UpdateAsync(item);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
    }
}
