using Biker.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Biker.Services
{
    public class NotificationService
    {
        public static NotificationService obj = new NotificationService();
        public static Dictionary<string, object> NotificatiionStack = new Dictionary<string, object>();

        public static void ExecuteNotificationIfExist(string notiChannel)
        {
            object returnParam;
            var notiIsExits = NotificatiionStack.TryGetValue(notiChannel, out returnParam);
            if (notiIsExits)
            {
                PublishNotification(notiChannel);
            }
        }

        public static void RemoveNotificationStack(string notiChannel)
        {
            if (NotificatiionStack.ContainsKey(notiChannel))
            {
                NotificatiionStack.Remove(notiChannel);
            }
        }

        public static void PublishNotification(string notiChannel)
        {
            if (NotificatiionStack.ContainsKey(notiChannel))
            {
                var sendNoti = new PublishNotificationModel
                {
                    NotiKey = notiChannel,
                    Params = NotificatiionStack.GetValueOrDefault(notiChannel)
                };
                MessagingCenter.Send(obj, MessagingChannel.SendNotification, sendNoti);
            }
        }

        public static void SubscriptNotification(Action<NotificationService, PublishNotificationModel> callback)
        {
            MessagingCenter.Subscribe<NotificationService, PublishNotificationModel>(obj, MessagingChannel.SendNotification, callback);
        }

        public static void UnSubscriptNotification() 
        {
            MessagingCenter.Unsubscribe<NotificationService, PublishNotificationModel>(obj, MessagingChannel.SendNotification);
        }

        public static void AddNotificationStack(string notiChannel, object param)
        {
            var aleadyAddNotification = !NotificatiionStack.TryAdd(notiChannel, param);
            if (aleadyAddNotification)
            {
                NotificatiionStack.Remove(notiChannel);
                NotificatiionStack.TryAdd(notiChannel, param);
            }
        }
    }
}
