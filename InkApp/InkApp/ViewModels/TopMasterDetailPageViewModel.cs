using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class TopMasterDetailPageViewModel : BindableBase, INavigatedAware
    {
        public DelegateCommand<string> NavigateCommand { get; }
        public INavigationService _navigationService;
        public TopMasterDetailPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(SavePhotos);
        }

        public void SavePhotos(string path)
        {
            _navigationService.NavigateAsync(path);
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
