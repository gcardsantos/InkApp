using Prism;
using Prism.Ioc;
using InkApp.ViewModels;
using InkApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using InkApp.Data;
using System.IO;
using System;
using DLToolkit.Forms.Controls;
using InkApp.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace InkApp
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */

        public static double ScreenWidth;
        public static double ScreenHeight;
        public static PhotoDatabase database;
        //public static IInstaApi Api;
        public static InstagramParser Api;
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        
        protected override async void OnInitialized()
        {
            AdMaiora.RealXaml.Client.AppManager.Init(this);
            InitializeComponent();
            FlowListView.Init();
            Api = new InstagramParser();
            await NavigationService.NavigateAsync("TopMasterDetailPage/NavigationPage/HomePage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<PessoasPage, PessoasPageViewModel>();
            containerRegistry.RegisterForNavigation<DetailsPage, DetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<ImagePage, ImagePageViewModel>();
            containerRegistry.RegisterForNavigation<AboutPage, AboutPageViewModel>();
            containerRegistry.RegisterForNavigation<SavePhotosPage, SavePhotosPageViewModel>();
            containerRegistry.RegisterForNavigation<TopMasterDetailPage, TopMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<RequestPage, RequestPageViewModel>();
            containerRegistry.RegisterForNavigation<FeedPage, FeedPageViewModel>();
        }

        public static PhotoDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new PhotoDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhotoSQLite.db3"));
                }
                return database;
            }
        }
    }
}
