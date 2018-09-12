using System;
using System.IO;
using Foundation;
using WikitudeComponent.iOS;

namespace XamarinExampleApp.iOS.AdvancedViewControllers
{
    public partial class SaveAndLoadInstantTargetViewController : ArExperienceViewController
    {
        private string instantTargetFilePath;
        private string augmentationsDefinitionFilePath;

        public SaveAndLoadInstantTargetViewController(ArExperience arExperience) : base(arExperience)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if ( !Directory.Exists(documents) ) {
                Directory.CreateDirectory(documents);
            }

            instantTargetFilePath = Path.Combine(documents, "current_instant_target.wto");
            augmentationsDefinitionFilePath = Path.Combine(documents, "current_augmentations.json");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            architectView.ReceivedJSONObject += (object sender, ArchitectViewReceivedJSONObjectEventArgs e) =>
            {
                string action = e.JsonObject.ObjectForKey(new NSString("action")).ToString();
                if (action == "save_current_instant_target")
                {
                    SaveCurrentInstantTarget(instantTargetFilePath);
                    SaveAugmentations(augmentationsDefinitionFilePath, e.JsonObject.ObjectForKey(new NSString("augmentations")).ToString());
                }
                else if (action == "load_existing_instant_target")
                {
                    LoadExistingInstantTarget(instantTargetFilePath, augmentationsDefinitionFilePath);
                }
            };
        }

        #region Private Methods
        private void SaveCurrentInstantTarget(string filePath)
        {
            architectView.CallJavaScript(new NSString("World.saveCurrentInstantTargetToUrl(\"" + filePath + "\")"));
        }

        private void LoadExistingInstantTarget(string existingInstantTargetFilePath, string augmentationsDefinitionsFilePath)
        {
            NSString javaScriptFunction = LoadAugmentations(augmentationsDefinitionsFilePath, out string augmentations)
                ? new NSString("World.loadExistingInstantTargetFromUrl(\"" + existingInstantTargetFilePath + "\", " + augmentations + ")")
                : new NSString("World.onError(\"Could not load saved augmentations, try saving an instant target first.\")");

            architectView.CallJavaScript(javaScriptFunction);
        }

        private void SaveAugmentations(string filePath, string data)
        {
            File.WriteAllText(filePath, data);
        }

        private bool LoadAugmentations(string filePath, out string fileContent)
        {
            if (File.Exists(filePath))
            {
                fileContent = File.ReadAllText(filePath);
                return true;
            }
            fileContent = null;
            return false;
        }
        #endregion
    }
}

