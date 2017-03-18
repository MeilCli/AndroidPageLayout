using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidSlideLayout;

namespace AndroidPageLayout {

    public class PageLayout : SlideLayout, INestedScrollingParent {

        private const string savedCurrentPageKey = "saved_current_page_key";
        private const string savedBaseStateKey = "saved_base_state_key";

        /// <summary>
        /// The constant of PageLayout scroll vertical direction 
        /// </summary>
        public const int Vertical = 0;

        /// <summary>
        /// The constant of PageLayout scroll horizontal direction
        /// </summary>
        public const int Horizontal = 1;


        /// <summary>
        /// Event of first visible page with floating point changed
        /// </summary>
        public event EventHandler<FirstVisiblePageFloatChangedEventArgs> FirstVisiblePageFloatChanged;

        /// <summary>
        /// Event of first visible page changed
        /// </summary>
        public event EventHandler<FirstVisiblePageChangedEventArgs> FirstVisiblePageChanged;

        private LinearStackLayout linearStackLayout;
        private NestedScrollingParentHelper nestedParentHelper;
        // if page orientaion touch motion and not drag in edge, so not send child touch event. 
        private bool isTouchHandled;
        // dragging point for touch event not want to be absorbed parent view
        private int draggingPoint;

        /// <summary>
        /// This layout allow multiple page
        /// </summary>
        public int MultiplePageSize {
            get {
                return linearStackLayout.MultiplePageSize;
            }
            set {
                linearStackLayout.MultiplePageSize = value;
            }
        }

        /// <summary>
        /// This layout's orientaion, page is stacked to orientation direction
        /// </summary>
        public int Orientation {
            get {
                return linearStackLayout.Orientation;
            }
            set {
                linearStackLayout.Orientation = value;
                setLinearStackLayoutParams();
            }
        }

        /// <summary>
        /// This layout' page size
        /// </summary>
        public int PageCount {
            get {
                return linearStackLayout.ChildCount;
            }
        }

        private float _currentFirstVisiblePageFloat;
        /// <summary>
        /// The number of current first visible page with floating point
        /// </summary>
        public float CurrentFirstVisiblePageFloat {
            get {
                return _currentFirstVisiblePageFloat;
            }
            private set {
                if (_currentFirstVisiblePageFloat != value) {
                    _currentFirstVisiblePageFloat = value;
                    FirstVisiblePageFloatChanged?.Invoke(this, new FirstVisiblePageFloatChangedEventArgs(value));
                }
            }
        }

        private int _currentFirstVisiblePage;
        /// <summary>
        /// The number of current first visible page
        /// </summary>
        public int CurrentFirstVisiblePage {
            get {
                return _currentFirstVisiblePage;
            }
            private set {
                if (_currentFirstVisiblePage != value) {
                    _currentFirstVisiblePage = value;
                    FirstVisiblePageChanged?.Invoke(this, new FirstVisiblePageChangedEventArgs(value));
                }
            }
        }

        /// <summary>
        /// Set default page before make layout
        /// </summary>
        public int? DefaultPage { private get; set; }

        private int distanceWidthWithOrientationPerPage {
            get {
                if (Orientation == Vertical) {
                    return 0;
                }
                return Width / MultiplePageSize;
            }
        }

        private int distanceHeightWithOrientationPerPage {
            get {
                if (Orientation == Horizontal) {
                    return 0;
                }
                return Height / MultiplePageSize;
            }
        }

        public PageLayout(Context context) : this(context, null) { }

        public PageLayout(Context context, IAttributeSet attr) : this(context, attr, 0) { }

        public PageLayout(Context context, IAttributeSet attr, int defStyle) : base(context, attr, defStyle) {
            setUpLinearStackLayout();
            var ar = context.ObtainStyledAttributes(attr, Resource.Styleable.PageLayout);
            try {
                MultiplePageSize = ar.GetInteger(Resource.Styleable.PageLayout_page_multi_size, 1);
                Orientation = ar.GetInteger(Resource.Styleable.PageLayout_page_orientation, Vertical);
            } finally {
                ar.Recycle();
            }

            nestedParentHelper = new NestedScrollingParentHelper(this);
        }

        public PageLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void setUpLinearStackLayout() {
            if (linearStackLayout != null) {
                return;
            }
            linearStackLayout = Inflate(Context, Resource.Layout.LinearStackLayout, this).FindViewById<LinearStackLayout>(Resource.Id.linear_stack_layout);
            linearStackLayout.LayoutParameters = new SlideLayout.LayoutParams();
            setLinearStackLayoutParams();
        }

        private void setLinearStackLayoutParams() {
            var layoutParameter = linearStackLayout.LayoutParameters as SlideLayout.LayoutParams;
            if (Orientation == Vertical) {
                layoutParameter.IsDraggableTopDirection = true;
                layoutParameter.IsDraggableBottomDirection = true;
                layoutParameter.IsDraggableLeftDirection = false;
                layoutParameter.IsDraggableRightDirection = false;
            } else if (Orientation == Horizontal) {
                layoutParameter.IsDraggableTopDirection = false;
                layoutParameter.IsDraggableBottomDirection = false;
                layoutParameter.IsDraggableLeftDirection = true;
                layoutParameter.IsDraggableRightDirection = true;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            if (linearStackLayout.Orientation == Vertical) {
                height = height * linearStackLayout.ChildCount;
            } else if (linearStackLayout.Orientation == Horizontal) {
                width = width * linearStackLayout.ChildCount;
            }
            int widthSpec = MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly);
            int heightSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
            linearStackLayout.Measure(widthSpec, heightSpec);
        }

        public override void OnViewAdded(View child) {
            base.OnViewAdded(child);
            if (child.LayoutParameters is LayoutParams == false) {
                return;
            }
            var parameter = child.LayoutParameters as LayoutParams;
            if (parameter.IsPage == false) {
                return;
            }
            RemoveView(child);
            linearStackLayout.AddView(child);
        }

        protected override IParcelable OnSaveInstanceState() {
            var bundle = new Bundle();
            bundle.PutInt(savedCurrentPageKey, CurrentFirstVisiblePage);
            bundle.PutParcelable(savedBaseStateKey, base.OnSaveInstanceState());
            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state) {
            if (state is Bundle) {
                var bundle = state as Bundle;
                int page = bundle.GetInt(savedCurrentPageKey);
                if (page >= 0 && page + (MultiplePageSize - 1) < PageCount) {
                    DefaultPage = page;
                }
                state = bundle.GetParcelable(savedBaseStateKey) as IParcelable;
            }
            base.OnRestoreInstanceState(state);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
            base.OnLayout(changed, left, top, right, bottom);
            if (DefaultPage != null && 0 < DefaultPage && DefaultPage + (MultiplePageSize - 1) < PageCount) {
                scrollTo(DefaultPage.Value);
            }
        }

        public override int ClampViewPositionVertical(View child, int top, int dy) {
            int result = base.ClampViewPositionVertical(child, top, dy);
            if (child == linearStackLayout && Orientation == Vertical) {
                if (result != CurrentDragChildViewDraggedTop) {
                    // if dragging, not want to be absorbed parent view
                    draggingPoint += dy;
                    if (draggingPoint > 50) {
                        Parent.RequestDisallowInterceptTouchEvent(true);
                    }
                }
                if (result + (distanceHeightWithOrientationPerPage * (PageCount - MultiplePageSize)) < 0) {
                    isTouchHandled = true;
                    return -1 * distanceHeightWithOrientationPerPage * (PageCount - MultiplePageSize);
                }
                if (result > 0) {
                    isTouchHandled = true;
                    return 0;
                }
            }
            if (isTouchHandled == false) {
                isTouchHandled = false;
            }
            return result;
        }

        public override int ClampViewPositionHorizontal(View child, int left, int dx) {
            int result = base.ClampViewPositionHorizontal(child, left, dx);
            if (child == linearStackLayout && Orientation == Horizontal) {
                if (result != CurrentDragChildViewLayoutedLeft) {
                    // if dragging, not want to be absorbed parent view
                    draggingPoint += dx;
                    if (draggingPoint > 50) {
                        Parent.RequestDisallowInterceptTouchEvent(true);
                    }
                }
                if (result + (distanceWidthWithOrientationPerPage * (PageCount - MultiplePageSize)) < 0) {
                    isTouchHandled = true;
                    return -1 * distanceWidthWithOrientationPerPage * (PageCount - MultiplePageSize);
                }
                if (result > 0) {
                    isTouchHandled = true;
                    return 0;
                }
            }
            if (isTouchHandled == false) {
                isTouchHandled = false;
            }
            return result;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev) {
            if (ev.Action == MotionEventActions.Down) {
                // if direct child view is clickable, touch event is absorbed by direct child view and will pull trigger click event.
                var foundTopChildView = findLinearStackLayoutTopChildUnder((int)ev.XPrecision, (int)ev.YPrecision);
                if (foundTopChildView?.Clickable == false) {
                    return false;
                }
            }
            return base.OnInterceptTouchEvent(ev) || isTouchHandled;
        }

        private View findLinearStackLayoutTopChildUnder(int x, int y) {
            int childCount = linearStackLayout.ChildCount;
            for (int i = childCount - 1; i >= 0; i--) {
                View child = linearStackLayout.GetChildAt(i);
                if (x >= child.Left && x < child.Right && y >= child.Top && y < child.Bottom) {
                    return child;
                }
            }
            return null;
        }

        public override void OnViewPositionChanged(View changedView, int left, int top, int dx, int dy) {
            base.OnViewPositionChanged(changedView, left, top, dx, dy);
            if (changedView == linearStackLayout) {
                CurrentFirstVisiblePageFloat = getCurrentFirstVisiblePageFloat();
                CurrentFirstVisiblePage = getCurrentFirstVisiblePage(CurrentFirstVisiblePageFloat);
            }
        }

        protected override void OnAttachedToWindow() {
            base.OnAttachedToWindow();
            ViewReleased += viewReleased;
        }

        protected override void OnDetachedFromWindow() {
            ViewReleased -= viewReleased;
            base.OnDetachedFromWindow();
        }

        private void viewReleased(object sender, ViewReleasedEventArgs args) {
            if (args.ReleasedChild != linearStackLayout) {
                return;
            }
            isTouchHandled = false;

            // when end dragging, allow to be absorbed parent view
            draggingPoint = 0;
            Parent.RequestDisallowInterceptTouchEvent(false);

            float velocity = Orientation == Vertical ? args.YVelocity : args.XVelocity;
            SmoothScroll(getCurrentFirstVisiblePage(getCurrentFirstVisiblePageFloat(), velocity));
            args.Handled = true;
        }

        private float getCurrentFirstVisiblePageFloat() {
            if (Orientation == Vertical) {
                return -1f * CurrentDragChildViewDraggedTop / distanceHeightWithOrientationPerPage;
            } else if (Orientation == Horizontal) {
                return -1f * CurrentDragChildViewDraggedLeft / distanceWidthWithOrientationPerPage;
            }
            return 0;
        }

        private int getCurrentFirstVisiblePage(float pageFloat, float velocity = 0) {
            float border = pageFloat % 1;
            int page = (int)pageFloat + (border >= 0.5f ? 1 : 0);
            if (velocity < -1000 && page + 1 < PageCount - (MultiplePageSize - 1)) {
                page += 1;
            } else if (velocity > 1000 && page - 1 >= 0) {
                page -= 1;
            }
            return page;
        }

        /// <summary>
        /// Scroll to page with smooth animation
        /// </summary>
        /// <param name="page">page of be scrolled</param>
        public void SmoothScroll(int page) {
            if (page < 0 || page >= PageCount) {
                throw new ArgumentOutOfRangeException("page out of range in PageLayout");
            }
            if (page + (MultiplePageSize - 1) >= PageCount) {
                throw new ArgumentOutOfRangeException("multiple page out of range in PageLayout");
            }
            DefaultPage = page;
            SmoothSlideViewTo(linearStackLayout, 0 + -page * distanceWidthWithOrientationPerPage, 0 + -page * distanceHeightWithOrientationPerPage);
        }

        /// <summary>
        /// Scroll to page
        /// </summary>
        /// <param name="page">page of be scrolled</param>
        public void ScrollTo(int page) {
            DefaultPage = page;
            scrollTo(page);
        }

        private void scrollTo(int page) {
            if (page < 0 || page >= PageCount) {
                throw new ArgumentOutOfRangeException("page out of range in PageLayout");
            }
            if (page + (MultiplePageSize - 1) >= PageCount) {
                throw new ArgumentOutOfRangeException("multiple page out of range in PageLayout");
            }

            int left = distanceWidthWithOrientationPerPage * page * -1;
            int top = distanceHeightWithOrientationPerPage * page * -1;
            int dx = left - linearStackLayout.Left;
            int dy = top - linearStackLayout.Top;
            if (dx != 0) {
                linearStackLayout.OffsetLeftAndRight(dx);
            }
            if (dy != 0) {
                linearStackLayout.OffsetTopAndBottom(dy);
            }

            if (dx != 0 || dy != 0) {
                OnViewPositionChanged(linearStackLayout, linearStackLayout.Left, linearStackLayout.Top, dx, dy);
            }
        }

        /// <summary>
        /// Get Page
        /// </summary>
        /// <param name="index">get page index</param>
        /// <returns>page's view</returns>
        public View GetPage(int index) {
            return linearStackLayout.GetChildAt(index);
        }

        /// <summary>
        /// Add page
        /// </summary>
        /// <param name="view">page</param>
        public void AddPageView(View view) {
            DefaultPage = CurrentFirstVisiblePage;
            linearStackLayout.AddView(view);
        }

        /// <summary>
        /// Add page with index
        /// </summary>
        /// <param name="view">page</param>
        /// <param name="index">index of be added</param>
        public void AddPageView(View view, int index) {
            DefaultPage = CurrentFirstVisiblePage;
            linearStackLayout.AddView(view);
        }

        /// <summary>
        /// Remove page
        /// </summary>
        /// <param name="view"></param>
        public void RemovePageView(View view) {
            if (linearStackLayout.IndexOfChild(view) < 0) {
                return;
            }
            if (CurrentFirstVisiblePage + (MultiplePageSize - 1) == PageCount - 1) {
                if (CurrentFirstVisiblePage > 0) {
                    ScrollTo(CurrentFirstVisiblePage - 1);
                }
            }
            DefaultPage = CurrentFirstVisiblePage;
            linearStackLayout.RemoveView(view);
        }

        /// <summary>
        /// Remove page at
        /// </summary>
        /// <param name="index">index of be removed</param>
        public void RemovePageView(int index) {
            if (index < 0 || index >= PageCount) {
                throw new ArgumentOutOfRangeException("removing page index is out of range");
            }
            if (CurrentFirstVisiblePage + (MultiplePageSize - 1) == PageCount - 1) {
                if (CurrentFirstVisiblePage > 0) {
                    ScrollTo(CurrentFirstVisiblePage - 1);
                }
            }
            DefaultPage = CurrentFirstVisiblePage;
            linearStackLayout.RemoveViewAt(index);
        }

        /// <summary>
        /// Clear all page
        /// </summary>
        public void ClearPage() {
            ScrollTo(0);
            linearStackLayout.RemoveAllViews();
        }

        // ----------------------------
        //      NestedScrollParent
        // ----------------------------

        public override ScrollAxis NestedScrollAxes => (ScrollAxis)Enum.ToObject(typeof(ScrollAxis), nestedParentHelper.NestedScrollAxes);

        public override bool OnNestedFling(View target, float velocityX, float velocityY, bool consumed) {
            return false;
        }

        public override bool OnNestedPreFling(View target, float velocityX, float velocityY) {
            return false;
        }

        public override void OnNestedPreScroll(View target, int dx, int dy, int[] consumed) {
        }

        public override void OnNestedScroll(View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed) {
            if (Orientation == Vertical) {
                linearStackLayoutDragTo(linearStackLayout.Left, linearStackLayout.Top - dyUnconsumed, 0, -dyUnconsumed);
            } else if (Orientation == Horizontal) {
                linearStackLayoutDragTo(linearStackLayout.Left + dxUnconsumed, linearStackLayout.Top, dxUnconsumed, 0);
            }
        }

        private void linearStackLayoutDragTo(int left, int top, int dx, int dy) {
            int clampedX = left;
            int clampedY = top;
            int oldLeft = linearStackLayout.Left;
            int oldTop = linearStackLayout.Top;
            if (dx != 0) {
                clampedX = ClampViewPositionHorizontal(linearStackLayout, left, dx);
                linearStackLayout.OffsetLeftAndRight(clampedX - oldLeft);
            }
            if (dy != 0) {
                clampedY = ClampViewPositionVertical(linearStackLayout, top, dy);
                linearStackLayout.OffsetTopAndBottom(clampedY - oldTop);
            }

            if (dx != 0 || dy != 0) {
                int clampedDx = clampedX - oldLeft;
                int clampedDy = clampedY - oldTop;
                OnViewPositionChanged(linearStackLayout, clampedX, clampedY, clampedDx, clampedDy);
            }
        }

        public override void OnNestedScrollAccepted(View child, View target, [GeneratedEnum] ScrollAxis axes) {
            if (child != linearStackLayout) {
                return;
            }
            nestedParentHelper.OnNestedScrollAccepted(child, target, (int)axes);
        }

        public override bool OnStartNestedScroll(View child, View target, [GeneratedEnum] ScrollAxis nestedScrollAxes) {
            if (child != linearStackLayout) {
                return false;
            }
            if (Orientation == Vertical) {
                return ((int)nestedScrollAxes & ViewCompat.ScrollAxisVertical) != 0;
            }
            if (Orientation == Horizontal) {
                return ((int)nestedScrollAxes & ViewCompat.ScrollAxisHorizontal) != 0;
            }
            // cannot reach this code point
            return false;
        }

        public override void OnStopNestedScroll(View target) {
            nestedParentHelper.OnStopNestedScroll(target);
            OnViewReleased(linearStackLayout, 0, 0);
        }

        // ----------------------------
        //        LayoutParams
        // ----------------------------

        protected override bool CheckLayoutParams(ViewGroup.LayoutParams p) {
            return p is LayoutParams;
        }

        protected override ViewGroup.LayoutParams GenerateDefaultLayoutParams() {
            return new LayoutParams();
        }

        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs) {
            return new LayoutParams(Context, attrs);
        }

        protected override ViewGroup.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams p) {
            if (p is LayoutParams) {
                return new LayoutParams(p as LayoutParams);
            } else if (p is SlideLayout.LayoutParams) {
                return new LayoutParams(p as SlideLayout.LayoutParams);
            } else if (p is FrameLayoutCompat.LayoutParams) {
                return new LayoutParams(p as FrameLayoutCompat.LayoutParams);
            } else if (p is MarginLayoutParams) {
                return new LayoutParams(p as MarginLayoutParams);
            }
            return new LayoutParams(p);
        }

        public new class LayoutParams : SlideLayout.LayoutParams {

            public bool IsPage { get; }

            public LayoutParams() : base() { }

            public LayoutParams(Context context, IAttributeSet attr) : base(context, attr) {
                var ar = context.ObtainStyledAttributes(attr, Resource.Styleable.PageLayout);
                try {
                    IsPage = ar.GetBoolean(Resource.Styleable.PageLayout_page_view, false);
                } finally {
                    ar.Recycle();
                }
            }

            public LayoutParams(ViewGroup.LayoutParams source) : base(source) { }

            public LayoutParams(MarginLayoutParams souce) : base(souce) { }

            public LayoutParams(FrameLayout.LayoutParams source) : base(source) { }

            public LayoutParams(SlideLayout.LayoutParams source) : base(source) { }

            public LayoutParams(LayoutParams source) : base(source) {
                IsPage = source.IsPage;
            }

        }

    }
}