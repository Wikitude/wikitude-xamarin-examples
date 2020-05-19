using System;
using AVFoundation;
using WikitudeComponent.iOS;

namespace XamarinExampleApp.iOS.CoreServices
{
    public class iOSArExperienceConverter
    {
        public iOSArExperienceConverter()
        {
        }

        public static WTFeatures ConvertFeatures(Features featuresMask)
        {
            WTFeatures iOSFeatures = 0;
            if ( (featuresMask & Features.ImageTracking) == Features.ImageTracking ) {
                iOSFeatures |= WTFeatures.WTFeature_ImageTracking;
            }
            if ((featuresMask & Features.ObjectTracking) == Features.ObjectTracking) {
                iOSFeatures |= WTFeatures.WTFeature_ObjectTracking;
            }
            if ((featuresMask & Features.InstantTracking) == Features.InstantTracking) {
                iOSFeatures |= WTFeatures.WTFeature_InstantTracking;
            }
            if ( (featuresMask & Features.Geo) == Features.Geo ) {
                iOSFeatures |= WTFeatures.WTFeature_Geo;
            }

            return iOSFeatures;
        }

        public static AVCaptureDevicePosition ConvertCameraPosition(CameraPosition cameraPosition)
        {
            switch (cameraPosition) {
                case CameraPosition.Back:
                    return AVCaptureDevicePosition.Back;
                case CameraPosition.Front:
                    return AVCaptureDevicePosition.Front;                
                case CameraPosition.Default:
                    return AVCaptureDevicePosition.Back;
                default:
                    return AVCaptureDevicePosition.Back;
            }
        }

        public static WTCaptureDeviceResolution ConvertCameraResolution(CameraResolution cameraResolution)
        {
            switch (cameraResolution) {
                case CameraResolution.SD_640x480:
                    return WTCaptureDeviceResolution.WTCaptureDeviceResolution_SD_640x480;
                case CameraResolution.HD_1280x720:
                    return WTCaptureDeviceResolution.WTCaptureDeviceResolution_HD_1280x720;
                case CameraResolution.Full_HD_1920x1080:
                    return WTCaptureDeviceResolution.WTCaptureDeviceResolution_FULL_HD_1920x1080;
                case CameraResolution.Auto:
                    return WTCaptureDeviceResolution.WTCaptureDeviceResolution_AUTO;
                default:
                    return WTCaptureDeviceResolution.WTCaptureDeviceResolution_SD_640x480;
            }
        }

        public static AVCaptureFocusMode ConvertCameraFocusMode(CameraFocusMode cameraFocusMode)
        {
            switch (cameraFocusMode) {
                case CameraFocusMode.AutofocusOff:
                    return AVCaptureFocusMode.Locked;
                case CameraFocusMode.AutofocusOnce:
                    return AVCaptureFocusMode.AutoFocus;
                case CameraFocusMode.AutofocusContinuous:
                    return AVCaptureFocusMode.ContinuousAutoFocus;
                default:
                    return AVCaptureFocusMode.ContinuousAutoFocus;
            }
        }

        // add a method that returns a startup configuration from an ArExperience
    }
}
