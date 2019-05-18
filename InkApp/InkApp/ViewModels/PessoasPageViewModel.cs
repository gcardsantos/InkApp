using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InkApp.ViewModels
{
    public class PessoasPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IInstaApi api;

        private ObservableCollection<Pessoa> _pessoas;
        public ObservableCollection<Pessoa> Pessoas { get { return _pessoas; } set { SetProperty(ref _pessoas, value); } }

        private ObservableCollection<Pessoa> _visiblePeople;
        public ObservableCollection<Pessoa> PeopleVisible { get { return _visiblePeople; } set { SetProperty(ref _visiblePeople, value); } }
        private bool _visible;
        public bool Visible { get { return _visible; } set { SetProperty(ref _visible, value); } }

        private object _item;
        public object Item { get { return _item; } set { SetProperty(ref _item, value); } }

        public DelegateCommand ItemTappedCommand { get; private set; }

        public DelegateCommand<string> SearchBarCommand { get; private set; }

        public PessoasPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Black;
            _navigationService = navigationService;
            Pessoas = new ObservableCollection<Pessoa>();
            PeopleVisible = new ObservableCollection<Pessoa>();
            ItemTappedCommand = new DelegateCommand(NavigateToDetails);
            SearchBarCommand = new DelegateCommand<string>(FilterPeople);
        }

        private void FilterPeople(string obj)
        {
            if (!String.IsNullOrWhiteSpace(obj))
            {
                var p = Pessoas.Where(n => n.Name.Contains(obj));
                PeopleVisible = new ObservableCollection<Pessoa>(p);
            }
            else
            {
                PeopleVisible = Pessoas;
            }            
        }

        public async void NavigateToDetails()
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("pessoa", Item);
            navigationParams.Add("api", api);
            await _navigationService.NavigateAsync("DetailsPage", navigationParams, false);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.Back)
            {
                _navigationService.GoBackToRootAsync();
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.Count > 0)
            {
                var pessoas = parameters["pessoas"] as List<Pessoa>;
                api = parameters["api"] as IInstaApi;
                Title = parameters["city"] as string;

                Pessoas.Clear();
                PeopleVisible.Clear();
                foreach (var p in pessoas)
                {            
                    Pessoas.Add(p);
                    PeopleVisible.Add(p);
                }
            }
        }
    }
}
