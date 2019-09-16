
using InkApp.Models;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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

        public Queue<KeyValuePair<Pessoa, List<InstagramItem>>> Imagens;
        //public Dictionary<Pessoa, List<InstagramItem>> Imagens;

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

        static Random random = new Random();

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Imagens = new Queue<KeyValuePair<Pessoa, List<InstagramItem>>>();
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
            pessoas.ForEach(n => Imagens.Enqueue(new KeyValuePair<Pessoa, List<InstagramItem>>(n, new List<InstagramItem>())));
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

                    var pair = Imagens.First(n => n.Key.Username.Equals(p.Username));
                    pair.Value.AddRange(list);
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

        //Not using
        public async void GetMoreDataAsync()
        {
            IsBusy = true;
            foreach(var p in Imagens) {

                await Task.Run(async () =>
                {
                    if (!String.IsNullOrWhiteSpace(p.Key.LastToken))
                    {
                        var list = await App.Api.GetMediaAsync(p.Key, 49);
                        p.Value.AddRange(list);
                    }
                });
            }             
            IsBusy = false;
        }

        public async Task CollectionLoadingMore()
        {
            try
            {
                IsBusy = true;
                IsLoadMore = false;

                List<InstagramItem> items = new List<InstagramItem>();

                int count = allItems.Count;
                bool localAttempt = false;

                //'done' valida se ainda há dados para se buscar
                //'localAttempt' tenta fazer uma nova requisição de dados para continuar o feed

                while (count == Feed.Count && Imagens.Count != 0)
                {
                    for (int i = 0; i < 4 && i < Imagens.Count; i++)
                    {
                        var pessoa = Imagens.Dequeue();
                        var k = pessoa.Value;

                        List<InstagramItem> l;

                        if (k.Count > 5)
                            l = k.GetRange(0, 5);
                        else
                            l = k;

                        l.ForEach(n =>
                        {
                            items.Add(n);
                            allItems.Add(n);
                            k.Remove(n);
                        });

                        if (k.Count > 0)
                            Imagens.Enqueue(pessoa);
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

                    //Thread t = new Thread(GetMoreDataAsync);
                    //t.Start();

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
