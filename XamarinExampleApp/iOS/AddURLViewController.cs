using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace XamarinExampleApp.iOS
{
    public partial class AddURLViewController : UITableViewController
    {
        protected class CameraSetting : UIPickerViewModel
        {
            public bool extended { get; set; }
            protected List<string> data;
            protected int defaultIndex { get; set; }
            protected int selectedIndex { get; set; }
            protected UITableView tableView;
            protected NSIndexPath indexPath;

            public CameraSetting(List<string> data, int defaultIndex)
            {
                this.extended = false;
                this.data = data;
                this.defaultIndex = defaultIndex;
                this.selectedIndex = -1;
            }

            public string DefaultData()
            {
                return data[defaultIndex];
            }

            public string SelectedData()
            {
                if (selectedIndex >= 0) {
                    return data[selectedIndex];
                } else {
                    return data[defaultIndex];
                }
            }

            public void SelectData(string data)
            {
                int index = this.data.FindIndex(x => x == data);
                selectedIndex = index;
            }

            public void Attach(UIPickerView pickerView, UITableView tableView, NSIndexPath indexPath)
            {
                pickerView.Model = this;
                pickerView.ShowSelectionIndicator = false;

                this.tableView = tableView;
                this.indexPath = indexPath;

                if (selectedIndex > 0)
                {
                    pickerView.Select(selectedIndex, 0, true);
                }
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return (System.nint)data.Count;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                return data[(int)row];
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                selectedIndex = (int)row;
                string settingValue = data[(int)row];
                UITableViewCell cell = tableView.CellAt(indexPath);
                if (cell != null)
                {
                    cell.DetailTextLabel.Text = settingValue;
                }
            }
        }

        public enum State {
            Created,
            Cancelled,
            Error,
            Edited
        }

        protected NSIndexPath currentIndexPath;
        protected CameraSetting[] cameraSettings;

        protected State state;
        protected ArExperience createdArExperience = null;
        protected ArExperience existedArExperience = null;

        public AddURLViewController(IntPtr handle) : base(handle)
        {
        }

        public State GetState()
        {
            return state;
        }

        public ArExperience CreatedArExperience()
        {
            return createdArExperience;
        }

        public void EditExistingArExperience(ArExperience arExperience)
        {
            existedArExperience = arExperience;
        }

        public ArExperience UpdatedArExperience()
        {
            return existedArExperience;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            cameraSettings = new CameraSetting[] {
                new CameraSetting(new List<string> {"640x480", "1280x720", "1920x1080", "Auto"}, 0),
                new CameraSetting(new List<string> {"Back", "Front"}, 0),
                new CameraSetting(new List<string> {"Once", "Continuous", "Off"}, 0)
            };

            URLTextField.EditingDidEndOnExit += (object sender, EventArgs e) => {
                titleTextField.BecomeFirstResponder();
            };
            titleTextField.EditingDidEndOnExit += (object sender, EventArgs e) => {
                URLTextField.BecomeFirstResponder();
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (existedArExperience != null)
            {
                UpdateFromExistingArExperience(existedArExperience);
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = base.GetCell(tableView, indexPath);
            switch (indexPath.Section)
            {
                case 3:
                    CameraSetting cameraSetting = CameraSettingForRow(indexPath.Row);
                    if (cameraSetting != null)
                    {
                        foreach (UIView subview in cell.ContentView.Subviews)
                        {
                            if (subview.GetType() == typeof(UIPickerView))
                            {
                                UIPickerView pickerView = (UIPickerView)subview;
                                cameraSetting.Attach(pickerView, tableView, NSIndexPath.Create(indexPath.Section, indexPath.Row - 1));
                            }
                        }
                    }
                    else
                    {
                        cameraSetting = CameraSettingForRow(indexPath.Row + 1);
                        if (cameraSetting != null)
                        {
                            cell.DetailTextLabel.Text = cameraSetting.SelectedData();
                        }
                    }

                    break;
            }

            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 3)
            {
                CameraSetting cameraSetting = CameraSettingForRow(indexPath.Row);
                if (cameraSetting != null)
                {
                    if (cameraSetting.extended)
                    {
                        return 120.0f;
                    }
                    else
                    {
                        return 0.0f;
                    }
                }
                else
                {
                    return base.GetHeightForRow(tableView, indexPath);
                }
            }
            else
            {
                return base.GetHeightForRow(tableView, indexPath);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 3)
            {
                if (indexPath.Row == 0 || indexPath.Row == 2 || indexPath.Row == 4)
                {
                    if (currentIndexPath != null && currentIndexPath != indexPath)
                    {
                        CameraSetting selectedCameraSetting = CameraSettingForRow(currentIndexPath.Row +1);
                        if ( selectedCameraSetting != null)
                        {
                            selectedCameraSetting.extended = false;
                        }
                    }
                    currentIndexPath = indexPath;

                    CameraSetting cameraSetting = CameraSettingForRow(indexPath.Row +1);
                    if ( cameraSetting != null )
                    {
                        cameraSetting.extended = !cameraSetting.extended;
                    }
                    tableView.BeginUpdates();
                    tableView.EndUpdates();
                }
            }
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (URLTextField != null && URLTextField.IsFirstResponder)
            {
                URLTextField.ResignFirstResponder();
            }
            if (titleTextField != null && titleTextField.IsFirstResponder)
            {
                titleTextField.ResignFirstResponder();
            }
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            bool performSegue = true;
            if (segueIdentifier == "WTSegueIdentifier_SaveURL")
            {
                if (existedArExperience != null)
                {
                    ArExperience arExperience = ArExperienceFromCurrentInput();
                    if (arExperience != null)
                    {
                        createdArExperience = arExperience;
                        state = State.Edited;
                    }
                    else
                    {
                        UIAlertController invalidArExperienceAlertController = UIAlertController.Create("Invalid Information Given", "The given experience URL is not ATS compliant", UIAlertControllerStyle.Alert);
                        invalidArExperienceAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        PresentViewController(invalidArExperienceAlertController, true, null);

                        state = State.Error;
                        performSegue = false;
                    }
                }
                else 
                {
                    ArExperience arExperience = ArExperienceFromCurrentInput();
                    if (arExperience != null)
                    {
                        createdArExperience = arExperience;
                        state = State.Created;
                    }
                    else
                    {
                        UIAlertController invalidArExperienceAlertController = UIAlertController.Create("Invalid Information Given", "The given experience URL is not ATS compliant", UIAlertControllerStyle.Alert);
                        invalidArExperienceAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        PresentViewController(invalidArExperienceAlertController, true, null);

                        state = State.Error;
                        performSegue = false;
                    }
                }
            }
            else if (segueIdentifier == "WTSegueIdentifier_CancelURLCreation")
            {
                state = State.Cancelled;
            }
            return performSegue;
        }

        #region Private
        private ArExperience ArExperienceFromCurrentInput()
        {
            string title = titleTextField.Text;
            string URL = URLTextField.Text;
            Features features = FeaturesFromSwitchInputs();
            CameraResolution resolution = CameraResolutionFromPickerInput();
            CameraPosition position = CameraPositionFromPickerInput();
            CameraFocusMode focusMode = CameraFocusModeFromPickerInput();
            if (URL.Contains("https")) 
            {
                ArExperience arExperience = new ArExperience(title, URL, features, position, resolution, focusMode);
                return arExperience;
            }
            else 
            {
                return null;
            }
        }

        private void UpdateFromExistingArExperience(ArExperience arExperience)
        {
            URLTextField.Text = arExperience.Path;
            titleTextField.Text = arExperience.Name;

            ImageTrackingFeatureSwitch.On = (arExperience.FeaturesMask & Features.ImageTracking) == Features.ImageTracking ? true : false;
            InstantTrackingFeatureSwitch.On = (arExperience.FeaturesMask & Features.InstantTracking) == Features.InstantTracking ? true : false;
            ObjectTrackingFeatureSwitch.On = (arExperience.FeaturesMask & Features.ObjectTracking) == Features.ObjectTracking ? true : false;
            GeoARFeatureSwitch.On = (arExperience.FeaturesMask & Features.Geo) == Features.Geo ? true : false;

            cameraSettings[0].SelectData(CameraResolutionToString(arExperience.CameraResolution));
            cameraSettings[1].SelectData(CameraPositionToString(arExperience.CameraPosition));
            cameraSettings[2].SelectData(CameraFocusModeToString(arExperience.CameraFocusMode));
        }

        private Features FeaturesFromSwitchInputs()
        {
            Features features = 0;
            if (ImageTrackingFeatureSwitch.On)
            {
                features |= Features.ImageTracking;
            }
            if (InstantTrackingFeatureSwitch.On)
            {
                features |= Features.InstantTracking;
            }
            if (ObjectTrackingFeatureSwitch.On)
            {
                features |= Features.ObjectTracking;
            }
            if (GeoARFeatureSwitch.On)
            {
                features |= Features.Geo;
            }

            return features;
        }

        private CameraResolution CameraResolutionFromPickerInput()
        {
            string cameraPositionString = cameraSettings[0].SelectedData();
            switch (cameraPositionString)
            {
                case "640x480":
                    return CameraResolution.SD_640x480;
                case "1280x720":
                    return CameraResolution.HD_1280x720;
                case "1920x1080":
                    return CameraResolution.Full_HD_1920x1080;
                default:
                    return CameraResolution.Auto;
            }
        }

        private string CameraResolutionToString(CameraResolution cameraResolution)
        {
            switch (cameraResolution)
            {
                case CameraResolution.SD_640x480:
                    return "640x480";
                case CameraResolution.HD_1280x720:
                    return "1280x720";
                case CameraResolution.Full_HD_1920x1080:
                    return "1920x1080";
                default:
                    return "Auto";
            }
        }

        private CameraPosition CameraPositionFromPickerInput()
        {
            string cameraPositionString = cameraSettings[1].SelectedData();
            switch (cameraPositionString)
            {
                case "Back":
                    return CameraPosition.Back;
                case "Front":
                    return CameraPosition.Front;
                default:
                    return CameraPosition.Default;
            }
        }

        private string CameraPositionToString(CameraPosition cameraPosition)
        {
            switch (cameraPosition)
            {
                case CameraPosition.Back:
                    return "Back";
                case CameraPosition.Front:
                    return "Front";
                default:
                    return "Default";
            }
        }

        private CameraFocusMode CameraFocusModeFromPickerInput()
        {
            string cameraFocusMode = cameraSettings[2].SelectedData();
            switch (cameraFocusMode)
            {
                case "Once":
                    return CameraFocusMode.AutofocusOnce;
                case "Continuous":
                    return CameraFocusMode.AutofocusContinuous;
                default:
                    return CameraFocusMode.AutofocusOff;
            }
        }

        private string CameraFocusModeToString(CameraFocusMode cameraFocusMode)
        {
            switch (cameraFocusMode)
            {
                case CameraFocusMode.AutofocusOnce:
                    return "Once";
                case CameraFocusMode.AutofocusContinuous:
                    return "Continuous";
                default:
                    return "Off";
            }
        }

        private CameraSetting CameraSettingForRow(int row)
        {
            if (row == 1)
            {
                return cameraSettings[0];
            }
            else if (row == 3)
            {
                return cameraSettings[1];
            }
            else if (row == 5)
            {
                return cameraSettings[2];
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}