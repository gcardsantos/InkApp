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

        private string _home;
        public string Home { get { return _home; } set { SetProperty(ref _home, value); } }

        private string _feed;
        public string Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private string _photos;
        public string Photos { get { return _photos; } set { SetProperty(ref _photos, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _visible;
        public bool Visible { get { return _visible; } set { SetProperty(ref _visible, value); } }

        public DelegateCommand<string> NavigateCommand { get; }
        public INavigationService _navigationService;
        public TopMasterDetailPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(Navigate);
            Home = "\uf2dc";
            Feed = "\uf238";
            Photos = "\uf193";
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
            IsBusy = true;
            Visible = false;
            int tentativas = 0;

            while (tentativas < 2 && String.IsNullOrWhiteSpace(QuantTatuadores))
            {
                try
                {
                    QuantTatuadores = (await (new Repository()).GetQuantPessoas()).ToString();
                }
                catch (Exception)
                {
                    tentativas++;
                }
            }
            
            
            Visible = true;
            IsBusy = false;
        }

        public void OnNavigatedFromAsync(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }
    }
}
