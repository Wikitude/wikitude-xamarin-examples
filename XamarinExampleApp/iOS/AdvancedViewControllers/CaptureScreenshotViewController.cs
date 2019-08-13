using UIKit;
using Foundation;
using WikitudeComponent.iOS;

namespace XamarinExampleApp.iOS.AdvancedViewControllers
{
    public partial class CaptureScreenshotViewController : ArExperienceViewController
    {
        public CaptureScreenshotViewController(ArExperience arExperience) : base(arExperience)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            architectView.ReceivedJSONObject += (object sender, ArchitectViewReceivedJSONObjectEventArgs e) => {
                string action = e.JsonObject.ObjectForKey(new NSString("action")).ToString();
                switch ( action ) {
                    case "capture_screen":
                        NSDictionary userInfo = new NSDictionary();
                        architectView.CaptureScreenWithMode(WTScreenshotCaptureMode._CamAndWebView, WTScreenshotSaveMode._PhotoLibrary, WTScreenshotSaveOptions.CallDelegateOnSuccess, userInfo);
                        break;
                    case "present_poi_details":
                        UIStoryboard mainStoryboard = UIStoryboard.FromName("Main", NSBundle.MainBundle);
                        PoiDetailViewController poiDetailViewController = (PoiDetailViewController)mainStoryboard.InstantiateViewController("poi_detail_view_controller");
                        poiDetailViewController.SetPoiDetails(e.JsonObject);

                        NavigationController.PushViewController(poiDetailViewController, true);
                        break;

                    default:
                        break;
                }
            };

            architectView.DidCaptureScreenWithContext += (object sender, ArchitectViewCaptureScreenEventArgs e) => {
                UIAlertController screencaptureSucceededAlertController = UIAlertController.Create("Successfully stored screenshot", "You find the screenshot in the Photo Library", UIAlertControllerStyle.Alert);
                screencaptureSucceededAlertController.AddAction(UIAlertAction.Create("Open", UIAlertActionStyle.Default, (UIAlertAction action) => {
                    UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("photos-redirect://"));
                }));
                screencaptureSucceededAlertController.AddAction(UIAlertAction.Create("Dismiss", UIAlertActionStyle.Cancel, null));
                PresentViewController(screencaptureSucceededAlertController, true, null);
            };
            architectView.DidFailCaptureScreenWithError += (object sender, ArchitectViewFailCaptureScreenEventArgs e) => {
                UIAlertController screencaptureFailedAlertController = UIAlertController.Create("Failed to capture screen", e.Error.LocalizedDescription, UIAlertControllerStyle.Alert);
                screencaptureFailedAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(screencaptureFailedAlertController, true, null);
            };
        }
    }
}

