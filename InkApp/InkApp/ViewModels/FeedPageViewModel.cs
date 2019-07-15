using DLToolkit.Forms.Controls;
using InkApp.Models;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkApp.ViewModels
{
    public class FeedPageViewModel : ViewModelBase
    {
        private Repository repository;
        public DelegateCommand<string> FilterCommand { get; private set; }
        public DelegateCommand<object> PhotoTappedCommand { get; private set; }

        public DelegateCommand LoadingCommand { get; set; }
        private List<InstagramItem> items;

        private FlowObservableCollection<InstagramItem> _feed;
        public FlowObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }


        private FlowObservableCollection<Estilo> _estilos;
        public FlowObservableCollection<Estilo> Estilos { get { return _estilos; } set { SetProperty(ref _estilos, value); } }

        private object _lastItemTapped;
        public object LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _loadMore;
        public bool IsLoadMore { get { return _loadMore; } set { SetProperty(ref _loadMore, value); } }
        public List<Pessoa> PeopleAdded { get; set; }

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Feed = new FlowObservableCollection<InstagramItem>();
            Estilos = new FlowObservableCollection<Estilo>();
            items = new List<InstagramItem>();
            PeopleAdded = new List<Pessoa>();
            FilterCommand = new DelegateCommand<string>(FilterData);
            PhotoTappedCommand = new DelegateCommand<object>(OpenPhotoAsync);
            LoadingCommand = new DelegateCommand(LoadMoreData);
            StartValueAsync();
        }

        private async void LoadMoreData()
        {
            await GetMoreDataAsync();
        }

        private void FilterData(string obj)
        {
            Feed.Clear();
            if (obj != "All")
                Feed.AddRange(items.Where(n => n.Tags.Contains(obj)));
            else
                Feed.AddRange(items);
        }

        private async void StartValueAsync()
        {
            await GetMoreDataAsync();
        }

        private async void OpenPhotoAsync(object obj)
        {
            var x = Feed.First(n => n.ImageLow.Equals((obj as InstagramItem).ImageLow));
            NavigationParameters np = new NavigationParameters();
            np.Add("photo", x);
            await NavigationService.NavigateAsync("ImagePage", np);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
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
                Feed.AddRange(items.OrderBy(a => Guid.NewGuid()));
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                IsBusy = false;
                IsLoadMore = true;
            }
        }


        public async Task GetDataAsync(Pessoa p)
        {
            var data = await App.Api.GetMediaAsync(p, 10);
            
            if (data != null)
            {
                var x = data.Where(n => !items.Any(e => e.ImageLow.Equals(n.ImageLow)));
                items.AddRange(x.OrderBy(a => Guid.NewGuid()));
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
