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
        public DelegateCommand LoadingCommand { get; private set; }
        public DelegateCommand<object> PhotoTappedCommand { get; private set; }

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
            LoadingCommand = new DelegateCommand(GetMoreDataAsync);
            PhotoTappedCommand = new DelegateCommand<object>(OpenPhotoAsync);
            StartValueAsync();
        }

        private void StartValueAsync()
        {
            Estilos.Add(new Estilo() { Name = "BlackWork1" });
            Estilos.Add(new Estilo() { Name = "BlackWork2" });
            Estilos.Add(new Estilo() { Name = "BlackWork3" });
            Estilos.Add(new Estilo() { Name = "BlackWork4" });
            Estilos.Add(new Estilo() { Name = "BlackWork5" });
            Estilos.Add(new Estilo() { Name = "BlackWork6" });
            GetMoreDataAsync();
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

        public async void GetMoreDataAsync()
        {
            try
            {
                IsBusy = true;
                IsLoadMore = false;
                var pessoas =  await repository.GetPessoas();
                pessoas.OrderBy(n => Guid.NewGuid());
                pessoas = pessoas.Where(n => !PeopleAdded.Exists(e => e.Username.Equals(n.Username))).ToList();
                pessoas.RemoveRange(pessoas.Count/2, pessoas.Count/2);
                
                foreach (Pessoa p in pessoas)
                {
                    var b = await App.Api.GetUserAsync(p);

                    if(b)
                        await GetDataAsync(p);
                }
                PeopleAdded.AddRange(pessoas);
                Feed.AddRange(items.OrderBy(a => Guid.NewGuid()));
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }finally
            {
                IsBusy = false;
                IsLoadMore = true;
            }
        }

        public async Task GetDataAsync(Pessoa p)
        {
            var data = await App.Api.GetMediaAsync(p);
            
            if (data != null)
            {
                var x = data.FindAll(n => !items.Exists(e => e.ImageLow == n.ImageLow));
                items.AddRange(x);
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {     
                GetMoreDataAsync();
                //_ = GetData(_pessoa);
            }
        }
    }
}
