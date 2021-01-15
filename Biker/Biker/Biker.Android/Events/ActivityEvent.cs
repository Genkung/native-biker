using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biker.Droid.Events
{
    public class ActivityEvent
    {
        public static event EventHandler<ActivtyResult> OnActivityResult;

        public static void OnActivityResult_Event(object sender, ActivtyResult result)
        {
            OnActivityResult?.Invoke(sender, result);
        }

        public class ActivtyResult
        {
            public int RequestCode { get; set; }
            public Result ResultCode { get; set; }
            public Intent Data { get; set; }
        }
    }
}