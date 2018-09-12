using Foundation;
using UIKit;
using WikitudeComponent.iOS;

namespace XamarinExampleApp.iOS.AdvancedViewControllers
{
    public partial class NativeDetailsViewController : ArExperienceViewController
    {
        public NativeDetailsViewController(ArExperience arExperience) : base(arExperience)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            architectView.ReceivedJSONObject += (object sender, ArchitectViewReceivedJSONObjectEventArgs e) =>
            {
                string action = e.JsonObject.ObjectForKey(new NSString("action")).ToString();
                if (action == "present_poi_details")
                {
                    UIStoryboard mainStoryboard = UIStoryboard.FromName("Main", NSBundle.MainBundle);
                    PoiDetailViewController poiDetailViewController = (PoiDetailViewController)mainStoryboard.InstantiateViewController("poi_detail_view_controller");
                    poiDetailViewController.SetPoiDetails(e.JsonObject);

                    NavigationController.PushViewController(poiDetailViewController, true);
                }
            };
        }
    }
}

