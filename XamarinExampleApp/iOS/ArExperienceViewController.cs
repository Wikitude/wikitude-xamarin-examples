using System;
using UIKit;
using Foundation;
using CoreGraphics;
using AVFoundation;
using WikitudeComponent.iOS;
using XamarinExampleApp.iOS.CoreServices;

namespace XamarinExampleApp.iOS
{
    public partial class ArExperienceViewController : UIViewController, IUIGestureRecognizerDelegate
    {
        protected class ArchitectDelegate : WTArchitectViewDelegate
        {
            [Weak]
            protected ArExperienceViewController arExperienceViewController;
            public ArchitectDelegate(ArExperienceViewController arExperienceViewController)
            {
                this.arExperienceViewController = arExperienceViewController;
            }

            public override void DidFinishLoadNavigation(WTArchitectView architectView, WTNavigation navigation)
            {
                Console.WriteLine("Finished loading Architect World");
                arExperienceViewController.ArchitectWorldFinishedLoading(navigation);
            }

            public override void DidFailToLoadNavigation(WTArchitectView architectView, WTNavigation navigation, NSError error)
            {
                string errorMessage = error.LocalizedDescription + " ('" + navigation.OriginalURL + "')";
                UIAlertController failedToLoadArchitectWorldAlertController = UIAlertController.Create("Failed to load Architect World", errorMessage, UIAlertControllerStyle.Alert);
                failedToLoadArchitectWorldAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                arExperienceViewController.PresentViewController(failedToLoadArchitectWorldAlertController, true, null);
            }

            public override UIViewController PresentingViewControllerForViewControllerPresentationInArchitectView(WTArchitectView architectView)
            {
                return arExperienceViewController;
            }
        }

        protected class NavigationControllerDelegate : UINavigationControllerDelegate
        {
            [Weak]
            protected WTArchitectView architectView = null;

            public NavigationControllerDelegate(WTArchitectView architectView)
            {
                this.architectView = architectView;
            }

            public override void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
            {         
                if (viewController.GetType() == typeof(ExperienceSelectionViewController) && architectView != null)
                {             
                    architectView.RemoveFromSuperview();
                    architectView.Dispose();                 
                }
            }
        }


        protected WTArchitectView architectView;
        protected ArchitectDelegate delegateObject;

        [Weak]
        protected ArExperience currentArExperience;
        protected WTNavigation loadedArExperienceNavigation = null;
        protected WTNavigation loadingArExperienceNavigation = null;

        protected NSObject applicationWillResignActiveObserver;
        protected NSObject applicationDidBecomeActiveObserver;

        protected AVCaptureDevicePosition currentCaptureDevicePosition;

        protected bool isRunning;


        public ArExperienceViewController(ArExperience arExperience) : base("ArExperienceViewController", null)
        {
            currentArExperience = arExperience;
            currentCaptureDevicePosition = AVCaptureDevicePosition.Unspecified;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            architectView = new WTArchitectView();
            architectView.SetLicenseKey("Woex1pskjRx8tnj0+cbhDcemFHEGQT1szReV2lRT1OEIoY+nd3HK6SZQTqiUZc8kjzA7KKH1CkJFTC0NHICBG0WOxhEOsSg8T3dm1B/OltukL1AFS2AfCnqYjLSimBLupveDxftECiH6WISVdBawXC+vUg/dVFAN/1/yyv/+k55TYWx0ZWRfX8xFOjxPU2nrT+dUGa+/wyw96rvh3ZBKQ/iDmtYkysmSjsuvg7p3lWlQwz4b3DfFfPBTTHSAcEObY6nBBf8WI4b3/LqoOIPv734wETomYqPgaCCsGYkSdy9f/p4cfNq7ij2iQxwG+vDlcqSBjD60uwI6A0BxlezOaqP8u6MbttJobGgtshn0Gca44GOPW4s2gmHRuiOGlDhogbDsslhScbAbdS3IF2BIGuXEylp7RTzAT/XdC/MSaOq86o59O4i4jUl4wi0c1XRKpj0U7NYSMBhgc3B1S/7EL486Z4nGlVns9EwvTX0cVqDAUegG/CWnf1CDZKNen6kqpe0rbe7O6TKerv4sMFUA6Kl4FPNgy3lcG5mNtUa6X1jiMW8hfZUbgAaZ5ZSGCpaiaTUGGYXfv3GnoYJx28/KnwW38khd0hdQ5pv228LPCnusBFpt+VOR6EUj/hZj5ZvRJ1Wo4Ar8teSKT2HcwjZYOY2TKQfTnfzr12zNaOXG5S/eMJCKrMJZmcGsIxEgNRKEVPm7h+L3ClwP57PFkis5ym91moHfZllZg5+EUKzG4bhVlSLHugz6BycfBpACrBVQNsdjmhQkpwzIli9vGcmWHhVl3cgrezfhDuCopg3jsHJEqvw37J2RkSyFr6BWY98YngbQPj6AdxTPr2ews0hiayjfpU0N+gfVUAmUgTFY9S6YYb5pTayRLqoMKH1BfCrhXIBsv/cpi37AbY8JZ4sSvv1zvr0XprOkQSsHdllR+Sc4QrWFmv4h+as5ZPCB7cMbjedocRonazzVMHIENyhX/BrD+EAJobM2OduTVwv3ci0gPRhTdaURx1oicdQbI1CS5k3hFwmA4UNvRMeFzcalK0M6UfodzkoPXcAfYpvX4SHwxu1s8M318Q9JGPT5H5G4spnZNUxF6llue98FL2aNLF1DlAYDi6uzPe5vj1Z3/g5eWcdL877C9Z8oWALkfVe1T/608YdnxET5JsyF06o3T4ZiB7HSnsqxWMDDBBKfjFN6Rc/oD/Sq1W4bDboVjfi0at8TArMwUoj3g++DtLXa46g+03g3mGGYkUWxyuBXTc7upNY1lVyjKN+oSd+2vQ7ttQQV2MJX1hVBGsN82hYwQ8YX4AY1ZKrj3Qx5gC5+I+02k8a85PjOHCKMPb7qKFSy");
            delegateObject = new ArchitectDelegate(this);
            architectView.Delegate = delegateObject;
            architectView.TranslatesAutoresizingMaskIntoConstraints = false;
            Add(architectView);

            architectView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            architectView.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            architectView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            architectView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            NavigationController.Delegate = new NavigationControllerDelegate(architectView);

            EdgesForExtendedLayout = UIRectEdge.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationItem.Title = currentArExperience.Name;

            NavigationController.InteractivePopGestureRecognizer.Delegate = this;

            LoadArExperienceIfRequired();
            StartArchitectViewRendering();

            UIInterfaceOrientation currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
            architectView.SetShouldRotateToInterfaceOrientation(true, currentOrientation);

            applicationWillResignActiveObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillResignActiveNotification, ApplicationWillResignActive);
            applicationDidBecomeActiveObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, ApplicationDidBecomeActive);

            // Prevent device from sleeping
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NavigationController.InteractivePopGestureRecognizer.Delegate = null;

            // Restore sleep ability to device
            UIApplication.SharedApplication.IdleTimerDisabled = false;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            StopArchitectViewRendering();

            NSNotificationCenter.DefaultCenter.RemoveObserver(applicationWillResignActiveObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(applicationDidBecomeActiveObserver);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            if (coordinator != null)
            {
                coordinator.AnimateAlongsideTransition((IUIViewControllerTransitionCoordinatorContext context) =>
                {
                    UIInterfaceOrientation newInterfaceOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                    architectView.SetShouldRotateToInterfaceOrientation(true, newInterfaceOrientation);
                }, null);
            }
            else
            {
                UIInterfaceOrientation newInterfaceOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                architectView.SetShouldRotateToInterfaceOrientation(true, newInterfaceOrientation);
            }

            base.ViewWillTransitionToSize(toSize, coordinator);
        }

        public void ArchitectWorldFinishedLoading(WTNavigation navigation)
        {
            if ( loadingArExperienceNavigation.Equals(navigation) ) 
            {
                loadedArExperienceNavigation = navigation;
            }
        }

        #region Delegation
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }
        #endregion

        #region Notifications
        private void ApplicationWillResignActive(NSNotification notification)
        {
            StopArchitectViewRendering();
        }

        private void ApplicationDidBecomeActive(NSNotification notification)
        {
            StartArchitectViewRendering();
        }
        #endregion

        #region Private Methods
        private void LoadArExperienceIfRequired()
        {
            NSUrl experienceURLValidator(string URL)
            {
                if (URL.Contains("https"))
                {
                    return NSUrl.FromString(URL);
                }
                else
                {
                    NSUrl relativeArExperienceURL = NSUrl.FromString(currentArExperience.Path);
                    string bundleSubdirectory = "ARchitectExamples/" + relativeArExperienceURL.RemoveLastPathComponent().AbsoluteString;
                    NSUrl bundleArExperienceURL = NSBundle.MainBundle.GetUrlForResource("index", "html", bundleSubdirectory);
                    return bundleArExperienceURL;
                }
            }
            NSUrl fullArExperienceURL = experienceURLValidator(currentArExperience.Path);

            if ( loadedArExperienceNavigation == null || (loadedArExperienceNavigation != null && !loadedArExperienceNavigation.OriginalURL.Equals(fullArExperienceURL) ) )
            {
                loadingArExperienceNavigation = architectView.LoadArchitectWorldFromURL(fullArExperienceURL);
            }
        }

        private void StartArchitectViewRendering()
        {
            if (!architectView.IsRunning)
            {
                architectView.Start((WTArchitectStartupConfiguration architectStartupConfiguration) =>
                {
                    if ( currentCaptureDevicePosition != AVCaptureDevicePosition.Unspecified ) {
                        architectStartupConfiguration.CaptureDevicePosition = currentCaptureDevicePosition;
                    } else {
                        architectStartupConfiguration.CaptureDevicePosition = iOSArExperienceConverter.ConvertCameraPosition(currentArExperience.CameraPosition);
                    }
                    architectStartupConfiguration.CaptureDeviceResolution = iOSArExperienceConverter.ConvertCameraResolution(currentArExperience.CameraResolution);
                    architectStartupConfiguration.CaptureDeviceFocusMode = iOSArExperienceConverter.ConvertCameraFocusMode(currentArExperience.CameraFocusMode);
                }, (bool success, NSError error) =>
                {
                    isRunning = success;
                });
            }
        }

        private void StopArchitectViewRendering()
        {
            if (isRunning)
            {
                architectView.Stop();
            }
        }
        #endregion
    }
}
