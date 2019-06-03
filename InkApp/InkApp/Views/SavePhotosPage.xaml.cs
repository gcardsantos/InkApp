using InkApp.Models;
using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class SavePhotosPage : ContentPage
    {
        public SavePhotosPage()
        {
            InitializeComponent();
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var imagem = e.CurrentSelection[0] as InstagramItem;
            await Navigation.PushModalAsync(new ImagePage(imagem));
        }
    }
}
