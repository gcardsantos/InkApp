using InkApp.Models;
using System.Collections;
using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var imagem = e.CurrentSelection[0] as InstagramItem;
            await Navigation.PushModalAsync(new ImagePage(imagem));
        }

        private void FlowListView_FlowItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = flowListView.ItemsSource as IList;

            if (items == null
                || items.Count == 0)
                return;

            if (e.Item != items[items.Count - 1])
                return;

            System.Diagnostics.Debug.WriteLine("Add more");
        }
    }
}
