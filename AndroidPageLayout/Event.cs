using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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