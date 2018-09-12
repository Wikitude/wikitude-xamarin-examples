
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using static Android.Widget.TextView;

namespace XamarinExampleApp.Droid.Advanced
{
    [Activity(Label = "SamplePoiDetailActivity")]
    public class SamplePoiDetailActivity : Activity
    {
        public static readonly string extrasKeyPoiId = "id";
        public static readonly string extrasKeyPoiTitle = "title";
        public static readonly string extrasKeyPoiDescription = "description";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_poidetail);

            var extras = Intent.Extras;
            ((TextView)FindViewById(Resource.Id.poi_detail_id_field_text_view)).SetText(extras.GetString(extrasKeyPoiId), BufferType.Normal);
            ((TextView)FindViewById(Resource.Id.poi_detail_name_field_text_view)).SetText(extras.GetString(extrasKeyPoiTitle), BufferType.Normal);
            ((TextView)FindViewById(Resource.Id.poi_detail_description_field_text_view)).SetText(extras.GetString(extrasKeyPoiDescription), BufferType.Normal);
        }
    }
}
