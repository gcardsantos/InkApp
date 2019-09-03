
using InkApp.Models;
using Prism.Commands;
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
        private Repository repository;
        public DelegateCommand<string> FilterCommand { get; private set; }
        public DelegateCommand PhotoTappedCommand { get; private set; }

        public DelegateCommand TopCommand { get; private set; }
        public DelegateCommand LoadingCommand { get; set; }

        private List<InstagramItem> items;

        private ObservableCollection<InstagramItem> _feed;
        public ObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }


        private ObservableCollection<InstagramItem> _toplist;
        public ObservableCollection<InstagramItem> TopList { get { return _toplist; } set { SetProperty(ref _toplist, value); } }

        private InstagramItem _lastItemTapped;
        public InstagramItem LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _loadMore;
        public bool IsLoadMore { get { return _loadMore; } set { SetProperty(ref _loadMore, value); } }


        private bool _isCollection;
        public bool CollectionVisible { get { return _isCollection; } set { SetProperty(ref _isCollection, value); } }

        private int _position;
        public int Position { get { return _position; } set { SetProperty(ref _position, value); } }

        public List<Pessoa> PeopleAdded { get; set; }

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Feed = new ObservableCollection<InstagramItem>();
            TopList = new ObservableCollection<InstagramItem>();
            items = new List<InstagramItem>();
            PeopleAdded = new List<Pessoa>();
            FilterCommand = new DelegateCommand<string>(FilterData);
            PhotoTappedCommand = new DelegateCommand(OpenPhotoAsync);
            LoadingCommand = new DelegateCommand(LoadMoreData);
            TopCommand = new DelegateCommand(CardOpenPhoto);
            StartValueAsync();
        }

        private async void CardOpenPhoto()
        {
            NavigationParameters np = new NavigationParameters
            {
                { "photo", TopList[Position] }
            };

            await NavigationService.NavigateAsync("ImagePage", np, false);
        }

        private async void LoadMoreData()
        {
            await GetMoreDataAsync();
        }

        private void FilterData(string obj)
        {
            Feed.Clear();
            if (obj != "All")
            {
                foreach(var x in items.Where(n => n.Tags.Contains(obj)))
                {
                    Feed.Add(x);
                }
            }
            else
            {
                foreach (var x in items)
                {
                    Feed.Add(x);
                }
            }
        }

        private async void StartValueAsync()
        {
            CollectionVisible = false;
            await GetMoreDataAsync();
            foreach (var x in items.GetRange(new Random().Next(0, items.Count - 1), 5))
            {
                TopList.Add(x);
            }
            CollectionVisible = true;
        }

        private async void OpenPhotoAsync()
        {
            if(LastTappedItem != null)
            {
                var x = Feed.First(n => n.ImageLow.Equals(LastTappedItem.ImageLow));
                NavigationParameters np = new NavigationParameters
                {
                    { "photo", x }
                };
                await NavigationService.NavigateAsync("ImagePage", np);
            }
            
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public async Task GetMoreDataAsync()
        {
            try
            {
                IsBusy = true;
                IsLoadMore = false;
                var pessoas =  await repository.GetShufflePessoas();
                pessoas = pessoas.Where(n => !PeopleAdded.Exists(e => e.Username.Equals(n.Username))).ToList();
                pessoas.RemoveRange(pessoas.Count/2, pessoas.Count/2);
                
                foreach (Pessoa p in pessoas)
                {
                    var b = await App.Api.GetUserAsync(p);

                    if (b)
                    {
                        await GetDataAsync(p);
                        PeopleAdded.Add(p);
                    }
                }
                foreach (var x in items.OrderBy(a => Guid.NewGuid()))
                {
                    Feed.Add(x);
                }
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                await NavigationService.NavigateAsync("ErrorConectionPage");
            }
            finally
            {
                IsBusy = false;
                IsLoadMore = true;
            }
        }


        public async Task GetDataAsync(Pessoa p)
        {
            try
            {
                var data = await App.Api.GetMediaAsync(p, 10);
            
                if (data != null)
                {
                    var x = data.Where(n => !items.Any(e => e.ImageLow.Equals(n.ImageLow)));
                    items.AddRange(x.OrderBy(a => Guid.NewGuid()));
                }
            }
            catch (Exception)
            {
                await NavigationService.NavigateAsync("ErrorConectionPage");
            }
        }


        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {     
                await GetMoreDataAsync();
            }
        }
    }
}
