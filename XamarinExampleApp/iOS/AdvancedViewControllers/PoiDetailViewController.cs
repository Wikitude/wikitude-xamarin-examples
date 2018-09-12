using Foundation;
using System;
using UIKit;

namespace XamarinExampleApp.iOS
{
    public partial class PoiDetailViewController : UITableViewController
    {

        protected NSDictionary poiDetails;
        public PoiDetailViewController (IntPtr handle) : base (handle)
        {
        }

        public void SetPoiDetails(NSDictionary poiDetails)
        {
            this.poiDetails = poiDetails;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationItem.Title = "Poi Details";

            UITableViewCell identifierCell = TableView.CellAt(NSIndexPath.Create(new int[] { 0, 0 }));
            identifierCell.DetailTextLabel.Text = (NSString)poiDetails.ObjectForKey(new NSString("id"));

            UITableViewCell titleCell = TableView.CellAt(NSIndexPath.Create(new int[] { 0, 1 }));
            titleCell.DetailTextLabel.Text = (NSString)poiDetails.ObjectForKey(new NSString("title"));

            UITableViewCell descriptionCell = TableView.CellAt(NSIndexPath.Create(new int[] { 0, 2 }));
            descriptionCell.DetailTextLabel.Text = (NSString)poiDetails.ObjectForKey(new NSString("description"));
        }
    }
}