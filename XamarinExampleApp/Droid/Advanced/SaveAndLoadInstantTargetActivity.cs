using System.IO;
using Android.App;
using Android.OS;
using Org.Json;
using Com.Wikitude.Architect;

namespace XamarinExampleApp.Droid.Advanced
{
    [Activity(Label = "SaveAndLoadInstantTargetActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SaveAndLoadInstantTargetActivity : SimpleArActivity, IArchitectJavaScriptInterfaceListener
    {
        private string instantTargetSaveFile;
        private string savedAugmentationsFile;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            instantTargetSaveFile = Path.Combine(GetExternalFilesDir(null).AbsolutePath, "SavedInstantTarget.wto");
            savedAugmentationsFile = Path.Combine(GetExternalFilesDir(null).AbsolutePath, "SavedAugmentations.json");

            /*
            * The ArchitectJavaScriptInterfaceListener has to be added to the Architect view after ArchitectView.onCreate.
            * There may be more than one ArchitectJavaScriptInterfaceListener.
            */
            architectView.AddArchitectJavaScriptInterfaceListener(this);
        }

        protected override void OnDestroy()
        {
            // The ArchitectJavaScriptInterfaceListener has to be removed from the Architect view before ArchitectView.onDestroy.
            architectView.RemoveArchitectJavaScriptInterfaceListener(this);
            base.OnDestroy();
        }

        public void OnJSONObjectReceived(JSONObject jsonObject)
        {
            switch(jsonObject.GetString("action"))
            {
                case "save_current_instant_target":
                    SaveAugmentations(jsonObject.GetString("augmentations"));
                    SaveCurrentInstantTarget();
                    break;
                case "load_existing_instant_target":
                    LoadExistingInstantTarget();
                    break;
            }
        }

        private void SaveCurrentInstantTarget()
        {
            architectView.CallJavascript("World.saveCurrentInstantTargetToUrl(\"" + instantTargetSaveFile + "\")");
        }

        private void LoadExistingInstantTarget()
        {
            var javaScriptFunction = LoadAugmentations(out string augmentations)
                ? "World.loadExistingInstantTargetFromUrl(\"" + instantTargetSaveFile + "\", " + augmentations + ")"
                : "World.onError(\"Could not load saved augmentations, try saving an instant target first.\")";

            architectView.CallJavascript(javaScriptFunction);
        }

        private void SaveAugmentations(string data)
        {
            File.WriteAllText(savedAugmentationsFile, data);
        }

        private bool LoadAugmentations(out string fileContent)
        {
            if (File.Exists(savedAugmentationsFile))
            {
                fileContent = File.ReadAllText(savedAugmentationsFile);
                return true;
            }
            fileContent = null;
            return false;
        }
    }
}
