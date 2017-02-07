using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using AndroidSlideLayout;

namespace AndroidPageLayout.App {

    [Activity(Label = "TwitterLikeImageViewerActivity",Theme = "@style/AppTheme")]
    public class TwitterLikeImageViewerActivity : AppCompatActivity {

        private const string transitionName = "transitionactivity_transition";
        private const string extraPage = "extra_page";

        private SlideLayout slideLayout;
        private PageLayout pageLayout;

        public static void Start(Activity activity,ImageView imageView,int page) {
            var intent = new Intent(activity,typeof(TwitterLikeImageViewerActivity));
            intent.PutExtra(extraPage,page);
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                var option = ActivityOptions.MakeSceneTransitionAnimation(activity,imageView,transitionName);
                activity.StartActivity(intent,option.ToBundle());
            } else {
                activity.StartActivity(intent);
            }
        }

        /// <summary>
        /// Convert dp to px
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private int convertDensityIndependentPixelToPixel(float dp) {
            var metrics = Resources.DisplayMetrics;
            return (int)(dp * ((int)metrics.DensityDpi / 160f));
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TwitterLikeImageViewer);

            slideLayout = FindViewById<SlideLayout>(Resource.Id.SlideLayout);
            slideLayout.ViewReleased += viewReleased;

            pageLayout = FindViewById<PageLayout>(Resource.Id.PageLayout);
            pageLayout.FirstVisiblePageChanged += currentFirstVisiblePageChanged;

            int page = Intent.GetIntExtra(extraPage,0);
            pageLayout.DefaultPage = page;
            setTitle(page);

            using(var imageView = FindViewById<ImageView>(Resource.Id.Image1)) {
                if(page == 0) {
                    ViewCompat.SetTransitionName(imageView,transitionName);
                }
            }
            using(var imageView = FindViewById<ImageView>(Resource.Id.Image2)) {
                if(page == 1) {
                    ViewCompat.SetTransitionName(imageView,transitionName);
                }
            }
            using(var imageView = FindViewById<ImageView>(Resource.Id.Image3)) {
                if(page == 2) {
                    ViewCompat.SetTransitionName(imageView,transitionName);
                }
            }
            using(var imageView = FindViewById<ImageView>(Resource.Id.Image4)) {
                if(page == 3) {
                    ViewCompat.SetTransitionName(imageView,transitionName);
                }
            }
        }

        protected override void OnDestroy() {
            slideLayout.ViewReleased -= viewReleased;
            slideLayout.Dispose();
            pageLayout.FirstVisiblePageChanged -= currentFirstVisiblePageChanged;
            pageLayout.Dispose();
            base.OnDestroy();
        }

        private void currentFirstVisiblePageChanged(object sender,FirstVisiblePageChangedEventArgs args) {
            setTitle(args.FirstVisiblePage);
        }

        private void setTitle(int currentFirstVisiblePage) {
            SupportActionBar.Title = $"{currentFirstVisiblePage+1}/{pageLayout.PageCount}";
        }

        private void viewReleased(object sender,ViewReleasedEventArgs args) {
            var slideLayout = sender as SlideLayout;
            int distance = Math.Abs(slideLayout.CurrentDragChildViewLayoutedTop - slideLayout.CurrentDragChildViewDraggedTop);
            int finishDistance = convertDensityIndependentPixelToPixel(150);
            if(distance > finishDistance) {
                args.Handled = true;
                ActivityCompat.FinishAfterTransition(this);
            }
        }
    }
}