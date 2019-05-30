using System;
using InkApp.Data;
using InkApp.Models;
using InkApp.ViewModels;
using Prism.Commands;
using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class ImagePage : ContentPage
    {
        public InstagramItem item;
        
        public ImagePage(InstagramItem v)
        {
            InitializeComponent();
            item = v;
            ImageVar.Source = v.ImageHigh;
        }
        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            App.Database.SaveItemAsync(item as InstagramItem);
        }

    }
}
