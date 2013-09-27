using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Common.IsolatedStoreage;

namespace AlertMe
{
    public partial class Options : PhoneApplicationPage
    {
        public Options()
        {
            InitializeComponent();

            GetCountdownDefaultTime();
        }

        #region "Events"

        private void ctlDefaultCountdownTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
               
            IS.SaveSetting("Alerts-DefaultCountdown", ctlDefaultCountdownTime.Value.ToString());
            App.gDefaultCountdown = TimeSpan.Parse(ctlDefaultCountdownTime.Value.ToString());
        }

        private void togglePlayAlarm_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("PlayAlarm", "On");
        }

        private void togglePlayAlarm_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("PlayAlarm", "Off");
        }

        private void chkContact1_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact1", "Enabled");
        }

        private void chkContact1_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact1", "Disabled");
        }

        private void chkContact2_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact2", "Enabled");
        }

        private void chkContact2_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact2", "Disabled");
        }

        private void chkContact3_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact3", "Enabled");
        }

        private void chkContact3_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact3", "Disabled");
        }

        #endregion "Events"

        #region "Methods"
        private void GetCountdownDefaultTime()
        {
            string countdownAlarmValue = string.Empty;

            if (IS.GetSettingStringValue("Alerts-DefaultCountdown") == string.Empty)
            {
                ctlDefaultCountdownTime.Value = new TimeSpan(0, 1, 0);
            }
            else
            {
                countdownAlarmValue = IS.GetSettingStringValue("Alerts-DefaultCountdown");
                ctlDefaultCountdownTime.Value = TimeSpan.Parse(countdownAlarmValue);
            }
        }
        #endregion "Methods"
    }
}