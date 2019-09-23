using Prism.Commands;
using Prism.Navigation;
using System;
using InkApp.Models;
using Xamarin.Forms;
using InkApp.Views;
using Prism;
using System.Collections.ObjectModel;

namespace InkApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase, IActiveAware
    {
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


        private ObservableCollection<string> _cidades;
        public ObservableCollection<string> Cidades { get { return _cidades; } set { SetProperty(ref _cidades, value); } }

        private object _city;

        public event EventHandler IsActiveChanged;

        public object City { get { return _city; } set { SetProperty(ref _city, value); } }


        public DelegateCommand NavigateToRequestPageCommand { get; private set; }
        public DelegateCommand NavigateToPessoasPageCommand { get; private set; }
        public DelegateCommand NavigateToAboutPageCommand { get; private set; }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }
        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
        public HomePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            NavigateToPessoasPageCommand = new DelegateCommand(NavigateToPessoasPage);
            NavigateToRequestPageCommand = new DelegateCommand(NavigateToRequestPage);
            NavigateToAboutPageCommand = new DelegateCommand(NavitageToAboutPageAsync);
            Cidades = new ObservableCollection<string>();
            StartValues();
        }

        public async void StartValues()
        {
            int attempt = 0;
            IsBusy = true;
            while(attempt != 1)
            {
                try
                {
                    var x = await (new Repository().GetPessoas());

                    foreach(var i in x)
                    {
                        string s = i.Estado + " / " + i.Cidade;
                        if (!Cidades.Contains(s))
                            Cidades.Add(s);
                    }
                    
                    attempt = 1;
                }
                catch (Exception e)
                {
                    string s = e.Message;
                }
            }

            IsBusy = false;
            PickerVisible = true;
            BtnEnabled = true;
            Login = true;
        }

        private async void NavigateToRequestPage()
        {
            await NavigationService.NavigateAsync("RequestPage");
        }

        private async void NavitageToAboutPageAsync()
        {
            await NavigationService.NavigateAsync(nameof(TopMasterDetailPage)+ "/" + nameof(NavigationPage) + "/" + nameof(PessoasPage));
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
                    
                    var city = City.ToString().Replace(" / ", "/").Split('/');
                    var pessoas = await App.Repository.GetPessoas(city[1], city[0]);
                    await App.Api.GetUserAsync(pessoas);
                    navigationParams.Add("pessoas", pessoas);
                    navigationParams.Add("city", city[1]);
                    navigationParams.Add("estado", city[0]);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Visible = true;
                    IsBusy = false;
                }
                finally
                {
                    if (Error == null)
                    {
                        IsBusy = false;
                        Visible = false;
                        BtnEnabled = true;
                        await NavigationService.NavigateAsync("PessoasPage", navigationParams, false);
                    }
                    else
                    {
                        await NavigationService.NavigateAsync("ErrorConectionPage", navigationParams, false);
                    }
                }
            }

        }


        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

    }
}
