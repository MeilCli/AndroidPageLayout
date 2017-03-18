using System;

namespace AndroidPageLayout {

    /// <summary>
    /// Event of first visible page with floating point changed
    /// </summary>
    public class FirstVisiblePageFloatChangedEventArgs : EventArgs {

        /// <summary>
        /// The number of first visible page with floting point
        /// </summary>
        public float FirstVisiblePageFloat { get; }

        public FirstVisiblePageFloatChangedEventArgs(float firstVisiblePageFloat) {
            FirstVisiblePageFloat = firstVisiblePageFloat;
        }
    }

    /// <summary>
    /// Event of first visible page changed
    /// </summary>
    public class FirstVisiblePageChangedEventArgs : EventArgs {

        /// <summary>
        /// The number of first visible page
        /// </summary>
        public int FirstVisiblePage { get; }

        public FirstVisiblePageChangedEventArgs(int firstVisiblePage) {
            FirstVisiblePage = firstVisiblePage;
        }
    }
}