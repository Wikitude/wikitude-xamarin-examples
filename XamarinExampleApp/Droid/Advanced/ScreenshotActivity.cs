using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Webkit;
using Com.Wikitude.Architect;
using Java.Lang;
using Org.Json;
using System.IO;
using Android.Content;
using Com.Wikitude.Common.Permission;
using Android.Widget;
using Android;

namespace XamarinExampleApp.Droid.Advanced
{
    [Activity(Label = "ScreenshotActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ScreenshotActivity : SimpleArActivity, IArchitectJavaScriptInterfaceListener, ArchitectView.ICaptureScreenCallback, IPermissionManagerPermissionManagerCallback
    {
        private Bitmap screenCapture;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            /*
             * The ArchitectJavaScriptInterfaceListener has to be added to the Architect view after ArchitectView.OnCreate.
             * There may be more than one ArchitectJavaScriptInterfaceListener.
             */
            architectView.AddArchitectJavaScriptInterfaceListener(this);
        }

        protected override void OnDestroy()
        {
            // The ArchitectJavaScriptInterfaceListener has to be removed from the Architect view before ArchitectView.OnDestroy.
            architectView.RemoveArchitectJavaScriptInterfaceListener(this);
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            int[] results = new int[grantResults.Length];
            for (int i = 0; i < grantResults.Length; i++) 
            {
                results[i] = (int)grantResults[i];
            }
            ArchitectView.PermissionManager.OnRequestPermissionsResult(requestCode, permissions, results);
        }

        /*
         * ArchitectJavaScriptInterfaceListener.OnJSONObjectReceived is called whenever
         * AR.platform.sendJSONObject is called in the JavaScript code.
         */
        public void OnJSONObjectReceived(JSONObject jsonObject)
        {
            if (jsonObject.GetString("action") == "capture_screen")
            {
                /*
                 * ArchitectView.CaptureScreen has two different modes:
                 *  - CaptureModeCamAndWebview which will capture the camera and web-view on top of it.
                 *  - CaptureModeCam which will capture ONLY the camera and its content (AR.Drawables).
                 *
                 * OnScreenCaptured will be called once the ArchitectView has processed the screen capturing and will
                 * provide a Bitmap containing the screenshot.
                 */
                architectView.CaptureScreen(ArchitectView.CaptureScreenCallback.CaptureModeCamAndWebview, this);
            }
        }

        public void OnScreenCaptured(Bitmap screenCapture)
        {
            if (screenCapture == null)
            {
                Toast.MakeText(this, Resource.String.error_screen_capture, ToastLength.Short);
            }
            else
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    ScreenCapture.SaveScreenCaptureToExternalStorage(this, screenCapture);
                } else
                {
                    this.screenCapture = screenCapture;
                    ArchitectView.PermissionManager.CheckPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 123, this);
                }
            }
        }

        public void PermissionsDenied(string[] deniedPermissions)
        {
            Toast.MakeText(this, GetString(Resource.String.permissions_denied) + string.Join(",", deniedPermissions), ToastLength.Short).Show();
        }

        public void PermissionsGranted(int requestCode)
        {
            ScreenCapture.SaveScreenCaptureToExternalStorage(this, screenCapture);
        }

        public void ShowPermissionRationale(int requestCode, string[] permissions)
        {
            var alertBuilder = new AlertDialog.Builder(this);
            alertBuilder.SetCancelable(true);
            alertBuilder.SetTitle(Resource.String.permission_rationale_title);
            alertBuilder.SetMessage(GetString(Resource.String.permission_rationale_text) + string.Join(",", permissions));

            alertBuilder.SetPositiveButton(Android.Resource.String.Yes, new System.EventHandler<DialogClickEventArgs>((sender, eventArgs) =>
            {
                ArchitectView.PermissionManager.PositiveRationaleResult(requestCode, permissions);
            }));

            var alert = alertBuilder.Create();
            alert.Show();
        }
    }
}
