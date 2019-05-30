using Prism;
using Prism.Ioc;
using InkApp.ViewModels;
using InkApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using InkApp.Data;
using System.IO;
using System;

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
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        
        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync("MainMasterDetailPage/NavigationPage/HomePage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<PessoasPage, PessoasPageViewModel>();
            containerRegistry.RegisterForNavigation<DetailsPage, DetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<ImagePage, ImagePageViewModel>();
            containerRegistry.RegisterForNavigation<AboutPage, AboutPageViewModel>();
            containerRegistry.RegisterForNavigation<MainMasterDetailPage, MainMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<SavePhotosPage, SavePhotosPageViewModel>();
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
