using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InkApp.ViewModels
{
    public class BasePeoplePageViewModel : ViewModelBase
    {
        private Repository repository;

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _isCollection;
        public bool CollectionVisible { get { return _isCollection; } set { SetProperty(ref _isCollection, value); } }

        private ObservableCollection<Pessoa> _pessoas;
        public ObservableCollection<Pessoa> Pessoas { get { return _pessoas; } set { SetProperty(ref _pessoas, value); } }

        private Pessoa _pessoa;
        public Pessoa People { get { return _pessoa; } set { SetProperty(ref _pessoa, value); } }

        public DelegateCommand OpenProfileCommand { get; }
        public DelegateCommand<string> FilterCommand { get; private set; }
        public BasePeoplePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Pessoas = new ObservableCollection<Pessoa>();
            FilterCommand = new DelegateCommand<string>(FilterPeople);
            OpenProfileCommand = new DelegateCommand(OpenProfile);
            StartValues();
        }

        private async void StartValues()
        {
            IsBusy = true;
            CollectionVisible = false;
            var x = await repository.GetPessoas();
            await App.Api.GetUserAsync(x);

            foreach (var i in x.OrderBy(n => n.Name).ToList())
            {
                Pessoas.Add(i);
            }
            IsBusy = false;
            CollectionVisible = true;
        }

        private void FilterPeople(string obj)
        {
            
        }

        private async void OpenProfile()
        {
            if(People != null)
            {
                var navigationParams = new NavigationParameters
                {
                    { "pessoa", People }
                };
                await NavigationService.NavigateAsync("DetailsPage", navigationParams, false);
            }
            
        }

    }
}
