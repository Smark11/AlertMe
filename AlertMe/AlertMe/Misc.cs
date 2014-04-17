using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using Common.IsolatedStoreage;

namespace AlertMe
{
    class Misc
    {


        //public void SendTextAlert()
        //{
        //    try
        //    {
        //        SmsComposeTask smsComposeTask = new SmsComposeTask();

        //        smsComposeTask.To = "123-45-45";
        //        smsComposeTask.Body = "My location is: " + Address;
        //        smsComposeTask.Show();

        //        UpdateSentTextCount();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        public void UpdateSentTextCount()
        {
            if (IS.GetSetting("SentTextCount") == null)
            {
                IS.SaveSetting("SentTextCount", 1);
            }
            else
            {
                IS.SaveSetting("SentTextCount", (int)IS.GetSetting("SentTextCount") + 1);
            }

            App.gSentTextCount = (int)IS.GetSetting("SentTextCount");
        }

    }
}
