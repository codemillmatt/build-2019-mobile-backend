using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace TailwindTraders.Mobile.Droid
{
    [Activity(
        Label = "@string/appName",
        Icon = "@mipmap/icon",
        Theme = "@style/SplashTheme",
        MainLauncher = true,
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //var intent = new Intent(this, typeof(MainActivity));
            //StartActivity(intent);

            var intent = new Intent(this, typeof(MainActivity));
            if (Intent.Extras != null)
                intent.PutExtras(Intent.Extras); // copy push info from splash to main
            StartActivity(intent);
        }
    }
}
