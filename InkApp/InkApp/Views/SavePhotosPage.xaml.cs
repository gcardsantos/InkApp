﻿using InkApp.Models;
using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class SavePhotosPage : ContentPage
    {
        public SavePhotosPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (dataCollection != null)
            {
                dataCollection.SelectedItem = null;
            }
        }
    }
}
