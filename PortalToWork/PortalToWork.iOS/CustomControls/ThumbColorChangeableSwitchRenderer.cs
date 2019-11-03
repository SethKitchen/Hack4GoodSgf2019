using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using PortalToWork.CustomControls;
using PortalToWork.iOS.CustomControls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ThumbColorChangeableSwitch), typeof(ThumbColorChangeableSwitchRenderer))]
namespace PortalToWork.iOS.CustomControls
{
    class ThumbColorChangeableSwitchRenderer : SwitchRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null) return;

            ThumbColorChangeableSwitch s = Element as ThumbColorChangeableSwitch;

            UISwitch sw = new UISwitch();
            sw.ThumbTintColor = s.SwitchThumbColor.ToUIColor();
            sw.OnTintColor = s.SwitchOnColor.ToUIColor();

            SetNativeControl(sw);
        }
    }
  
}