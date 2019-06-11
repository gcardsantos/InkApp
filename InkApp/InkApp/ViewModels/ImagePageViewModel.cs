using InkApp.Data;
using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class ImagePageViewModel : ViewModelBase
    {
        public DelegateCommand PhotoCommand;

        public InstagramItem item;
        public InstagramItem Item { get { return item; } set { SetProperty(ref item, value); } }


        private string _image;
        public string ImageHigh { get { return _image; } set { SetProperty(ref _image, value); } }
        public ImagePageViewModel(INavigationService navigationService) :base(navigationService)
        {
            PhotoCommand = new DelegateCommand(SavePhoto);
        }

        private void SavePhoto()
        {
            App.Database.SaveItemAsync(Item);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Item = parameters["photo"] as InstagramItem;
            ImageHigh = Item.ImageHigh;
        }
    }
}
