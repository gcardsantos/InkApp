using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkApp.ViewModels
{
    public class TopMasterDetailPageViewModel : BindableBase, INavigatedAware
    {
        private string _quant;
        public string QuantTatuadores { get { return _quant; } set { SetProperty(ref _quant, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        public DelegateCommand<string> NavigateCommand { get; }
        public INavigationService _navigationService;
        public TopMasterDetailPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(Navigate);

            StartValue();
            
        }

        private async void StartValue()
        {
            IsBusy = true;
            QuantTatuadores = (await App.Repository.GetQuantPessoas()).ToString();
            IsBusy = false;
        }

        public async void Navigate(string path)
        {
            await NavigateAsync(path);
        }

        public async System.Threading.Tasks.Task NavigateAsync(string path)
        {
            await _navigationService.NavigateAsync(path);
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedFromAsync(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }
    }
}
