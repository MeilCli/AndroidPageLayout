using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace AndroidPageLayout.App {

    [Activity(Label = "DynamicAddOrRemovePageActivity", Theme = "@style/AppTheme")]
    public class DynamicAddOrRemovePageActivity : AppCompatActivity {

        private const string label = nameof(DynamicAddOrRemovePageActivity);

        private PageLayout pageLayout;
        private TextView pageCount;

        public static void Start(Activity activity) {
            var intent = new Intent(activity, typeof(DynamicAddOrRemovePageActivity));
            activity.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DynamicAddOrRemovePage);

            pageLayout = FindViewById<PageLayout>(Resource.Id.PageLayout);
            pageLayout.FirstVisiblePageFloatChanged += currentFirstVisiblePageFloatChanged;
            pageCount = FindViewById<TextView>(Resource.Id.PageCount);

            setTitle(pageLayout.CurrentFirstVisiblePageFloat);
            setPageCount();

            using (var button = FindViewById<Button>(Resource.Id.AddPage)) {
                button.Click += click;
            }
            using (var button = FindViewById<Button>(Resource.Id.RemovePage)) {
                button.Click += click;
            }
            using (var button = FindViewById<Button>(Resource.Id.ClearPage)) {
                button.Click += click;
            }
        }

        protected override void OnDestroy() {
            using (var button = FindViewById<Button>(Resource.Id.AddPage)) {
                button.Click -= click;
            }
            using (var button = FindViewById<Button>(Resource.Id.RemovePage)) {
                button.Click -= click;
            }
            using (var button = FindViewById<Button>(Resource.Id.ClearPage)) {
                button.Click -= click;
            }
            pageLayout.FirstVisiblePageFloatChanged -= currentFirstVisiblePageFloatChanged;
            pageLayout.Dispose();
            pageCount.Dispose();
            base.OnDestroy();
        }

        private void click(object sender, EventArgs args) {
            View view = sender as View;
            switch (view.Id) {
                case Resource.Id.AddPage:
                    pageLayout.AddPageView(createNewPage());
                    break;
                case Resource.Id.RemovePage:
                    pageLayout.RemovePageView(pageLayout.PageCount - 1);
                    break;
                case Resource.Id.ClearPage:
                    pageLayout.ClearPage();
                    break;
            }
            setPageCount();
        }

        private View createNewPage() {
            var textView = new TextView(this) {
                Text = $"Page{pageLayout.PageCount + 1}",
                Gravity = GravityFlags.Center
            };
            if (pageLayout.PageCount % 2 == 0) {
                textView.SetBackgroundColor(Color.Aqua);
            } else {
                textView.SetBackgroundColor(Color.Blue);
            }
            return textView;
        }

        private void setPageCount() {
            pageCount.Text = $"Count:{pageLayout.PageCount}";
        }

        private void currentFirstVisiblePageFloatChanged(object sender, FirstVisiblePageFloatChangedEventArgs args) {
            setTitle(args.FirstVisiblePageFloat);
        }

        private void setTitle(float currentFirstVisiblePageFloat) {
            SupportActionBar.Title = $"{label} Page:{currentFirstVisiblePageFloat}";
        }
    }
}