﻿
using DLToolkit.Forms.Controls;
using InkApp.Models;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InkApp.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private FlowObservableCollection<InstagramItem> _feed;
        //baixa resolução  / alta resolução
        public FlowObservableCollection<InstagramItem> Feed { get { return _feed; } set { SetProperty(ref _feed, value); } }

        //------regiao do profile info
        private string _sobre;
        public string Sobre { get { return _sobre; } set { SetProperty(ref _sobre, value); } }

        private string _whatsapp;
        public string Whatsapp { get { return _whatsapp; } set { SetProperty(ref _whatsapp, value); } }

        private string _instagram;
        public string Instagram { get { return _instagram; } set { SetProperty(ref _instagram, value); } }

        private string _image;
        public string ProfileImage { get { return _image; } set { SetProperty(ref _image, value); } }

        private string _local;
        public string Local { get { return _local; } set { SetProperty(ref _local, value); } }

        //-------------
        private object _photo;
        public object Image { get { return _photo; } set { SetProperty(ref _photo, value); } }

        private object _lastItemTapped;
        public object LastTappedItem { get { return _photo; } set { SetProperty(ref _lastItemTapped, value); } }

        private Pessoa _pessoa;

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _visible;
        public bool Visible { get { return _visible; } set { SetProperty(ref _visible, value); } }


        public DelegateCommand<object> PhotoTappedCommand { get; private set; }
        public DelegateCommand BtnIg { get; private set; }
        public DelegateCommand BtnLocal { get; private set; }
        public DelegateCommand BtnWhats { get; private set; }
        public DelegateCommand BtnFace { get; private set; }
        public DelegateCommand LoadingCommand { get; private set; }

        public DetailsPageViewModel(INavigationService navigationService):base(navigationService)
        {
            BtnWhats = new DelegateCommand(OpenWhatsApp);
            BtnFace = new DelegateCommand(OpenFace);
            BtnIg = new DelegateCommand(OpenInstagram);
            BtnLocal = new DelegateCommand(OpenLocal);
            PhotoTappedCommand = new DelegateCommand<object>(OpenPhotoAsync);
            LoadingCommand = new DelegateCommand(LoadMoreData);
            Feed = new FlowObservableCollection<InstagramItem>();
        }

        private async void OpenPhotoAsync(object obj)
        {
            var x = Feed.First(n => n.ImageLow.Equals((obj as InstagramItem).ImageLow));
            NavigationParameters np = new NavigationParameters();
            np.Add("photo", x);
            await NavigationService.NavigateAsync("ImagePage", np);
        }

        private void LoadMoreData()
        {
            _ = GetMediaAsync(_pessoa);
        }

        private void OpenLocal()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Device.OpenUri(
                      new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(Local))));
                    break;
                case Device.Android:
                    Device.OpenUri(
                      new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(Local))));
                    break;
            }
        }

        private void OpenFace()
        {
            Device.OpenUri(new Uri("https://m.facebook.com/" + _pessoa.Facebook));
        }

        private void OpenWhatsApp()
        {
            Device.OpenUri(new Uri("https://wa.me/"+ _pessoa.Numero));
        }

        private void OpenInstagram()
        {
            Device.OpenUri(new Uri("https://www.instagram.com/" + _pessoa.Username));
        }


        public async Task GetMediaAsync(Pessoa p)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                var data = await App.Api.GetMediaAsync(p, 49);

                if (data != null)
                {
                    //var lis = data.Where(n => Feed.Any(e => !e.ImageLow.Equals(n.ImageLow)));
                    Feed.AddRange(data);
                }
                IsBusy = false;
            }
            
        }

        public override void OnNavigatedFromAsync(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.Back)
            {
                App.Api.CloseUser();
                _navigationService.GoBackAsync();
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                _pessoa = parameters["pessoa"] as Pessoa;
                ProfileImage = _pessoa.Image;
                Local = _pessoa.Local;
                Sobre = _pessoa.Sobre;
                Title = _pessoa.Name;
                _ = GetMediaAsync(_pessoa);
                //_ = GetData(_pessoa);
            }            
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
