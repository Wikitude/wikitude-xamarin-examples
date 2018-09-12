// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace XamarinExampleApp.iOS
{
    [Register ("AddURLViewController")]
    partial class AddURLViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch GeoARFeatureSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ImageTrackingFeatureSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch InstantTrackingFeatureSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ObjectTrackingFeatureSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField titleTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField URLTextField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GeoARFeatureSwitch != null) {
                GeoARFeatureSwitch.Dispose ();
                GeoARFeatureSwitch = null;
            }

            if (ImageTrackingFeatureSwitch != null) {
                ImageTrackingFeatureSwitch.Dispose ();
                ImageTrackingFeatureSwitch = null;
            }

            if (InstantTrackingFeatureSwitch != null) {
                InstantTrackingFeatureSwitch.Dispose ();
                InstantTrackingFeatureSwitch = null;
            }

            if (ObjectTrackingFeatureSwitch != null) {
                ObjectTrackingFeatureSwitch.Dispose ();
                ObjectTrackingFeatureSwitch = null;
            }

            if (titleTextField != null) {
                titleTextField.Dispose ();
                titleTextField = null;
            }

            if (URLTextField != null) {
                URLTextField.Dispose ();
                URLTextField = null;
            }
        }
    }
}