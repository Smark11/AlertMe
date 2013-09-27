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
            txtYourName.Text = App.gYourName;
            txtContact1Name.Text = App.gContact1Name;
            txtContact1Email.Text = App.gContact1Email;
            txtContact1Phone.Text = App.gContact1Phone;
            txtContact2Name.Text = App.gContact2Name;
            txtContact2Email.Text = App.gContact2Email;
            txtContact2Phone.Text = App.gContact2Phone;

            if (App.gContact1Enabled == "YES")
            {
                chkContact1.IsChecked = true;
            }
            else
            {
                chkContact1.IsChecked = false;
            }

            if (App.gContact2Enabled == "YES")
            {
                chkContact2.IsChecked = true;
            }
            else
            {
                chkContact2.IsChecked = false;
            }

            if (App.gPlayAlarm == "YES")
            {
                togglePlayAlarm.IsChecked = true;
            }
            else
            {
                togglePlayAlarm.IsChecked = false;
            }

        }

        #region "Events"

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
            IS.SaveSetting("YourName", txtYourName.Text.ToString());
            IS.SaveSetting("Contact1Name", txtContact1Name.Text.ToString());
            IS.SaveSetting("Contact1Email", txtContact1Email.Text.ToString());
            IS.SaveSetting("Contact1Phone", txtContact1Phone.Text.ToString());
            IS.SaveSetting("Contact2Name", txtContact2Name.Text.ToString());
            IS.SaveSetting("Contact2Email", txtContact2Email.Text.ToString());
            IS.SaveSetting("Contact2Phone", txtContact2Phone.Text.ToString());
                      
            App.gYourName = txtYourName.Text;
            App.gContact1Name = txtContact1Name.Text;
            App.gContact1Email = txtContact1Email.Text;
            App.gContact1Phone = txtContact1Phone.Text;
            App.gContact2Name = txtContact2Name.Text;
            App.gContact2Email = txtContact2Email.Text;
            App.gContact2Phone = txtContact2Phone.Text;         
        }

        private void ctlDefaultCountdownTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (ctlDefaultCountdownTime != null)
            {
                IS.SaveSetting("DefaultCountdown", ctlDefaultCountdownTime.Value.ToString());
                App.gDefaultCountdown = TimeSpan.Parse(ctlDefaultCountdownTime.Value.ToString());
            }
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