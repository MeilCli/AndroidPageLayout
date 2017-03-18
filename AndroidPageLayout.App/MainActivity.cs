using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace AndroidPageLayout.App {

    [Activity(Label = "AndroidPageLayout.App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {

        private const string label = nameof(MainActivity);

        private PageLayout pageLayout;

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            pageLayout = FindViewById<PageLayout>(Resource.Id.PageLayout);
            pageLayout.FirstVisiblePageFloatChanged += currentFirstVisiblePageFloatChanged;
            if (pageLayout == null) {
                throw new Exception("pagelayout is null");
            }
            try {
                var f = pageLayout.CurrentFirstVisiblePageFloat;
            } catch (Exception) {
                throw new Exception("pagelayout.current null");
            }
            setTitle(pageLayout.CurrentFirstVisiblePageFloat);

            using (var page = FindViewById<View>(Resource.Id.SimpleVertical)) {
                page.Click += click;
            }
            using (var page = FindViewById<View>(Resource.Id.SimpleHorizontal)) {
                page.Click += click;
            }
            using (var page = FindViewById<View>(Resource.Id.SimpleNestedScrollingParent)) {
                page.Click += click;
            }
            using (var page = FindViewById<View>(Resource.Id.SimpleMultiplePage)) {
                page.Click += click;
            }
            using (var page = FindViewById<View>(Resource.Id.DynamicAddOrRemovePage)) {
                page.Click += click;
            }

            // TwitterLikeImageViewer
            using (var image = FindViewById<ImageView>(Resource.Id.Image1)) {
                image.Click += click;
            }
            using (var image = FindViewById<ImageView>(Resource.Id.Image2)) {
                image.Click += click;
            }
            using (var image = FindViewById<ImageView>(Resource.Id.Image3)) {
                image.Click += click;
            }
            using (var image = FindViewById<ImageView>(Resource.Id.Image4)) {
                image.Click += click;
            }
        }

        protected override void OnDestroy() {
            using (var page = FindViewById<View>(Resource.Id.SimpleVertical)) {
                page.Click -= click;
            }
            using (var page = FindViewById<View>(Resource.Id.SimpleHorizontal)) {
                page.Click -= click;
            }
            using (var page = FindViewById<View>(Resource.Id.SimpleNestedScrollingParent)) {
                page.Click -= click;
            }
            using (var page = FindViewById<View>(Resource.Id.SimpleMultiplePage)) {
                page.Click -= click;
            }
            using (var page = FindViewById<View>(Resource.Id.DynamicAddOrRemovePage)) {
                page.Click -= click;
            }

            // TwitterLikeImageViewer
            using (var image = FindViewById<ImageView>(Resource.Id.Image1)) {
                image.Click -= click;
            }
            using (var image = FindViewById<ImageView>(Resource.Id.Image2)) {
                image.Click -= click;
            }
            using (var image = FindViewById<ImageView>(Resource.Id.Image3)) {
                image.Click -= click;
            }
            using (var image = FindViewById<ImageView>(Resource.Id.Image4)) {
                image.Click -= click;
            }

            pageLayout.FirstVisiblePageFloatChanged -= currentFirstVisiblePageFloatChanged;
            pageLayout.Dispose();
            base.OnDestroy();
        }

        private void click(object sender, EventArgs args) {
            var view = sender as View;
            switch (view.Id) {
                case Resource.Id.SimpleVertical:
                    SimpleVerticalActivity.Start(this);
                    break;
                case Resource.Id.SimpleHorizontal:
                    SimpleHorizontalActivity.Start(this);
                    break;
                case Resource.Id.SimpleNestedScrollingParent:
                    SimpleNestedScrollingParent.Start(this);
                    break;
                case Resource.Id.SimpleMultiplePage:
                    SimpleMultiplePageActivity.Start(this);
                    break;
                case Resource.Id.DynamicAddOrRemovePage:
                    DynamicAddOrRemovePageActivity.Start(this);
                    break;

                // TwitterLikeImageViewer
                case Resource.Id.Image1:
                    TwitterLikeImageViewerActivity.Start(this, view as ImageView, 0);
                    break;
                case Resource.Id.Image2:
                    TwitterLikeImageViewerActivity.Start(this, view as ImageView, 1);
                    break;
                case Resource.Id.Image3:
                    TwitterLikeImageViewerActivity.Start(this, view as ImageView, 2);
                    break;
                case Resource.Id.Image4:
                    TwitterLikeImageViewerActivity.Start(this, view as ImageView, 3);
                    break;
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

