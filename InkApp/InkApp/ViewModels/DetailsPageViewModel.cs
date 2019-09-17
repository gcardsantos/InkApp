

using InkApp.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InkApp.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        private List<InstagramItem> instagramItems;

        public InstagramItem Item;

        //baixa resolução  / alta resolução
        private ObservableCollection<InstagramItem> _feed;
        public ObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        //------regiao do profile info
        private string _sobre;
        public string Sobre { get { return _sobre; } set { SetProperty(ref _sobre, value); } }

        private string _whatsapp;
        public string Whatsapp { get { return _whatsapp; } set { SetProperty(ref _whatsapp, value); } }

        private string _instagram;
        public string Instagram { get { return _instagram; } set { SetProperty(ref _instagram, value); } }

        private string _image;
        public string ProfileImage { get { return _image; } set { SetProperty(ref _image, value); } }

        private string _email;
        public string Email { get { return _email; } set { SetProperty(ref _email, value); } }

        private string _local;
        public string Local { get { return _local; } set { SetProperty(ref _local, value); } }

        private string _nome;
        public string Nome { get { return _nome; } set { SetProperty(ref _nome, value); } }

        //-------------
        private object _photo;
        public object Image { get { return _photo; } set { SetProperty(ref _photo, value); } }

        private InstagramItem _lastItemTapped;
        public InstagramItem LastTappedItem { get { return _lastItemTapped; } set { SetProperty(ref _lastItemTapped, value); } }

        private Pessoa _pessoa;

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _loadMore;
        public bool IsLoadMore { get { return _loadMore; } set { SetProperty(ref _loadMore, value); } }

        private bool _visible;
        public bool Visible { get { return _visible; } set { SetProperty(ref _visible, value); } }

        private bool _isCollection;
        public bool CollectionVisible { get { return _isCollection; } set { SetProperty(ref _isCollection, value); } }

        public DelegateCommand PhotoTappedCommand { get; private set; }
        public DelegateCommand BtnIg { get; private set; }
        public DelegateCommand BtnLocal { get; private set; }

        public DelegateCommand BtnWhats { get; private set; }
        public DelegateCommand BtnFace { get; private set; }
        public DelegateCommand BtnEmail { get; private set; }
        public DelegateCommand LoadingCommand { get; private set; }
        public DelegateCommand ReportCommand { get; private set; }

        private readonly IPageDialogService PageDialogService;

        public DetailsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) :base(navigationService)
        {
            PageDialogService = pageDialogService;
            BtnWhats = new DelegateCommand(OpenWhatsAppAsync);
            BtnFace = new DelegateCommand(OpenFace);
            BtnEmail = new DelegateCommand(OpenEmail);
            BtnIg = new DelegateCommand(OpenInstagram);
            BtnLocal = new DelegateCommand(OpenLocal);
            PhotoTappedCommand = new DelegateCommand(OpenPhotoAsync);
            ReportCommand = new DelegateCommand(OpenReportPage);
            LoadingCommand = new DelegateCommand(LoadMoreDataAsync);
            Feed = new ObservableCollection<InstagramItem>();
            instagramItems = new List<InstagramItem>();
        }

        private async void OpenEmail()
        {
            if (!_pessoa.Email.Equals("nao"))
                await Launcher.TryOpenAsync(new Uri("mailto:" + Email));
            else
                await PageDialogService.DisplayAlertAsync("Email", "Tatuador não possui contato via email.", "Ok");
        }

        private async void OpenReportPage()
        {
            NavigationParameters np = new NavigationParameters
            {
                { "Pessoa", _pessoa }
            };
            await NavigationService.NavigateAsync("ReportPage", np);
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

        private async void LoadMoreDataAsync()
        {
            await GetMediaAsync(_pessoa);
        }

        private async void OpenLocal()
        {
            string rua = _pessoa.Local + ", " + _pessoa.Cidade + " - " + _pessoa.Estado;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    await Launcher.TryOpenAsync(
                      new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(rua))));
                    break;
                case Device.Android:
                    await Launcher.TryOpenAsync(
                      new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(rua))));
                    break;
            }
        }

        private async void OpenFace()
        {
            if (!_pessoa.Facebook.Equals("nao"))
                await Launcher.TryOpenAsync(new Uri("fb://" + _pessoa.Facebook));
            else
                await PageDialogService.DisplayAlertAsync("Facebook", "Tatuador não possui facebook.", "Ok");
        }

        private async void OpenWhatsAppAsync()
        {
            if(!_pessoa.Numero.Equals("nao"))
                await Launcher.TryOpenAsync(new Uri("https://wa.me/"+ _pessoa.Numero));
            else
                await PageDialogService.DisplayAlertAsync("WhatsApp", "Tatuador não possui whatsapp.", "Ok");
        }

        private async void OpenInstagram()
        {
            await Launcher.TryOpenAsync(new Uri("https://www.instagram.com/" + _pessoa.Username));
        }


        public async Task GetMediaAsync(Pessoa p)
        {
            try
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    IsLoadMore = false;

                    if (!p.NextPage && instagramItems.Count <= 0)
                    {
                        IsBusy = false;
                        IsLoadMore = false;
                        return;
                    }
                    

                    if(instagramItems.Count <= 0 && p.NextPage)
                    {
                        instagramItems = new List<InstagramItem>(await App.Api.GetAllMediaAsync(p));
                    }

                    if (instagramItems.Count > 30)
                    {
                        foreach(var x in instagramItems.GetRange(0, 30))
                        {
                            Feed.Add(x);
                        }
                        instagramItems.RemoveRange(0, 30);
                    }
                    else
                    {
                        foreach (var x in instagramItems)
                        {
                            Feed.Add(x);
                        }
                        instagramItems.Clear();
                    }
                
                    IsBusy = false;
                    IsLoadMore = true;
                }
            }
            catch (Exception)
            {
                await NavigationService.NavigateAsync("ErrorConectionPage");
            }
            
        }

        public override void OnNavigatedFromAsync(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.Back)
            {
                App.Api.CloseUser(_pessoa);
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.New)
            {
                _pessoa = parameters["pessoa"] as Pessoa;
                CollectionVisible = false;
                _pessoa.NextPage = true;
                Nome = _pessoa.Name;
                ProfileImage = _pessoa.Image;
                Local = _pessoa.Local;
                Email = _pessoa.Email;
                Sobre = _pessoa.Sobre;
                await GetMediaAsync(_pessoa);
                CollectionVisible = true;
            }            
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
