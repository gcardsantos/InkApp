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
        private string _home;
        public string Home { get { return _home; } set { SetProperty(ref _home, value); } }

        private string _feed;
        public string Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private string _photos;
        public string Photos { get { return _photos; } set { SetProperty(ref _photos, value); } }

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

        public void Navigate(string path)
        {
            NavigateAsync(path);
        }

        public async System.Threading.Tasks.Task NavigateAsync(string path)
        {
            await _navigationService.NavigateAsync(path);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
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
