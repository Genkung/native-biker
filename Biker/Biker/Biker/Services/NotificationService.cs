using Biker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Biker.Services
{
    public class NotificationService
    {
        public Dictionary<string, object> NotificatiionStack = new Dictionary<string, object>();

        public object GetParamsInNotificationStack(string notiChannel)
        {
            object returnCallback;
            NotificatiionStack.TryGetValue(notiChannel, out returnCallback);
            return returnCallback;
        }

        public void PublishNotification(string notiChannel, object param)
        {
            AddNotificationStack(notiChannel, param);

            if (NotificatiionStack.ContainsKey(notiChannel))
            {
                var sendNoti = new PublishNotificationModel
                {
                    NotiKey = notiChannel,
                    Params = NotificatiionStack.GetValueOrDefault(notiChannel)
                };
                MessagingCenter.Send(this, MessagingChannel.SendNotification, sendNoti);
            }
        }

        public void SubscriptNotification(Action<NotificationService, PublishNotificationModel> callback)
        {
            MessagingCenter.Subscribe<NotificationService, PublishNotificationModel>(this, MessagingChannel.SendNotification, callback);
        }

        private void AddNotificationStack(string notiChannel, object param)
        {
            var aleadyAddNotification = NotificatiionStack.TryAdd(notiChannel, param);
            if (!aleadyAddNotification)
            {
                NotificatiionStack.Remove(notiChannel);
                NotificatiionStack.TryAdd(notiChannel, param);
            }
        }
    }
}
