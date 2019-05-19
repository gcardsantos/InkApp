using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using InkApp.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.API.Builder;
using Xamarin.Forms;

namespace InkApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IInstaApi api;
        public string AdUnitId { get; set; } = "ca-app-pub-3940256099942544/6300978111";
        //bool para exibir o picker
        private bool _picker;
        public bool PickerVisible { get { return _picker; } set { SetProperty(ref _picker, value); } }
        //bool para exibir progress bar
        private bool _busy;
        public bool IsBusy { get { return _busy; } set { SetProperty(ref _busy, value); } }

        //bool para exibir erros
        private bool _visible;
        public bool Visible { get { return _visible; } set { SetProperty(ref _visible, value); } }

        //bool para efetuar login
        private bool _login;
        public bool Login { get { return _login; } set { SetProperty(ref _login, value); } }

        //bool para exibir botao sincronizar
        private bool _btnEnabled;
        public bool BtnEnabled { get { return _btnEnabled; } set { SetProperty(ref _btnEnabled, value); } }

        private object _city;
        public object City { get { return _city; } set { SetProperty(ref _city, value); } }



        public DelegateCommand NavigateToPessoasPageCommand { get; private set; }
        public DelegateCommand NavigateToAboutPageCommand { get; private set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            BtnEnabled = false;
            Login = true;
            PickerVisible = false;
            _navigationService = navigationService;
            LogInstaAsync();
            NavigateToPessoasPageCommand = new DelegateCommand(NavigateToPessoasPage);
            NavigateToAboutPageCommand = new DelegateCommand(NavitageToAboutPageAsync);
        }

        private async void NavitageToAboutPageAsync()
        {
            await _navigationService.NavigateAsync("PessoasPage");
        }

        private async Task LogInstaAsync()
        {
            IsBusy = true;

            var userSession = new UserSessionData
            {
                UserName = "tatuapp",
                Password = "inkapp"
            };

            try
            {
                api = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .SetRequestDelay(RequestDelay.FromSeconds(1, 3))
                .Build();
                await api.LoginAsync();
                BtnEnabled = true;
                Visible = false;
            }
            catch (Exception)
            {
                Visible = true;
            }
            finally
            {
                IsBusy = false;
                Login = false;
                PickerVisible = true;
            }
        }
        private async void NavigateToPessoasPage()
        {
            if (City == null)
                return;

            var navigationParams = new NavigationParameters();
            if (!IsBusy)
            {
                Exception Error = null;
                try
                {
                    IsBusy = true;
                    BtnEnabled = false;                   
                    
                    var Repository = new Repository();
                    var city = City.ToString().Replace(" / ", "/").Split('/');
                    var pessoas = await Repository.GetPessoas(city[1], city[0]);
                    await GetPhotoAsync(pessoas);
                    navigationParams.Add("pessoas", pessoas);
                    navigationParams.Add("city", city[1]);
                    navigationParams.Add("estado", city[0]);
                    navigationParams.Add("api", api);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Visible = true;
                }
                finally
                {
                    if (Error == null)
                    {
                        IsBusy = false;
                        Visible = false;
                        BtnEnabled = true;
                        await _navigationService.NavigateAsync("PessoasPage", navigationParams, false);
                    }
                }
            }

        }

        public async Task GetPhotoAsync(List<Pessoa> pessoa)
        {
            foreach (var p in pessoa)
            {
                try
                {
                    if (api.IsUserAuthenticated)
                    {
                        var info = await api.UserProcessor.GetUserAsync(p.Username);
                        p.Image = info.Value.ProfilePicture;
                    }
                }
                catch (Exception)
                {

                }

            }
        }
    }
}
