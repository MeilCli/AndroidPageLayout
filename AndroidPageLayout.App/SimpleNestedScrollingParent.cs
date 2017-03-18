using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AndroidPageLayout.App {

    [Activity(Label = "SimpleNestedScrollingParent", Theme = "@style/AppTheme")]
    public class SimpleNestedScrollingParent : AppCompatActivity {

        private const string label = nameof(SimpleNestedScrollingParent);

        private PageLayout pageLayout;

        public static void Start(Activity activity) {
            var intent = new Intent(activity, typeof(SimpleNestedScrollingParent));
            activity.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SimpleNestedScrollingParent);

            pageLayout = FindViewById<PageLayout>(Resource.Id.PageLayout);
            pageLayout.FirstVisiblePageFloatChanged += currentFirstVisiblePageFloatChanged;

            setTitle(pageLayout.CurrentFirstVisiblePageFloat);

            using (var item = FindViewById<TextView>(Resource.Id.Item1)) {
                item.Click += click;
            }
            using (var item = FindViewById<TextView>(Resource.Id.Item2)) {
                item.Click += click;
            }
            using (var item = FindViewById<TextView>(Resource.Id.Item3)) {
                item.Click += click;
            }
            using (var item = FindViewById<TextView>(Resource.Id.Item4)) {
                item.Click += click;
            }
        }

        protected override void OnDestroy() {
            using (var item = FindViewById<TextView>(Resource.Id.Item1)) {
                item.Click -= click;
            }
            using (var item = FindViewById<TextView>(Resource.Id.Item2)) {
                item.Click -= click;
            }
            using (var item = FindViewById<TextView>(Resource.Id.Item3)) {
                item.Click -= click;
            }
            using (var item = FindViewById<TextView>(Resource.Id.Item4)) {
                item.Click -= click;
            }
            pageLayout.FirstVisiblePageFloatChanged -= currentFirstVisiblePageFloatChanged;
            pageLayout.Dispose();
            base.OnDestroy();
        }

        private void click(object sender, EventArgs args) {
            var textView = sender as TextView;
            if (textView != null) {
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