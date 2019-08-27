using InkApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InkApp.ViewModels
{
    public class SavePhotosPageViewModel : ViewModelBase
    {
        public DelegateCommand PhotoTappedCommand { get; private set; }

        private ObservableCollection<InstagramItem> _feed;
        public ObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private InstagramItem _lastItemTapped;
        public InstagramItem LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _nothing;
        public bool Nothing { get { return _nothing; } set { SetProperty(ref _nothing, value); } }


        private bool _isCollection;
        public bool CollectionVisible { get { return _isCollection; } set { SetProperty(ref _isCollection, value); } }

        public SavePhotosPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Nothing = false;
            StartValue();
            PhotoTappedCommand = new DelegateCommand(OpenPhoto);
        }

        public async void StartValue()
        {
            IsBusy = true;
            CollectionVisible = false;
            List<InstagramItem> list = await App.Database.GetItemsAsync();

            Nothing = list.Count == 0 ? true : false;

            Feed = new ObservableCollection<InstagramItem>(list);

            IsBusy = false;
            CollectionVisible = true;
        }

        public async void OpenPhoto()
        {
            if(LastTappedItem != null)
            {
                NavigationParameters np = new NavigationParameters
                {
                    { "photo", LastTappedItem }
                };
                await NavigationService.NavigateAsync("ImagePage", np);
            }            
        }
    }
}
