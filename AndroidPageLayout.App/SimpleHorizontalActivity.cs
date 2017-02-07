using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace AndroidPageLayout.App {

    [Activity(Label = "SimpleHorizontalActivity",Theme = "@style/AppTheme")]
    public class SimpleHorizontalActivity : AppCompatActivity {

        private const string label = nameof(SimpleHorizontalActivity);

        private PageLayout pageLayout;

        public static void Start(Activity activity) {
            var intent = new Intent(activity,typeof(SimpleHorizontalActivity));
            activity.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SimpleHorizontal);

            pageLayout = FindViewById<PageLayout>(Resource.Id.PageLayout);
            pageLayout.FirstVisiblePageFloatChanged += currentFirstVisiblePageFloatChanged;

            setTitle(pageLayout.CurrentFirstVisiblePageFloat);

            using(var page = FindViewById<TextView>(Resource.Id.Page1)) {
                page.Click += click;
            }
            using(var page = FindViewById<TextView>(Resource.Id.Page2)) {
                page.Click += click;
            }
            using(var page = FindViewById<TextView>(Resource.Id.Page3)) {
                page.Click += click;
            }
            using(var page = FindViewById<TextView>(Resource.Id.Page4)) {
                page.Click += click;
            }
        }

        protected override void OnDestroy() {
            using(var page = FindViewById<TextView>(Resource.Id.Page1)) {
                page.Click -= click;
            }
            using(var page = FindViewById<TextView>(Resource.Id.Page2)) {
                page.Click -= click;
            }
            using(var page = FindViewById<TextView>(Resource.Id.Page3)) {
                page.Click -= click;
            }
            using(var page = FindViewById<TextView>(Resource.Id.Page4)) {
                page.Click -= click;
            }
            pageLayout.FirstVisiblePageFloatChanged -= currentFirstVisiblePageFloatChanged;
            pageLayout.Dispose();
            base.OnDestroy();
        }

        private void click(object sender,EventArgs args) {
            var textView = sender as TextView;
            if(textView != null) {
                Toast.MakeText(this,$"Clicked TextView: {textView.Text}",ToastLength.Short).Show();
            }
        }

        private void currentFirstVisiblePageFloatChanged(object sender,FirstVisiblePageFloatChangedEventArgs args) {
            setTitle(args.FirstVisiblePageFloat);
        }

        private void setTitle(float currentFirstVisiblePageFloat) {
            SupportActionBar.Title = $"{label} Page:{currentFirstVisiblePageFloat}";
        }
    }
}