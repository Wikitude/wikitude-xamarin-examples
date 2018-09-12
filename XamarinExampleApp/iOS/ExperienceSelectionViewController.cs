using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using WikitudeComponent.iOS;
using XamarinExampleApp.iOS.CoreServices;
using XamarinExampleApp.iOS.AdvancedViewControllers;

namespace XamarinExampleApp.iOS
{
    public partial class ExperienceSelectionViewController : UITableViewController
    {
        private List<ArExperienceGroup> experienceGroups;
        private WTAuthorizationRequestManager authorizationRequestManager = new WTAuthorizationRequestManager();

        public ExperienceSelectionViewController(IntPtr handle) : base(handle)
        {
            NSUrl exampleDefinitionsFileURL = NSBundle.MainBundle.GetUrlForResource("samples", "json", "ARchitectExamples");
            if (exampleDefinitionsFileURL != null)
            {
                NSData exampleExperienceData = NSData.FromUrl(exampleDefinitionsFileURL);
                NSString exampleExperienceDefinitions = NSString.FromData(exampleExperienceData, NSStringEncoding.UTF8);
                experienceGroups = ArExperienceManager.ParseExampleDefintion(exampleExperienceDefinitions);
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return experienceGroups.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return experienceGroups[Convert.ToInt32(section.ToString())].Name;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return experienceGroups[Convert.ToInt32(section.ToString())].ArExperiences.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell("example_experience_cell_identifier");

            ArExperience exampleExperience = experienceGroups[indexPath.Section].ArExperiences[indexPath.Row];

            cell.TextLabel.Text = exampleExperience.Name;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ArExperience selectedExampleExperience = experienceGroups[indexPath.Section].ArExperiences[indexPath.Row];
            WTFeatures requiredFeatures = iOSArExperienceConverter.ConvertFeatures(selectedExampleExperience.FeaturesMask);

            ArExperienceAuthorizationController.AuthorizeRestricedAPIAccess(authorizationRequestManager, requiredFeatures, () => {
                UIViewController arExperienceViewController = GetViewControllerForArExperience(selectedExampleExperience);
                NavigationController.PushViewController(arExperienceViewController, true);
            }, (UIAlertController alertController) => {
                tableView.DeselectRow(indexPath, true);
                PresentViewController(alertController, true, null);
            });
        }

        [Action("ComeBackFromSDKInformationViewController:")]
        public void ComeBackFromSDKInformationViewController(UIStoryboardSegue segue)
        {

        }

        [Action("ComeBackFromManageURLsViewController:")]
        public void ComeBackFromManageURLsViewController(UIStoryboardSegue segue)
        {

        }

        #region Private Methods
        private UIViewController GetViewControllerForArExperience(ArExperience arExperience)
        {
            switch (arExperience.Extension)
            {
                case "Screenshot":
                    CaptureScreenshotViewController captureGestureExampleScreenshotViewController = new CaptureScreenshotViewController(arExperience);
                    return captureGestureExampleScreenshotViewController;
                case "SaveAndLoadInstantTarget":
                    SaveAndLoadInstantTargetViewController saveAndLoadInstantTargetViewController = new SaveAndLoadInstantTargetViewController(arExperience);
                    return saveAndLoadInstantTargetViewController;
                case "ObtainPoiDataFromApplicationModel":
                    ObtainPoiDataFromApplicationModelViewController obtainPoiDataFromApplicationModelViewController = new ObtainPoiDataFromApplicationModelViewController(arExperience);
                    return obtainPoiDataFromApplicationModelViewController;
                case "NativeDetailScreen":
                    NativeDetailsViewController nativeDetailsViewController = new NativeDetailsViewController(arExperience);
                    return nativeDetailsViewController;
                case "PoisCaptureScreen":
                    CaptureScreenshotViewController capturePoiScreenshotViewController = new CaptureScreenshotViewController(arExperience);
                    return capturePoiScreenshotViewController;
                default:
                    ArExperienceViewController arExperienceViewController = new ArExperienceViewController(arExperience);
                    return arExperienceViewController;
            }
        }
        #endregion
    }
}