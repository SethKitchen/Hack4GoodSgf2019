using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using PortalToWork.CustomControls;
using PortalToWork.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NoUnderlinePicker), typeof(NoUnderlinePickerRenderer))]
namespace PortalToWork.iOS
{
    class NoUnderlinePickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as Picker;
            this.Control.BorderStyle = UITextBorderStyle.None;
            Control.TextAlignment = UITextAlignment.Right;
        }
    }
}