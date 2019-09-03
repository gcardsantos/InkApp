using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class BasePeoplePage : ContentPage
    {
        public BasePeoplePage()
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
