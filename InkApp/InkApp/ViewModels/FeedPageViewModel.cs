
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

        public ConcurrentQueue<KeyValuePair<Pessoa, List<InstagramItem>>> Imagens;

        //private ObservableCollection<InstagramItem> _toplist;
        //public ObservableCollection<InstagramItem> TopList { get { return _toplist; } set { SetProperty(ref _toplist, value); } }

        //public Dictionary<Pessoa, List<InstagramItem>> Imagens;

        private InstagramItem _lastItemTapped;
        public InstagramItem LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _isCollection;
        public bool CollectionVisible { get { return _isCollection; } set { SetProperty(ref _isCollection, value); } }


        public string FilterSelected { get; set; }

        public List<Pessoa> PeopleAdded { get; set; }

        static Random random = new Random();

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Imagens = new ConcurrentQueue<KeyValuePair<Pessoa, List<InstagramItem>>>();
            Feed = new ObservableCollection<InstagramItem>();
            //TopList = new ObservableCollection<InstagramItem>();
            allItems = new List<InstagramItem>();
            PeopleAdded = new List<Pessoa>();
            FilterCommand = new DelegateCommand<string>(FilterData);
            PhotoTappedCommand = new DelegateCommand(OpenPhotoAsync);
            LoadingCommand = new DelegateCommand(LoadMoreData);
            TopCommand = new DelegateCommand(CardOpenPhoto);
            FilterSelected = "All";
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

        public async Task GetData(List<Pessoa> pessoas)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(pessoas, async p =>
                {
                    await Task.Factory.StartNew(async () =>
                    {
                        var t1 = App.Api.GetUserAsync(p);

                        await Task.WhenAll(t1);

                        if (t1.Result)
                        {
                            var t2 = App.Api.GetMediaAsync(p, 49);

                            await Task.WhenAll(t2);
                            if (t2.Result.Count > 0)
                            {
                                var list = t2.Result;

                                Imagens.Enqueue(new KeyValuePair<Pessoa, List<InstagramItem>>(p, list));
                                //var pair = Imagens.First(n => n.Key.Username.Equals(p.Username));
                                //pair.Value.AddRange(list);
                            }

                        }
                    }).ConfigureAwait(false);
                });

                //erros.ForEach(n => Imagens.ToList().Remove);
            });
        }

        private async void StartValueAsync()
        {
            CollectionVisible = false;

            //IsBusy se repetiu pelo fato da solicitação de novos items do CollectionView setar para false antes de concluir a busca por novas imagens
            IsBusy = true;
            var pessoas = await repository.GetShufflePessoas();
            //pessoas.ForEach(n => Imagens.Enqueue(new KeyValuePair<Pessoa, List<InstagramItem>>(n, new List<InstagramItem>())));
            IsBusy = true;

            await Task.WhenAll(GetData(pessoas));
            await Task.Delay(1000);
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


        public void CircleQueue(KeyValuePair<Pessoa, List<InstagramItem>> pessoa, List<InstagramItem> itemsShuffle)
        {
            var k = pessoa.Value;

            List<InstagramItem> l;

            if (k.Count > 10)
                l = k.GetRange(0, 10);
            else
                l = k;

            l.ForEach(n =>
            {
                itemsShuffle.Add(n);
                allItems.Add(n);
                k.Remove(n);
            });    
        }

        public async Task CollectionLoadingMore()
        {
            try
            {
                IsBusy = true;
                var itemsShuffle = new List<InstagramItem>();
                for (int i = 0; i < 4; i++)
                {
                    KeyValuePair<Pessoa, List<InstagramItem>> saida = new KeyValuePair<Pessoa, List<InstagramItem>>();

                    if(Imagens.TryDequeue(out saida))
                    {
                        if (Imagens.Count > 2)
                        {
                            if (saida.Value.Count > 0)
                            {
                                CircleQueue(saida, itemsShuffle);
                            }
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                        
                        Imagens.Enqueue(saida);
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }

                if(itemsShuffle.Count > 0)
                {
                    itemsShuffle = itemsShuffle.OrderBy(a => Guid.NewGuid()).ToList();

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
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }
    }
}
