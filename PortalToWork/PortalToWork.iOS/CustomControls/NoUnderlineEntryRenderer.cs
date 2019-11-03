using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomRenderer.iOS;
using Foundation;
using PortalToWork.CustomControls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NoUnderlineEntry), typeof(NoUnderlineEntryRenderer))]
namespace CustomRenderer.iOS
{
    public class NoUnderlineEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {

                //Control.BorderStyle = UITextBorderStyle.None;
                Control.TintColor = UIColor.FromRGB(38, 153, 251);
                Control.Layer.CornerRadius = 10;
                Control.TextColor = UIColor.White;

            }
        }
    }
}