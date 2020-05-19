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
            architectView.SetLicenseKey("rRcPFV/GWHOalFjHX9rP9TWGNRKVu8P4FSKvHtps1mo14SexXUmlVAebLNuKKr9OcOFD89RiMH03AY3eJL09d3Pbvb/V+AVYsQiBROkqqAhYe2lDojp++ZAPDx2RM9rJrD+1qYyUUbdUyKzIJXrU09u4tST9NdhER08njP2tMydTYWx0ZWRfX/p90uj/Yn9x/bcRTK6REaUg/GJT6uUKh7KfnXmxAtt0RI9WNjVPQFFjS1WFGtrRI43/VqyS0gnfsjmiov6fyrE+0aGBxJIzBNWupROE+AYw9LFkJ0gRN6KhsqvawIobvSPbVH+OaYanwnIV8q34LyTRujMzvJL+ke0hEfucf6eChYWe3O5kGCRD09oDnBzBLYnZotRjtuDb2eiHksj28kNuHJTlWItLA4A5Xjri7I1FmnCnTYezZfS2EHHazgOwfYAx+RMTSDXkdjrfradWo4kQFlERljYr1fXTh0T9s19r9FJTeao5/4UbUqcAW8mu71LoIQ5i2gJLDEp4d7xBEBaSznQ2TI4DSNW13lGlTXx8Ma47sFk4uxcxNy1S56RC1bPXA/iJGudxQrGMlhrYuwYcbEpKqEAqRB3xCZKV0M/69hlcZTreu3+1LbtYpLBFQ9GGMPC5FMjzVt29UFSVFyChB6PJlfVrpXbyvlq7ZFKWPc77HKIUyVhx5cSuI19pMxoTPiK5FfcuD7NeUJISK2loWzM/Cd5kvjqCZf0mGJ4zs9iwAQrkhpBGr07lwyAKJ0wH4ybZIdFXb69uZHnp9YnibYF6cuq5L+66lNPRicm1ojF46Sc6SkiVeZDfS6J1f2UOL1ymEMi3eH7pc8+AQ5JUn7XJWr8xIcYTlBa4HkJkRV7ire2Daij3cNywrcVv1GuReHLyW+UipWGPKrvY8IONHmkLEuAgdU9WupbmVdt24Cjn2s1n/ecIIKIVm9xgvdd5n4DHXKsOOWY03gp43g/5jgTJdl1PNwaVIvnwC1zMchAL5Ld49im8pcZbYiQC/MQqAdixxpORPZ0i6j0TM86K7P6DgSxmMNP/SG4vDx0m9mxvCIzvyevNl69Rc2yRToAwY1yGHMHyT2LwWr1NDhhW620ALR/u8gycvRhICYmISCwuCEBuSK+2UyKuKHk50gCr+xfLenxYshOJC+3dyGgBKXMkh/T8i0vKIBaKX5LcD0BY+msO4h/vrb4dMB61qzxCuJM8ax6O5tuQc4u5WOi/6XrAIRFTCqLMST8U6JKN689s70FJtvQYm0DpbPfYTOfeA53B5fphfsTMQqXFwKPhVLczCoWftmlLhHb/NcmNmCHnTp/Mm9yObyNsiG3oQ1Wbb1a9eMOcJ5y/Wvpi0RSYGwIfJcIIknvJIwPphZ3AJ3K9x/M89kct/J65XZMAMdnM1FbtLRpgKUVAUIUJ/E6V03QP/ElUHHukYjbXABWs/fJ/6uy9E4aXjbmzJQ6I9VKQ1uUsT2Oh8585HoXp6LLiFxADdRSIllJBtuMCmgfrd06qQ/q9wu8xFzvJYBeIT6xlCbsBXgdm");
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
