using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
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

            ctlDefaultCountdownTime.Value = App.gDefaultCountdown;
            txtSentTextCount.Text = App.gSentTextCount.ToString();


         
            if (App.gPlayAlarm == "YES")
            {
                togglePlayAlarm.IsChecked = true;
            }
            else
            {
                togglePlayAlarm.IsChecked = false;
            }

            if (App.gEnableCountdown == "YES")
            {
                toggleEnableCountdown.IsChecked = true;
            }
            else
            {
                toggleEnableCountdown.IsChecked = false;
            }
        }

        #region "Events"

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
          
        }

        private void ctlDefaultCountdownTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (ctlDefaultCountdownTime != null)
            {
                IS.SaveSetting("DefaultCountdown", ctlDefaultCountdownTime.Value.ToString());
                App.gDefaultCountdown = TimeSpan.Parse(ctlDefaultCountdownTime.Value.ToString());
            }
        }

        private void toggleEnableCountdown_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("EnableCountdown", "YES");
            App.gEnableCountdown = "YES";
        }

        private void toggleEnableCountdown_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("EnableCountdown", "NO");
            App.gEnableCountdown = "NO";
        }

        private void togglePlayAlarm_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("PlayAlarm", "YES");
            App.gPlayAlarm = "YES";
        }

        private void togglePlayAlarm_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("PlayAlarm", "NO");
            App.gPlayAlarm = "NO";
        }

        private void chkContact1_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact1Enabled", "YES");
            App.gContact1Enabled = "YES";
        }

        private void chkContact1_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact1Enabled", "NO");
            App.gContact1Enabled = "NO";
        }

        private void chkContact2_Checked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact2Enabled", "YES");
            App.gContact2Enabled = "YES";
        }

        private void chkContact2_Unchecked(object sender, RoutedEventArgs e)
        {
            IS.SaveSetting("Contact2Enabled", "NO");
            App.gContact2Enabled = "NO";
        }

        #endregion "Events"

    }
}