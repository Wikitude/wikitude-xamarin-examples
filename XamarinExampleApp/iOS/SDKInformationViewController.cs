using Foundation;
using System;
using UIKit;
using WikitudeComponent.iOS;

namespace XamarinExampleApp.iOS
{
    public partial class SDKInformationViewController : UITableViewController
    {
        public SDKInformationViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UITableViewCell versionNumberCell = this.TableView.CellAt(NSIndexPath.Create(new int[] { 0, 0 }));
            versionNumberCell.DetailTextLabel.Text = WTArchitectView.SDKVersion;

            UITableViewCell buildDateCell = this.TableView.CellAt(NSIndexPath.Create(new int[] { 1, 0 }));
            buildDateCell.DetailTextLabel.Text = WTArchitectView.SDKBuildInformation.BuildDate;

            UITableViewCell buildNumberCell = this.TableView.CellAt(NSIndexPath.Create(new int[] { 1, 1 }));
            buildNumberCell.DetailTextLabel.Text = WTArchitectView.SDKBuildInformation.BuildNumber;

            UITableViewCell buildConfigurationCell = TableView.CellAt(NSIndexPath.Create(new int[] { 1, 2 }));
            buildConfigurationCell.DetailTextLabel.Text = WTArchitectView.SDKBuildInformation.BuildConfiguration;
        }
    }
}