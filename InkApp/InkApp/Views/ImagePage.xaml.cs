using InkApp.ViewModels;
using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class ImagePage : ContentPage
    {
        public ImagePage(string v)
        {
            InitializeComponent();
            ImageVar.Source = v;
        }
    }
}
