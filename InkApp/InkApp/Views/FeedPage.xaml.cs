using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class FeedPage : ContentPage
    {
        public FeedPage()
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

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            lbAll.TextColor = Color.Black;
            lbBlackGray.TextColor = Color.Black;
            lbBlackWork.TextColor = Color.Black;
            lbFineLine.TextColor = Color.Black;
            lbGeometric.TextColor = Color.Black;
            lbOldSchool.TextColor = Color.Black;
            lbTribal.TextColor = Color.Black;

            frAll.BackgroundColor = Color.White;
            frBlackGray.BackgroundColor = Color.White;
            frBlackWork.BackgroundColor = Color.White;
            frFineLine.BackgroundColor = Color.White;
            frGeometric.BackgroundColor = Color.White;
            frOldSchool.BackgroundColor = Color.White;
            frTribal.BackgroundColor = Color.White;

            var x = sender as Frame;
            var s = x.Content as Label;
            x.BackgroundColor = Color.Black;
            s.TextColor = Color.White;
        }

        private void TapGestureRecognizer_Tapped_1(object sender, System.EventArgs e)
        {

        }
    }
}
