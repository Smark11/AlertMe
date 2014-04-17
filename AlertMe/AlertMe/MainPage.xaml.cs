using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AlertMe.Resources;
using Microsoft.Phone.Tasks;
using Common.IsolatedStoreage;
using System.ComponentModel;

namespace AlertMe
{
    public partial class MainPage : INotifyPropertyChanged
    {
        PhoneNumberChooserTask phoneNumberChooserTask;

        public event PropertyChangedEventHandler PropertyChanged;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();

            if (IS.GetSettingStringValue("DefaultCountdown") == string.Empty)
            {
                App.gDefaultCountdown = new TimeSpan(0, 0, 5);
            }
            else
            {
                App.gDefaultCountdown = TimeSpan.Parse(IS.GetSettingStringValue("DefaultCountdown"));              
            }

            AlertButton = "/Assets/Button.jpg";


            
            phoneNumberChooserTask = new PhoneNumberChooserTask();
            phoneNumberChooserTask.Completed += new EventHandler<PhoneNumberResult>(phoneNumberChooserTask_Completed);
            phoneNumberChooserTask.Show();

            this.DataContext = this;
        }

        void phoneNumberChooserTask_Completed(object sender, PhoneNumberResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MessageBox.Show("The phone number for " + e.DisplayName + " is " + e.PhoneNumber);

                //Code to start a new call using the retrieved phone number.
                //PhoneCallTask phoneCallTask = new PhoneCallTask();
                //phoneCallTask.DisplayName = e.DisplayName;
                //phoneCallTask.PhoneNumber = e.PhoneNumber;
                //phoneCallTask.Show();
            }
        }

        #region "Properties"

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _alertButton;
        public string AlertButton
        {
            get { return _alertButton; }
            set { _alertButton = value; NotifyPropertyChanged("_alertButton"); }
        }
        #endregion "Properties"

        #region "Methods"

        #endregion "Methods"

        #region "Common Routines"

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // Create a new button and set the text value to the localized string from AppResources.
            //ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/feature.email.png", UriKind.Relative));
            //appBarButton.Text = AppResources.AppBarEmailButton;
            //ApplicationBar.Buttons.Add(appBarButton);
            //appBarButton.Click += new EventHandler(Email_Click);

            //ApplicationBarIconButton appBarButton2 = new ApplicationBarIconButton(new Uri("/Assets/cancel.png", UriKind.Relative));
            //appBarButton2.Text = AppResources.AppBarClearLapsButton;
            //ApplicationBar.Buttons.Add(appBarButton2);
            //appBarButton2.Click += new EventHandler(DeleteLaps_Click);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppMenuItemOptions);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
            appBarMenuItem.Click += new EventHandler(Options_Click);

            ApplicationBarMenuItem appBarMenuItem2 = new ApplicationBarMenuItem(AppResources.AppMenuItemAbout);
            ApplicationBar.MenuItems.Add(appBarMenuItem2);
            appBarMenuItem2.Click += new EventHandler(About_Click);

            ApplicationBarMenuItem appBarMenuItem4 = new ApplicationBarMenuItem(AppResources.AppMenuItemMoreApps);
            ApplicationBar.MenuItems.Add(appBarMenuItem4);
            appBarMenuItem4.Click += new EventHandler(MoreApps_Click);
        }

        #endregion "Common Routines"

        #region "Events"

        private void Alert_Click(object sender, RoutedEventArgs e)
        {
            if (App.gSentTextCount > 5)
            {
                MessageBox.Show("You have exceeded the trail number of texts sent limit.  Please purchase application.");
            }
            else
            {
                NavigationService.Navigate(new Uri("/SendAlert.xaml", UriKind.Relative));
            }
        }

        private void Options_Click(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri("/Options.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void Review_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void MoreApps_Click(object sender, EventArgs e)
        {
            MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();

            marketplaceSearchTask.SearchTerms = "KLBCreations";
            marketplaceSearchTask.Show();
        }


        #endregion "Events"





    }
}