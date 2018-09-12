using System;
using Foundation;
using UIKit;
using WikitudeComponent.iOS;

namespace XamarinExampleApp.iOS.CoreServices
{
    public static class ArExperienceAuthorizationController
    {
        public static void AuthorizeRestricedAPIAccess(WTAuthorizationRequestManager authorizationRequestManager, WTFeatures requiredFeatures, Action successHandler, Action<UIAlertController> errorHandler)
        {
            if (!authorizationRequestManager.RequestingRestrictedAppleiOSSDKAPIAuthorization)
            {
                NSOrderedSet<NSNumber> restrictedAppleiOSSDKAPIs = WTAuthorizationRequestManager.RestrictedAppleiOSSDKAPIAuthorizationsForRequiredFeatures(requiredFeatures);
                authorizationRequestManager.RequestRestrictedAppleiOSSDKAPIAuthorization(restrictedAppleiOSSDKAPIs, (bool success, NSError error) =>
                {
                    if (success)
                    {
                        successHandler();
                    }
                    else
                    {
                        NSDictionary unauthorizedAPIInfo = (NSDictionary)error.UserInfo.ObjectForKey(WTAuthorizationRequestManager.WTUnauthorizedAppleiOSSDKAPIsKey);

                        string detailedAuthorizationErrorLogMessage = "The following authorization states do not meet the requirements: ";
                        string missingAuthorizations = "In order to use the Wikitude SDK, please grant access to the following:";
                        foreach (NSString unauthorizedAPIKey in unauthorizedAPIInfo.Keys)
                        {
                            int authorizationStatus = Int32.Parse(unauthorizedAPIInfo.ObjectForKey(unauthorizedAPIKey).ToString());
                            detailedAuthorizationErrorLogMessage += "\n" + unauthorizedAPIKey.ToString() + " = " + WTAuthorizationRequestManager.StringFromAuthorizationStatusForUnauthorizedAppleiOSSDKAPI(authorizationStatus, unauthorizedAPIKey);
                            missingAuthorizations += "\n* " + WTAuthorizationRequestManager.HumanReadableDescriptionForUnauthorizedAppleiOSSDKAPI(unauthorizedAPIKey);
                        }

                        Console.WriteLine(detailedAuthorizationErrorLogMessage);

                        UIAlertController settingsAlertController = UIAlertController.Create("Required API authorizations missing", missingAuthorizations, UIAlertControllerStyle.Alert);
                        settingsAlertController.AddAction(UIAlertAction.Create("Open Settings", UIAlertActionStyle.Default, (UIAlertAction action) =>
                        {
                            UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString));
                        }));
                        settingsAlertController.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Destructive, null));

                        errorHandler(settingsAlertController);
                    }
                });
            }
        }
    }
}
