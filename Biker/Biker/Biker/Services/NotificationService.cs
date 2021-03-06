﻿using Biker.Models;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
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

        public static void PublishNotification(string notiChannel)
        {
            if (NotificatiionStack.ContainsKey(notiChannel))
            {
                var sendNoti = new PublishNotificationModel
                {
                    NotiChannel = notiChannel,
                    Params = NotificatiionStack.GetValueOrDefault(notiChannel)
                };
                MessagingCenter.Send(obj, MessagingChannel.SendNotification, sendNoti);
            }
        }

        public static async Task RegisterDevice()
        {
            var bikerId = BikerService.GetBikerInfo()._id;

            var playerId = (await OneSignal.Current.IdsAvailableAsync()).PlayerId;
            var platform = Xamarin.Essentials.DeviceInfo.Platform.ToString().ToLower();

            var deviceInfo = new { InstallationId = playerId, Platform = platform };

            await HttpClientService.Post($"https://delivery-3rd-api.azurewebsites.net/api/Biker/RegisterBikerDevice/{bikerId}", deviceInfo);
        }

        public static async Task UnRegisterDevice() 
        {
            var bikerId = BikerService.GetBikerInfo()._id;

            var playerId = (await OneSignal.Current.IdsAvailableAsync()).PlayerId;
            var platform = Xamarin.Essentials.DeviceInfo.Platform.ToString().ToLower();

            var deviceInfo = new { installationId = playerId, platform = platform };

            await HttpClientService.Put($"https://delivery-3rd-api.azurewebsites.net/api/Biker/UnRegisterBikerDevice/{bikerId}", deviceInfo);
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

        public static void RemoveNotificationStack(string notiChannel)
        {
            if (NotificatiionStack.ContainsKey(notiChannel))
            {
                NotificatiionStack.Remove(notiChannel);
            }
        }

        public static void ExecuteNotificationIfExist(string notiChannel)
        {
            object returnParam;
            var notiIsExits = NotificatiionStack.TryGetValue(notiChannel, out returnParam);
            if (notiIsExits)
            {
                PublishNotification(notiChannel);
            }
        }
    }
}
