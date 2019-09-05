
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

        private List<InstagramItem> allItems;

        private ObservableCollection<InstagramItem> _feed;
        public ObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        public object mutex;

        private ObservableCollection<InstagramItem> _toplist;
        public ObservableCollection<InstagramItem> TopList { get { return _toplist; } set { SetProperty(ref _toplist, value); } }

        public Dictionary<Pessoa, List<InstagramItem>> Imagens;

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

        public bool done;

        public string FilterSelected { get; set; }

        public List<Pessoa> PeopleAdded { get; set; }

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Imagens = new Dictionary<Pessoa, List<InstagramItem>>();
            Feed = new ObservableCollection<InstagramItem>();
            //TopList = new ObservableCollection<InstagramItem>();
            allItems = new List<InstagramItem>();
            PeopleAdded = new List<Pessoa>();
            FilterCommand = new DelegateCommand<string>(FilterData);
            PhotoTappedCommand = new DelegateCommand(OpenPhotoAsync);
            LoadingCommand = new DelegateCommand(LoadMoreData);
            TopCommand = new DelegateCommand(CardOpenPhoto);
            FilterSelected = "All";
            done = false;
            StartValueAsync();
        }

        private void CardOpenPhoto()
        {
            //NavigationParameters np = new NavigationParameters
            //{
            //    { "photo", TopList[Position] }
            //};

            //await NavigationService.NavigateAsync("ImagePage", np, false);
        }

        private async void LoadMoreData()
        {
            await CollectionLoadingMore();
        }

        private void FilterData(string obj)
        {
            FilterSelected = obj;
            Feed.Clear();
            if (obj != "All")
            {
                foreach(var x in allItems.Where(n => n.Tags.Contains(FilterSelected)))
                {
                    Feed.Add(x);
                }
            }
            else
            {
                foreach (var x in allItems)
                {
                    Feed.Add(x);
                }
            }
        }

        private async void StartValueAsync()
        {
            CollectionVisible = false;

            //IsBusy se repetiu pelo fato da solicitação de novos items do CollectionView setar para false antes de concluir a busca por novas imagens
            IsBusy = true;
            var pessoas = await repository.GetShufflePessoas();
            pessoas.ForEach(n => Imagens.Add(n, new List<InstagramItem>()));
            IsBusy = true;
            foreach (var p in pessoas)
            {
                await Task.Run(async () =>
                {
                    var t1 = App.Api.GetUserAsync(p);

                    await Task.WhenAll(t1);

                    var t2 = App.Api.GetMediaAsync(p, 49);

                    await Task.WhenAll(t2);

                    var list = t2.Result;

                    Imagens[p].AddRange(list);
                });
                
            }

            await CollectionLoadingMore();
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

        public void GetMoreDataAsync()
        {
            IsBusy = true;
            Task.Run(
                () => Parallel.ForEach(Imagens, async p =>
                {
                    if (!String.IsNullOrWhiteSpace(p.Key.LastToken))
                    {
                        var list = await App.Api.GetMediaAsync(p.Key, 49);
                        p.Value.AddRange(list);
                    }
                })
            ); 
            IsBusy = false;
        }

        public async Task CollectionLoadingMore()
        {
            try
            {
                IsBusy = true;
                IsLoadMore = false;

                Random r = new Random();
                List<InstagramItem> items = new List<InstagramItem>();

                int count = allItems.Count;
                bool localAttempt = false;

                //'done' valida se ainda há dados para se buscar
                //'localAttempt' tenta fazer uma nova requisição de dados para continuar o feed

                while (count == Feed.Count)
                {
                    for (int i = 0; i < 4 && i < Imagens.Count - 1; i++)
                    {
                        var pessoa = Imagens.ToList()[r.Next(Imagens.Count - 1)];
                        var k = pessoa.Value;

                        List<InstagramItem> l;

                        if (k.Count > 5)
                            l = k.GetRange(0, 5);
                        else
                            l = k;

                        foreach(InstagramItem item in l)
                        {
                            items.Add(item);
                            allItems.Add(item);
                            k.Remove(item);
                        }

                        if (k.Count == 0)
                            Imagens.Remove(pessoa.Key);
                    }

                    if (localAttempt && count == allItems.Count)
                    {
                        //done = true;
                        break;
                    }

                    var itemsShuffle = items.OrderBy(a => Guid.NewGuid());

                    if (FilterSelected.Equals("All"))
                    {
                        foreach (var x in itemsShuffle)
                        {
                            Feed.Add(x);
                        }
                    }
                    else
                    {
                        foreach (var x in itemsShuffle)
                        {
                            if (x.Tags.ToLower().Contains(FilterSelected.ToLower()))
                            {
                                Feed.Add(x);
                            }
                        }
                    }

                    await Task.Run(() => GetMoreDataAsync());

                    if (count == allItems.Count)
                    {    
                        localAttempt = true;
                    }                       
                            
                }
            }
            catch (InvalidOperationException r)
            {
                string s = r.Message;
            }
            catch (Exception ex)
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


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }
    }
}
