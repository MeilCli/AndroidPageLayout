using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AndroidPageLayout.App {

    [Activity(Label = "SimpleMultiplePageActivity", Theme = "@style/AppTheme")]
    public class SimpleMultiplePageActivity : AppCompatActivity {

        private const string label = nameof(SimpleMultiplePageActivity);

        private PageLayout pageLayout;

        public static void Start(Activity activity) {
            var intent = new Intent(activity, typeof(SimpleMultiplePageActivity));
            activity.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SimpleMultiplePage);

            pageLayout = FindViewById<PageLayout>(Resource.Id.PageLayout);
            pageLayout.FirstVisiblePageFloatChanged += currentFirstVisiblePageFloatChanged;

            setTitle(pageLayout.CurrentFirstVisiblePageFloat);

            using (var page = FindViewById<TextView>(Resource.Id.Page1)) {
                page.Click += click;
            }
            using (var page = FindViewById<TextView>(Resource.Id.Page2)) {
                page.Click += click;
            }
            using (var page = FindViewById<TextView>(Resource.Id.Page3)) {
                page.Click += click;
            }
            using (var page = FindViewById<TextView>(Resource.Id.Page4)) {
                page.Click += click;
            }
        }

        protected override void OnDestroy() {
            using (var page = FindViewById<TextView>(Resource.Id.Page1)) {
                page.Click -= click;
            }
            using (var page = FindViewById<TextView>(Resource.Id.Page2)) {
                page.Click -= click;
            }
            using (var page = FindViewById<TextView>(Resource.Id.Page3)) {
                page.Click -= click;
            }
            using (var page = FindViewById<TextView>(Resource.Id.Page4)) {
                page.Click -= click;
            }
            pageLayout.FirstVisiblePageFloatChanged -= currentFirstVisiblePageFloatChanged;
            pageLayout.Dispose();
            base.OnDestroy();
        }

        private void click(object sender, EventArgs args) {
            if (sender is TextView textView) {
                Toast.MakeText(this, $"Clicked TextView: {textView.Text}", ToastLength.Short).Show();
            }
        }

        private void currentFirstVisiblePageFloatChanged(object sender, FirstVisiblePageFloatChangedEventArgs args) {
            setTitle(args.FirstVisiblePageFloat);
        }

        private void setTitle(float currentFirstVisiblePageFloat) {
            SupportActionBar.Title = $"{label} Page:{currentFirstVisiblePageFloat}";
        }
    }
}