using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using PortalToWork.Droid;
using Xamarin.Forms;
using PortalToWork.CustomControls;
using Xamarin.Forms.Platform.Android;
using Android.Content;

[assembly: ExportRenderer(typeof(NoUnderlineEntry), typeof(NoUnderlineEntryRenderer))]
namespace PortalToWork.Droid
{
    public class NoUnderlineEntryRenderer : EntryRenderer
    {
        public NoUnderlineEntryRenderer(Context context):base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackground(gd);
                this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                //this line sets the bordercolor
                gd.SetColor(2529787);

                Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.White));
            }
        }
    }
}