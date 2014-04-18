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
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;

namespace AlertMe
{
    public partial class MainPage : INotifyPropertyChanged
    {
        PhoneNumberChooserTask phoneNumberChooserTask;

        public event PropertyChangedEventHandler PropertyChanged;

        Geolocator myGeoLocator;
        // Constructor
        public MainPage()
        {          
            InitializeComponent();
            Addresses = new List<string>();
           // GetSettings();
            GetGPSLocation();

            BuildLocalizedApplicationBar();

            AlertButton = "/Assets/Button.jpg";

            if ((Application.Current as App).IsTrial)
            {
                TextStatusMessage = "Trail Text Sent: " + App.gSentTextCount.ToString() + "/" + App.gTextLimit;          
                TextStatusMessageVisibility = Visibility.Visible;
            }
            else
            {
                TextStatusMessage = string.Empty;
                TextStatusMessageVisibility = Visibility.Collapsed;
            };
         
            this.DataContext = this;
        }

        #region "Properties"

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _textStatusMessage;
        public string TextStatusMessage
        {
            get { return _textStatusMessage; }
            set
            {
                _textStatusMessage = value;
                NotifyPropertyChanged("TextStatusMessage");
            }
        }

        private Visibility _textStatusMessageVisibility;
        public Visibility TextStatusMessageVisibility
        {
            get { return _textStatusMessageVisibility; }
            set
            {
                _textStatusMessageVisibility = value;
                NotifyPropertyChanged("TextStatusMessageVisibility");
            }
        }

        private string _alertButton;
        public string AlertButton
        {
            get { return _alertButton; }
            set { _alertButton = value; NotifyPropertyChanged("_alertButton"); }
        }

        private Double _latitude;
        public Double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                NotifyPropertyChanged("Latitude");
            }
        }

        private Double _longitude;
        public Double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                NotifyPropertyChanged("Longitude");
            }
        }

        private List<string> _addresses;
        public List<string> Addresses
        {
            get { return _addresses; }
            set
            {
                _addresses = value;
                NotifyPropertyChanged("Addresses");
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyPropertyChanged("Address");
            }
        }

        private string _mapURL;
        public string MapURL
        {
            get { return _mapURL; }
            set
            {
                _mapURL = value;
                NotifyPropertyChanged("MapURL");
            }
        }
        #endregion "Properties"

        #region "Methods"

        private async void GetGPSLocation()
        {
            try
            {
                myGeoLocator = new Geolocator();
                myGeoLocator.DesiredAccuracy = PositionAccuracy.Default;
                myGeoLocator.MovementThreshold = 50;

                Geoposition geoposition = await myGeoLocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));

                Latitude = geoposition.Coordinate.Latitude;
                Longitude = geoposition.Coordinate.Longitude;

                MapURL = "https://www.google.com/maps/place/@" + Latitude + "," + Longitude;

                var reverseGeocode = new ReverseGeocodeQuery();
                reverseGeocode.GeoCoordinate = new GeoCoordinate(geoposition.Coordinate.Latitude, Longitude);
                reverseGeocode.QueryCompleted += ReverseGeocodeQueryCompleted;
                reverseGeocode.QueryAsync();
            }

            catch (Exception ex)
            {
            }
        }

        private void ReverseGeocodeQueryCompleted(object sender, QueryCompletedEventArgs<System.Collections.Generic.IList<MapLocation>> e)
        {
            var reverseGeocode = sender as ReverseGeocodeQuery;

            try
            {
                if (reverseGeocode != null)
                {
                    reverseGeocode.QueryCompleted -= ReverseGeocodeQueryCompleted;
                }

                Addresses.Clear();

                if (!e.Cancelled)
                {
                    foreach (var address in e.Result.Select(adrInfo => adrInfo.Information.Address))
                    {
                        Addresses.Add(string.Format("{0} {1} {2} {3} {4}", address.Street, address.HouseNumber, address.PostalCode,
                          address.City, address.Country).Trim());
                        Address = address.HouseNumber + " " + address.Street + " " + address.City + " " + address.State + " " + address.PostalCode;
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        private void SendTextAlert()
        {
            try
            {
                UpdateSentTextCount(1);
                SmsComposeTask smsComposeTask = new SmsComposeTask();
                smsComposeTask.To = "";
                smsComposeTask.Body = "My location is: " + Address;
                smsComposeTask.Show();                                     
            }
            catch (Exception ex)
            {
                UpdateSentTextCount(-1);
                MessageBox.Show("Text not sent.  Error = " + ex.Message);
            }
        }

        private void UpdateSentTextCount(int value)
        {
            if (IS.GetSetting("SentTextCount") == null)
            {
                IS.SaveSetting("SentTextCount", 1);
            }
            else
            {
                IS.SaveSetting("SentTextCount", (int)IS.GetSetting("SentTextCount") + value);
            }

            App.gSentTextCount = (int)IS.GetSetting("SentTextCount");
            TextStatusMessage = "Trail Text Sent: " + App.gSentTextCount.ToString() + "/" + App.gTextLimit;          
        }

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
            //ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppMenuItemOptions);
            //ApplicationBar.MenuItems.Add(appBarMenuItem);
            //appBarMenuItem.Click += new EventHandler(Options_Click);

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
            if ((App.gSentTextCount > App.gTextLimit) && ((Application.Current as App).IsTrial))
            {
                MessageBox.Show("You have exceeded the trail number of texts sent limit (" + App.gTextLimit + ").  Please purchase application.");
            }
            else
            {
                SendTextAlert();
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