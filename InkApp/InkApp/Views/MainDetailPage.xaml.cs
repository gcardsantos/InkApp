using Prism.Navigation;
using Xamarin.Forms;

namespace InkApp.Views
{
    public partial class MainDetailPage : MasterDetailPage, IMasterDetailPageOptions
    {
        public MainDetailPage()
        {
            InitializeComponent();
        }

        public bool IsPresentedAfterNavigation
        {
            get { return false; }
        }
    }
}