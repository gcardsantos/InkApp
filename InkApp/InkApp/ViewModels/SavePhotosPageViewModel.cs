using DLToolkit.Forms.Controls;
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
    public class SavePhotosPageViewModel : ViewModelBase
    {
        public DelegateCommand LoadingCommand { get; private set; }
        public DelegateCommand<object> PhotoTappedCommand { get; private set; }

        private FlowObservableCollection<InstagramItem> _feed;
        public FlowObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private object _lastItemTapped;
        public object LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }


        public SavePhotosPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            StartValue();
            PhotoTappedCommand = new DelegateCommand<object>(OpenPhoto);
        }

        public async void StartValue()
        {
            List<InstagramItem> list = await App.Database.GetItemsAsync();
            Feed = new FlowObservableCollection<InstagramItem>(list);
        }

        public async void OpenPhoto(object obj)
        {
            NavigationParameters np = new NavigationParameters();
            np.Add("photo", obj);

            await NavigationService.NavigateAsync("ImagePage", np);
        }
    }
}
