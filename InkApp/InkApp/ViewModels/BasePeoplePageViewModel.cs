using DLToolkit.Forms.Controls;
using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class BasePeoplePageViewModel : ViewModelBase
    {
        private Repository repository;

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private FlowObservableCollection<Pessoa> _pessoas;
        public FlowObservableCollection<Pessoa> Pessoas { get { return _pessoas; } set { SetProperty(ref _pessoas, value); } }

        private Pessoa _pessoa;
        public Pessoa People { get { return _pessoa; } set { SetProperty(ref _pessoa, value); } }

        public DelegateCommand OpenProfileCommand { get; }
        public DelegateCommand<string> FilterCommand { get; private set; }
        public BasePeoplePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Pessoas = new FlowObservableCollection<Pessoa>();
            FilterCommand = new DelegateCommand<string>(FilterPeople);
            OpenProfileCommand = new DelegateCommand(OpenProfile);
            StartValues();
        }

        private async void StartValues()
        {
            IsBusy = true;
            var x = await repository.GetPessoas();
            await App.Api.GetUserAsync(x);
            Pessoas.AddRange(x.OrderBy(n => n.Name).ToList());
            IsBusy = false;
        }

        private void FilterPeople(string obj)
        {
            
        }

        private async void OpenProfile()
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("pessoa", People);
            await NavigationService.NavigateAsync("DetailsPage", navigationParams, false);
        }

    }
}
