using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace XamarinExampleApp.Droid.Util.adapters
{
    public class ArExpandableListAdapter : BaseExpandableListAdapter
    {
        private readonly Context context;
        private readonly List<ArExperienceGroup> experienceGroups;

        public ArExpandableListAdapter(Context context, List<ArExperienceGroup> experienceGroups)
        {
            this.context = context;
            this.experienceGroups = experienceGroups;
        }

        public override int GroupCount => experienceGroups.Count;

        public override int GetChildrenCount(int groupPosition) => experienceGroups[groupPosition].ArExperiences.Count;

        public override Java.Lang.Object GetGroup(int groupPosition) => null;

        public override bool HasStableIds => false;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition) => null;

        public override long GetChildId(int groupPosition, int childPosition) => childPosition;

        public override bool IsChildSelectable(int groupPosition, int childPosition) => true;

        public override long GetGroupId(int groupPosition) => groupPosition;

        public override View GetGroupView(int groupPosition, bool isExpanded, View view, ViewGroup parent)
        {
            if (view == null)
            {
                var inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Resource.Layout.Expand_header, null);
            }

            var groupText = view.FindViewById(Resource.Id.expand_text) as TextView;
            groupText.Text = experienceGroups[groupPosition].Name;

            if (isExpanded) 
            {
                view.SetBackgroundColor(context.Resources.GetColor(Resource.Color.wikitude_primary));
            } 
            else 
            {
                view.SetBackgroundColor(context.Resources.GetColor(Resource.Color.wikitude_white));
            }

            return view;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View view, ViewGroup parent)
        {
            if (view == null)
            {
                var inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Resource.Layout.Expand_row, null);
            }

            var experienceText = view.FindViewById(Resource.Id.expand_row_text) as TextView;
            experienceText.Text = experienceGroups[groupPosition].ArExperiences[childPosition].Name;

            return view;
        }

    }
}
