using System;
using System.IO;
using Android;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
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
            // 1. Save bitmap to file & compress to jpeg. You may use PNG too
            var resolver = activity.ContentResolver;
            var values = new ContentValues();
            values.Put(MediaStore.MediaColumns.MimeType, "image/jpeg");

            var uri = resolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);
        
            using (var stream = resolver.OpenOutputStream(uri))
            {
                screenCapture.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
            }

            // 2. create send intent
            Intent share = new Intent(Intent.ActionSend);
            share.SetType("image/jpg");
            share.AddFlags(ActivityFlags.GrantReadUriPermission);
            share.PutExtra(Intent.ExtraStream, uri);


            // 3. launch intent-chooser
            string chooserTitle = "Share Snaphot";
            activity.StartActivity(Intent.CreateChooser(share, chooserTitle));
        }
    }
}
