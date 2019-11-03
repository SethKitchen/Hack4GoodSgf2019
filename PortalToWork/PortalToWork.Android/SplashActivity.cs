using Android.App;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PortalToWork.Droid
{
    [Activity(Label = "SplashActivity", Theme = "@style/Splash", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartActivity(typeof(MainActivity));

            Finish();
        }
    }
}