﻿using Prism;
using Prism.Ioc;
using InkApp.ViewModels;
using InkApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using InkApp.Data;
using System.IO;
using System;
using InkApp.Services;
using Plugin.Connectivity;
using InkApp.Models;

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

        public static Repository Repository;
        public static PhotoDatabase database;
        //public static IInstaApi Api;
        public static InstagramParser Api;
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        
        protected override async void OnInitialized()
        {
            CheckConnection();
            InitializeComponent();
            Api = new InstagramParser();
            Repository = new Repository();
            await NavigationService.NavigateAsync("/TopMasterDetailPage/CustomNavigationPage/HomePage");
            //await NavigationService.NavigateAsync(new System.Uri("/TopMasterDetailPage/CustomNavigationPage/HomePage", System.UriKind.Absolute));

        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<CustomNavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<PessoasPage, PessoasPageViewModel>();
            containerRegistry.RegisterForNavigation<DetailsPage, DetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<ImagePage, ImagePageViewModel>();
            containerRegistry.RegisterForNavigation<SavePhotosPage, SavePhotosPageViewModel>();
            containerRegistry.RegisterForNavigation<TopMasterDetailPage, TopMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<RequestPage, RequestPageViewModel>();
            containerRegistry.RegisterForNavigation<FeedPage, FeedPageViewModel>();
            containerRegistry.RegisterForNavigation<ErrorConectionPage, ErrorConectionPageViewModel>();
            containerRegistry.RegisterForNavigation<BasePeoplePage, BasePeoplePageViewModel>();
            containerRegistry.RegisterForNavigation<ReportPage, ReportPageViewModel>();
        }

        private async void CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
                await NavigationService.NavigateAsync("ErrorConectionPage");
            else
                return;
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
