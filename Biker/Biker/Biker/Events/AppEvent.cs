using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Biker.Events
{
    public class AppEvent
    {
        public static event EventHandler<Page> OnAppResume;

        public static void Resume(object sender, Page e)
        {
            OnAppResume?.Invoke(sender, e);
        }
    }
}
