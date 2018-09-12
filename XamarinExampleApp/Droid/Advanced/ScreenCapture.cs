using System;
using System.IO;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Com.Wikitude.Architect;
using Com.Wikitude.Common.Permission;
using Java.Lang;

namespace XamarinExampleApp.Droid.Advanced
{
    public static class ScreenCapture
    {
        /*
         * This method will store the screenshot in a file and will create an intent to share it.
         */
        public static void SaveScreenCaptureToExternalStorage(Activity activity, Bitmap screenCapture)
        {
            // store screenCapture into external cache directory
            var externalStoragePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var screenCapturePath = System.IO.Path.Combine(externalStoragePath, "screenCapture_" + JavaSystem.CurrentTimeMillis() + ".jpg");
            var screenCaptureFile = new Java.IO.File(screenCapturePath);

            // 1. Save bitmap to file & compress to jpeg. You may use PNG too
            using (var stream = new FileStream(screenCapturePath, FileMode.Create))
            {
                screenCapture.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
            }

            // 2. create send intent
            Intent share = new Intent(Intent.ActionSend);
            share.SetType("image/jpg");
            var apkURI = Android.Support.V4.Content.FileProvider.GetUriForFile(
                activity.ApplicationContext,
                "com.wikitude.xamarinexample.provider",
                screenCaptureFile);
            share.AddFlags(ActivityFlags.GrantReadUriPermission);
            share.PutExtra(Intent.ExtraStream, apkURI);


            // 3. launch intent-chooser
            string chooserTitle = "Share Snaphot";
            activity.StartActivity(Intent.CreateChooser(share, chooserTitle));
        }
    }
}
