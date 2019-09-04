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
            lbAll.TextColor = Color.Gray;
            lbBlackGray.TextColor = Color.Gray;
            lbBlackWork.TextColor = Color.Gray;
            lbFineLine.TextColor = Color.Gray;
            lbGeometric.TextColor = Color.Gray;
            lbOldSchool.TextColor = Color.Gray;
            lbTribal.TextColor = Color.Gray;

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
