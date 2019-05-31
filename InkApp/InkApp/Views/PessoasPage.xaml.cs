using Xamarin.Forms;
using InkApp.Models;
namespace InkApp.Views
{
    public partial class PessoasPage : ContentPage
    {
        ViewCell lastCell;
        public PessoasPage()
        {
            InitializeComponent();
        }

        private void ViewCell_Tapped(object sender, System.EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.LightGray;
                lastCell = viewCell;
            }
        }
    }
}
