using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Prism;
using Prism.Ioc;


namespace InkApp.Droid
{
    [Activity(Label = "InkApp", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public partial class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            //this.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);
            //InitFontScale();
            Window.SetStatusBarColor(Color.Black);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(false);
            LoadApplication(new App(new AndroidInitializer()));
        }

        //private void InitFontScale()
        //{
        //    Configuration configuration = Resources.Configuration;
        //    configuration.FontScale = (float)1.3;
        //    0.85 small, 1 standard, 1.15 big，1.3 more bigger ，1.45 supper big
        //    DisplayMetrics metrics = new DisplayMetrics();
        //    WindowManager.DefaultDisplay.GetMetrics(metrics);
        //    metrics.ScaledDensity = configuration.FontScale * metrics.Density;
        //    BaseContext.Resources.UpdateConfiguration(configuration, metrics);
        //}
    }


    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

