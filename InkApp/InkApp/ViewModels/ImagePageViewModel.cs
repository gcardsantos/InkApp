﻿using InkApp.Data;
using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace InkApp.ViewModels
{
    public class ImagePageViewModel : ViewModelBase
    {
        public IPageDialogService PageDialogService { get; }
        public DelegateCommand PhotoCommand { get; set; }
        public DelegateCommand ProfileCommand { get; set; }
        public DelegateCommand DeleteCommand { get; }

        public InstagramItem item;
        
        public InstagramItem Item { get { return item; } set { SetProperty(ref item, value); } }

        public Color _cor;
        public Color Cor { get { return _cor; } set { SetProperty(ref _cor, value); } }

        public Pessoa _pessoa;

        public Pessoa Pessoa { get { return _pessoa; } set { SetProperty(ref _pessoa, value); } }

        private string _image;
        public string ImageHigh { get { return _image; } set { SetProperty(ref _image, value); } }

        private string _profile;
        public string Profile { get { return _profile; } set { SetProperty(ref _profile, value); } }

        private string _name;
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        private string _heart;
        public string Heart { get { return _heart; } set { SetProperty(ref _heart, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }
        public ImagePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) :base(navigationService)
        {
            PageDialogService = pageDialogService;
            PhotoCommand = new DelegateCommand(SavePhoto);
            ProfileCommand = new DelegateCommand(GoProfile);
            DeleteCommand = new DelegateCommand(DeletePhoto);
        }

        private async void GoProfile()
        {
            var navigationParams = new NavigationParameters();
            Pessoa.Name = Name;
            navigationParams.Add("pessoa", Pessoa);
            navigationParams.Add("photo", Item);
            await NavigationService.NavigateAsync("DetailsPage", navigationParams);
        }

        private void DeletePhoto()
        {
            
        }

        private async void SavePhoto()
        {
            try
            {
                if (Color.Black == Cor)
                {
                    Item.Name = Name;
                    Item.Username = Pessoa.Username;
                    var c = await App.Database.SaveItemAsync(Item);

                    if (c == 1)
                    {
                        Cor = Color.Red;
                        Heart = "\uf2d1";
                    }
                        
                    await PageDialogService.DisplayAlertAsync("Salvo", "Salvo com sucesso.", "Ok");
                }
                else
                {
                    var c = await App.Database.DeleteItemAsync(Item);

                    if (c == 1)
                    {
                        Cor = Color.Black;
                        Heart = "\uf2d5";
                    }
                    await PageDialogService.DisplayAlertAsync("Removido", "Removido com sucesso.", "Ok");
                }
            }
            catch (Exception)
            {
                await PageDialogService.DisplayAlertAsync("Erro", "Ocorreu um erro ao salvar/remover foto.", "Ok");
            }
            

            await NavigationService.GoBackAsync();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if(Item == null)
            {
                IsBusy = true;
                Item = parameters["photo"] as InstagramItem;
                Pessoa = parameters["pessoa"] as Pessoa;
                ImageHigh = Item.ImageHigh;

                try
                {
                    if (Pessoa == null)
                    {
                        Pessoa = await (new Repository().GetPessoa(Item.Name));
                        await App.Api.GetUserAsync(Pessoa);
                        IsBusy = false;
                        Item.People = Pessoa;
                        Profile = Pessoa.Image;
                    }

                    Name = Item.Name;

                    var x = await App.Database.GetItemAsync(Item);

                    if (x == null)
                    {
                        Cor = Color.Black;
                        Heart = "\uf2d5";
                    }
                    else
                    {
                        Cor = Color.Red;
                        Heart = "\uf2d1";
                    }

                }
                catch (Exception)
                {
                    await NavigationService.NavigateAsync("ErrorConectionPage");
                }
            }
            
        }
    }
}
