using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using XamarinExampleApp.Droid.Util.Urllauncher;

namespace XamarinExampleApp.Droid.Util.Adapters
{
    public class UrlLauncherStorageListAdapter : BaseAdapter
    {

        private readonly Context context;
        private readonly ArExperienceGroup experienceGroup;

        public UrlLauncherStorageListAdapter(Context context, ArExperienceGroup experienceGroup)
        {
            this.context = context;
            this.experienceGroup = experienceGroup;
        }

        public override int Count => experienceGroup.ArExperiences.Count;

        public override Java.Lang.Object GetItem(int position) => null;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                convertView = inflater.Inflate(Resource.Layout.Url_launcher_list_row, null);
            }

            var experience = experienceGroup.ArExperiences[position];

            var name = convertView.FindViewById(Resource.Id.url_list_row_name) as TextView;
            name.Text = experience.Name;

            var editView = convertView.FindViewById(Resource.Id.url_list_row_edit);
            editView.Click += (sender, e) => 
            {
                var intent = new Intent(context, typeof(UrlLauncherSettingsActivity));
                intent.PutExtra(UrlLauncherStorageActivity.UrlLauncherExperienceGroup, ArExperienceGroup.Serialize(experienceGroup));
                intent.PutExtra(UrlLauncherStorageActivity.UrlLauncherEditExperienceId, position);
                context.StartActivity(intent);
            };

            var deleteView = convertView.FindViewById(Resource.Id.url_list_row_delete);
            deleteView.Click += (sender, e) =>
            {
                experienceGroup.ArExperiences.RemoveAt(position);
                NotifyDataSetChanged();
            };
            return convertView;
        }
    }
}
