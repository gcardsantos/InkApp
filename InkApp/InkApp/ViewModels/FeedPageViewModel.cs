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

        private FlowObservableCollection<InstagramItem> _feed;
        public FlowObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        private object _lastItemTapped;
        public object LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }
        public List<Pessoa> PeopleAdded { get; set; }

        public FeedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            repository = new Repository();
            Feed = new FlowObservableCollection<InstagramItem>();
            PeopleAdded = new List<Pessoa>();
            LoadingCommand = new DelegateCommand(GetMoreDataAsync);
            PhotoTappedCommand = new DelegateCommand<object>(OpenPhotoAsync);
            StartValueAsync();
        }

        private void StartValueAsync()
        {
            Task.Run(() => { GetMoreDataAsync(); });
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
                var pessoas = Task.Run(async () => { return await repository.GetPessoas(); }).Result;
                pessoas.OrderBy(n => Guid.NewGuid());
                pessoas = pessoas.Where(n => !PeopleAdded.Exists(e => e.Username.Equals(n.Username))).ToList();
                pessoas.RemoveRange(pessoas.Count/2, pessoas.Count/2);
                
                foreach (Pessoa p in pessoas)
                {
                    await App.Api.GetUserAsync(p);
                    await GetDataAsync(p);
                }
                PeopleAdded.AddRange(pessoas);

            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }finally
            {
                IsBusy = false;
            }
        }

        public async Task GetDataAsync(Pessoa p)
        {
            var data = await App.Api.GetMediaAsync(p, 49);
            
            if (data != null)
            {
                data.RemoveAt(data.Count / 2);
                //var l = data.Where(n => Feed.Any(e => e.ImageLow.Equals(n.ImageLow)));
                Feed.AddRange(data);
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
