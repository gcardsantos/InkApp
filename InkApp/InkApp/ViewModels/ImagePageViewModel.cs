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
        public DelegateCommand PhotoCommand { get; set; }
        public DelegateCommand ProfileCommand { get; set; }

        public DelegateCommand DeleteCommand { get; }

        public InstagramItem item;
        
        public InstagramItem Item { get { return item; } set { SetProperty(ref item, value); } }

        public Pessoa _pessoa;

        public Pessoa Pessoa { get { return _pessoa; } set { SetProperty(ref _pessoa, value); } }

        private string _image;
        public string ImageHigh { get { return _image; } set { SetProperty(ref _image, value); } }
        public ImagePageViewModel(INavigationService navigationService) :base(navigationService)
        {
            PhotoCommand = new DelegateCommand(SavePhoto);
            ProfileCommand = new DelegateCommand(GoProfile);
            DeleteCommand = new DelegateCommand(DeletePhoto);
        }

        private async void GoProfile()
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("pessoa", Pessoa);
            await NavigationService.NavigateAsync("DetailsPage", navigationParams, false);
        }

        private void DeletePhoto()
        {
            App.Database.DeleteItemAsync(Item);
        }

        private void SavePhoto()
        {
            App.Database.SaveItemAsync(Item);
        }



        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Item = parameters["photo"] as InstagramItem;
            Pessoa = Item.People;
            ImageHigh = Item.ImageHigh;
        }
    }
}
