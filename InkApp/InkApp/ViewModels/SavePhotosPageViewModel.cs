using DLToolkit.Forms.Controls;
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
        public DelegateCommand LoadingCommand { get; private set; }
        public DelegateCommand<object> PhotoTappedCommand { get; private set; }

        private FlowObservableCollection<InstagramItem> _feed;
        public FlowObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private object _lastItemTapped;
        public object LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _loading;
        public bool IsLoadingInfinite { get { return _loading; } set { SetProperty(ref _loading, value); } }

        private bool _nothing;
        public bool Nothing { get { return _nothing; } set { SetProperty(ref _nothing, value); } }

        public SavePhotosPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Nothing = false;
            StartValue();
            PhotoTappedCommand = new DelegateCommand<object>(OpenPhoto);
            LoadingCommand = new DelegateCommand(async () =>
            {
                await LoadMoreAsync();
            });
        }

        protected virtual async Task LoadMoreAsync()
        {
            var oldTotal = Feed.Count;

            await Task.Delay(3000);

            var howMany = 60;

            var items = new List<InstagramItem>();

            for (int i = oldTotal; i < oldTotal + howMany; i++)
            {
                items.Add(new InstagramItem());
            }

            Feed.AddRange(items);

            IsLoadingInfinite = false;
        }

        public async void StartValue()
        {
            List<InstagramItem> list = await App.Database.GetItemsAsync();

            Nothing = list.Count == 0 ? true : false;

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
