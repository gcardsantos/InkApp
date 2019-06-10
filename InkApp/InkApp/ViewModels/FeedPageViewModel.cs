using DLToolkit.Forms.Controls;
using InkApp.Models;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
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
    public class FeedPageViewModel : ViewModelBase
    {
        private int maxPage = 1;
        private Repository repository;
        public DelegateCommand LoadingCommand { get; private set; }

        private FlowObservableCollection<InstagramItem> _feed;
        public FlowObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private object _lastItemTapped;
        public object LastItemTapped { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Feed = new FlowObservableCollection<InstagramItem>();
            LoadingCommand = new DelegateCommand(GetMoreData);
        }



        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }


        public void GetMoreData()
        {
            try
            {
                IsBusy = true;
                var pessoas = Task.Run(async () => { return await repository.GetPessoas(); }).Result;
                pessoas.OrderBy(n => Guid.NewGuid());
                pessoas.RemoveRange(pessoas.Count / 2, pessoas.Count - 1);
                foreach (Pessoa p in pessoas)
                    _ = GetDataAsync(p);
            }
            finally
            {
                maxPage += 2;
                IsBusy = false;
            }
        }

        public async Task GetDataAsync(Pessoa p)
        {
            var data = await App.Api.GetMediaAsync(p);

            if (data != null)
            {
                Feed.AddRange(data.Where(n => Feed.Any(e => e.ImageLow.Equals(n.ImageLow))));
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {     
                GetMoreData();
                //_ = GetData(_pessoa);
            }
        }
    }
}
