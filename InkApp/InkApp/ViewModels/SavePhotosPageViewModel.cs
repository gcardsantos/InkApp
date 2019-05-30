using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InkApp.ViewModels
{
    public class SavePhotosPageViewModel : BindableBase
    {
        private object _item;
        public object Item { get { return _item; } set { SetProperty(ref _item, value); } }

        private ObservableCollection<InstagramItem> _photos;
        public ObservableCollection<InstagramItem> Photos { get { return _photos; } set { SetProperty(ref _photos, value); } }

        public DelegateCommand command;
        public SavePhotosPageViewModel()
        {
            StartValue();
            command = new DelegateCommand(DeletePhoto);
        }

        public async void StartValue()
        {
            List<InstagramItem> list = await App.Database.GetItemsAsync();
            Photos = new ObservableCollection<InstagramItem>(list);
        }

        public async void DeletePhoto()
        {
            await App.Database.DeleteItemAsync(Item as InstagramItem);
        }
    }
}
