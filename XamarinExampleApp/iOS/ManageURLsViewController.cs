using Foundation;
using System;
using System.IO;
using System.Collections.Generic;
using UIKit;
using WikitudeComponent.iOS;
using XamarinExampleApp.iOS.CoreServices;

namespace XamarinExampleApp.iOS
{
    public partial class ManageURLsViewController : UITableViewController
    {
        protected ArExperienceGroup arExperienceGroup = new ArExperienceGroup("", new List<ArExperience>());
        protected string persitentArExperienceGroupFilePath;
        protected WTAuthorizationRequestManager authorizationRequestManager = new WTAuthorizationRequestManager();

        public ManageURLsViewController (IntPtr handle) : base (handle)
        {
            string documentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            persitentArExperienceGroupFilePath = Path.Combine(documentsDirectoryPath, "favourites");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ArExperienceGroup restoredArExperienceGroup = ArExperienceManager.OpenFromFile(persitentArExperienceGroupFilePath);
            if (restoredArExperienceGroup != null)
            {
                arExperienceGroup = restoredArExperienceGroup;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return arExperienceGroup.ArExperiences.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ArExperience arExperience = arExperienceGroup.ArExperiences[indexPath.Row];
            UITableViewCell cell = tableView.DequeueReusableCell("custom_experience_cell_identifier");
            cell.TextLabel.Text = arExperience.Path;
            cell.DetailTextLabel.Text = arExperience.Name;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ArExperience selectedArExperience = arExperienceGroup.ArExperiences[indexPath.Row];
            WTFeatures requiredFeatures = iOSArExperienceConverter.ConvertFeatures(selectedArExperience.FeaturesMask);

            ArExperienceAuthorizationController.AuthorizeRestricedAPIAccess(authorizationRequestManager, requiredFeatures, () => {
                ArExperienceViewController arExperienceViewController = new ArExperienceViewController(selectedArExperience);
                NavigationController.PushViewController(arExperienceViewController, true);
            }, (UIAlertController alertController) => {
                tableView.DeselectRow(indexPath, true);
                PresentViewController(alertController, true, null);
            });
        }

        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewRowAction deleteRowAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Delete", (UITableViewRowAction rowAction, NSIndexPath actionIndexPath) =>
            {
                arExperienceGroup.ArExperiences.RemoveAt(actionIndexPath.Row);
                tableView.DeleteRows(new NSIndexPath[] { actionIndexPath }, UITableViewRowAnimation.Right);
                ArExperienceManager.WriteToFile(persitentArExperienceGroupFilePath, arExperienceGroup);
            });

            UITableViewRowAction editRowAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, "Edit", (UITableViewRowAction rowAction, NSIndexPath actionIndexPath) =>
            {
                PerformSegue("add_url_view_controller_segue", actionIndexPath);
            });
            editRowAction.BackgroundColor = UIColor.FromRGB(0.060f, 0.502f, 0.998f);

            return new UITableViewRowAction [] {deleteRowAction, editRowAction};
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.DestinationViewController.GetType() == typeof(UINavigationController))
            {
                if (sender != null && sender.GetType() == typeof(NSIndexPath))
                {
                    UINavigationController navigationController = (UINavigationController)segue.DestinationViewController;
                    AddURLViewController addURLViewController = (AddURLViewController)navigationController.TopViewController;

                    ArExperience selectedArExperience = arExperienceGroup.ArExperiences[((NSIndexPath)sender).Row];
                    addURLViewController.EditExistingArExperience(selectedArExperience);
                }
            }
        }

        [Action("ComeBackFromAddURLViewController:")]
        public void ComeBackFromAddURLViewController(UIStoryboardSegue segue)
        {
            if (segue.SourceViewController.GetType() == typeof(AddURLViewController))
            {
                AddURLViewController addURLViewController = (AddURLViewController)segue.SourceViewController;
                if (addURLViewController.GetState() == AddURLViewController.State.Created)
                {
                    ArExperience arExperience = addURLViewController.CreatedArExperience();
                    arExperienceGroup.ArExperiences.Insert(0, arExperience);

                    TableView.InsertRows(new NSIndexPath[] { NSIndexPath.Create(new int[] {0, 0}) }, UITableViewRowAnimation.Top);
                }
                else if (addURLViewController.GetState() == AddURLViewController.State.Edited)
                {
                    ArExperience updatedArExperience = addURLViewController.UpdatedArExperience();
                    int index = arExperienceGroup.ArExperiences.FindIndex(x => x == updatedArExperience);
                    arExperienceGroup.ArExperiences[index] = addURLViewController.CreatedArExperience();
                    TableView.ReloadRows(new NSIndexPath[] { NSIndexPath.Create(new int[] { 0, index }) }, UITableViewRowAnimation.Automatic);
                }
                ArExperienceManager.WriteToFile(persitentArExperienceGroupFilePath, arExperienceGroup);
            }
        }
    }
}