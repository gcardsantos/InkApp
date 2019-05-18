
using InkApp.Models;
using InstagramApiSharp;
using InstagramApiSharp.API;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace InkApp.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IInstaApi api;
        private ObservableCollection<InstagramItem> _feed;

        //baixa resolução  / alta resolução

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

        private string _local;
        public string Local { get { return _local; } set { SetProperty(ref _local, value); } }

        //-------------
        private object _photo;
        public object Image { get { return _photo; } set { SetProperty(ref _photo, value); } }

        private Pessoa _pessoa;

        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        private bool _visible;
        public bool Visible { get { return _visible; } set { SetProperty(ref _visible, value); } }

        public DelegateCommand<object> PhotoTappedCommand { get; private set; }
        public DelegateCommand BtnIg { get; private set; }
        public DelegateCommand BtnWhats { get; private set; }
        public DelegateCommand BtnFace { get; private set; }

        public DetailsPageViewModel(INavigationService navigationService):base(navigationService)
        {
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Black;
            
            Feed = new ObservableCollection<InstagramItem>();
            BtnWhats = new DelegateCommand(OpenWhatsApp);
            BtnFace = new DelegateCommand(OpenFace);
            BtnIg = new DelegateCommand(OpenInstagram);
        }

        private void OpenFace()
        {
            Device.OpenUri(new Uri("https://www.facebook.com/" + _pessoa.Facebook));
        }

        private void OpenWhatsApp()
        {
            Device.OpenUri(new Uri("https://wa.me/"+ _pessoa.Numero));
        }

        private void OpenInstagram()
        {
            Device.OpenUri(new Uri("https://www.instagram.com/" + _pessoa.Username));
        }


        public async void GetData(Pessoa p)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                Exception Error = null;
                try
                {
                    var collection = await api.UserProcessor.GetUserMediaAsync(p.Username, PaginationParameters.MaxPagesToLoad(2));
                    if (collection.Succeeded)
                    {
                        Feed.Clear();

                        foreach (var item in collection.Value)
                        {
                            if(item.Images.Count > 0 && item.Videos.Count == 0)
                            {
                                Feed.Add(new InstagramItem() { ImageLow = item.Images[1].Uri, ImageHigh = item.Images[0].Uri });
                            }                            
                        }
                    }
                    Visible = false;
                }
                catch(Exception ex)
                {
                    Error = ex;
                    Visible = true;
                }
                finally
                {
                    if(Error == null)
                        IsBusy = false;
                }
            }
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.Back)
            {
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
                api = parameters["api"] as IInstaApi;
                Title = _pessoa.Name;
                GetData(_pessoa);
            }            
        }


        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
