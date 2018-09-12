using System;
using Com.Wikitude.Common.Camera;

namespace XamarinExampleApp.Droid.Util
{
    public static class PlatformConverter
    {

        public static CameraSettings.CameraPosition ConvertSharedToPlatformPosition(CameraPosition position)
        {
            switch (position)
            {
                case CameraPosition.Front:
                    return CameraSettings.CameraPosition.Front;
                case CameraPosition.Back:
                    return CameraSettings.CameraPosition.Back;
                default:
                    return CameraSettings.CameraPosition.Default;
            }
        }

        public static CameraSettings.CameraResolution ConvertSharedToPlatformResolution(CameraResolution resolution)
        {
            switch (resolution)
            {
                case CameraResolution.HD_1280x720:
                    return CameraSettings.CameraResolution.HD1280x720;
                case CameraResolution.Full_HD_1920x1080:
                    return CameraSettings.CameraResolution.FULLHD1920x1080;
                case CameraResolution.Auto:
                    return CameraSettings.CameraResolution.Auto;
                default:
                    return CameraSettings.CameraResolution.SD640x480;
            }
        }

        public static CameraSettings.CameraFocusMode ConvertSharedToPlatformFocusMode(CameraFocusMode focusMode)
        {
            switch (focusMode)
            {
                case CameraFocusMode.AutofocusOnce:
                    return CameraSettings.CameraFocusMode.Once;
                case CameraFocusMode.AutofocusOff:
                    return CameraSettings.CameraFocusMode.Off;
                default:
                    return CameraSettings.CameraFocusMode.Continuous;
            }
        }
    }
}
