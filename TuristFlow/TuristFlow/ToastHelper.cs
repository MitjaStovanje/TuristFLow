using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Notifications;

namespace TuristFlow
{
    public class ToastHelper
    {
        public void ShowToastWithTitleAndMessage(string title, string message)
        {
            ToastVisual visual = new ToastVisual
            {
                TitleText = new ToastText
                {
                    Text = title
                },

                BodyTextLine1 = new ToastText
                {
                    Text = message
                },
              
            };
           

            ToastContent toastContent = new ToastContent
            {
                Visual = visual
               
            };

            var toast = new ToastNotification(toastContent.GetXml());
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);
        }
    }
}
