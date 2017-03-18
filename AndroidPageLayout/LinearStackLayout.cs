using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace AndroidPageLayout {

    /// <summary>
    /// This Layout is alike LinearLayout and stack MatchParent to orientation direction
    /// </summary>
    public class LinearStackLayout : ViewGroup {

        public const int Vertical = 0;
        public const int Horizontal = 1;

        private int _multiplePageSize = 1;
        public int MultiplePageSize {
            get => _multiplePageSize;
            set {
                if (value < 0) {
                    throw new NotSupportedException("not support less thann 0 page size");
                }
                _multiplePageSize = value;
                RequestLayout();
            }
        }

        private int _orientation = Vertical;
        public int Orientation {
            get => _orientation;
            set {
                if (value != Vertical && value != Horizontal) {
                    throw new NotSupportedException("not support undefined orientation");
                }
                _orientation = value;
                RequestLayout();
            }
        }

        public LinearStackLayout(Context context) : base(context) { }

        public LinearStackLayout(Context context, IAttributeSet attr) : base(context, attr) { }

        public LinearStackLayout(Context context, IAttributeSet attr, int defStyle) : base(context, attr, defStyle) { }

        public LinearStackLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            int measureWidth = MeasureSpec.GetSize(widthMeasureSpec);
            int measureHeight = MeasureSpec.GetSize(heightMeasureSpec);
            var measureWidthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var measureHeightMode = MeasureSpec.GetMode(heightMeasureSpec);

            int width = 0;
            int height = 0;
            int childState = 0;

            for (int i = 0; i < ChildCount; i++) {
                var child = GetChildAt(i);
                if (child.Visibility != ViewStates.Gone) {
                    int childWidth = measureWidth;
                    int childHeight = measureHeight;
                    var childWidthMode = measureWidthMode;
                    var childHeightMode = measureHeightMode;
                    if (Orientation == Vertical) {
                        childHeight = (measureHeight / ChildCount) / MultiplePageSize;
                        childHeightMode = MeasureSpecMode.Exactly;
                    } else if (Orientation == Horizontal) {
                        childWidth = (measureWidth / ChildCount) / MultiplePageSize;
                        childWidthMode = MeasureSpecMode.Exactly;
                    }
                    child.Measure(MeasureSpec.MakeMeasureSpec(childWidth, childWidthMode), MeasureSpec.MakeMeasureSpec(childHeight, childHeightMode));
                    if (Orientation == Vertical) {
                        width = Math.Max(width, child.MeasuredWidth);
                        height += child.MeasuredHeight;
                    } else if (Orientation == Horizontal) {
                        width += child.MeasuredWidth;
                        height = Math.Max(height, child.MeasuredHeight);
                    }
                    childState = CombineMeasuredStates(childState, child.MeasuredState);
                }
            }

            SetMeasuredDimension(ResolveSizeAndState(width, widthMeasureSpec, childState), ResolveSizeAndState(height, heightMeasureSpec, childState));
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b) {
            int currentTop = 0;
            int currentLeft = 0;

            for (int i = 0; i < ChildCount; i++) {
                var child = GetChildAt(i);
                if (child.Visibility != ViewStates.Gone) {
                    child.Layout(currentLeft, currentTop, currentLeft + child.MeasuredWidth, currentTop + child.MeasuredHeight);
                    if (Orientation == Vertical) {
                        currentTop += child.MeasuredHeight;
                    } else if (Orientation == Horizontal) {
                        currentLeft += child.MeasuredWidth;
                    }
                }
            }
        }

    }
}