using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;
using Common.IsolatedStoreage;
using Microsoft.Phone.Maps.Controls;
using System.Threading;




namespace AlertMe
{
    public partial class SendAlert : INotifyPropertyChanged
    {
        DispatcherTimer dispatcherTimer;
        public event PropertyChangedEventHandler PropertyChanged;
        Geolocator myGeoLocator;
        int debug;

        public SendAlert()
        {
            try
            {
                debug = 0;
                InitializeComponent();
                Addresses = new List<string>();
                debug = 1;
                GetGPSLocation();

                //Sleep to give GPS time to get address
                Thread.Sleep(2000);
                debug = 3;
                // RequestDirections();

                if (App.gEnableCountdown.ToUpper() == "YES")
                {
                    debug = 31;
                    ClockValue = App.gDefaultCountdown;
                }
                else
                {
                    debug = 32;
                    ClockValue = new TimeSpan(0, 0, 1); ;
                }
                debug = 4;
                ClockValueString = ClockValue.ToString(@"\:s");

                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
                debug = 5;
                this.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Alert Constructor -> Debug = " + debug + " msg=" + ex.Message);
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

        private TimeSpan _clockValue;
        public TimeSpan ClockValue
        {
            get { return _clockValue; }
            set
            {
                _clockValue = value;
                NotifyPropertyChanged("ClockValue");
            }
        }

        private String _clockValueString;
        public String ClockValueString
        {
            get { return _clockValueString; }
            set
            {
                _clockValueString = value;
                NotifyPropertyChanged("ClockValueString");
            }
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

        #endregion "Properties"

        #region "Events"

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ClockValue = ClockValue - new TimeSpan(0, 0, 1);
            ClockValueString = ClockValue.ToString(@"\:s");

            if (ClockValue == new TimeSpan(0, 0, 0))
            {
                dispatcherTimer.Stop();
                SendTextAlert();
            }
        }

        private void StopAlert_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
        }

        #endregion "Events"

        #region "Methods"

        private void SendTextAlert()
        {
            try
            {
               

                SmsComposeTask smsComposeTask = new SmsComposeTask();

                smsComposeTask.To = "123-45-45";
                smsComposeTask.Body = "My location is: " + Address;
                smsComposeTask.Show();

                UpdateSentTextCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SendTextAlert -> " + ex.Message);
            }
        }

        private void UpdateSentTextCount()
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

        private void SendEmail()
        {
            try
            {
                EmailComposeTask emailCompuser = new EmailComposeTask();

                emailCompuser.Subject = "GPS Location";
                emailCompuser.Body = "This is a test email";
                emailCompuser.Show();
            }
            catch (Exception)
            {
            }
        }

        async void RequestDirections()
        {
            // Get the values required to specify the destination.
            string latitude = "47.6451413797194";
            string longitude = "-122.141964733601";
            string name = "Redmond, WA";

            // Assemble the Uri to launch.
            Uri uri = new Uri("ms-drive-to:?destination.latitude=" + latitude +
                "&destination.longitude=" + longitude + "&destination.name=" + name);
            // The resulting Uri is: "ms-drive-to:?destination.latitude=47.6451413797194
            //  &destination.longitude=-122.141964733601&destination.name=Redmond, WA")

            // Launch the Uri.
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // Uri launched.
            }
            else
            {
                // Uri failed to launch.
            }
        }

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

        #endregion "Methods"
    }

}
