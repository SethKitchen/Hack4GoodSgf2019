using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using PortalToWork.CustomControls;
using PortalToWork.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoUnderlinePicker), typeof(NoUnderlinePickerRenderer))]
namespace PortalToWork.Droid
{
    class NoUnderlinePickerRenderer : PickerRenderer
    {
        public NoUnderlinePickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetStroke(0, Android.Graphics.Color.Transparent);
                Control.SetBackground(gd);
                Control.Gravity = GravityFlags.Right;
            }
        }
    }
}