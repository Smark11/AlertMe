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

namespace AlertMe
{
    public partial class SendAlert : INotifyPropertyChanged
    {
        DispatcherTimer dispatcherTimer;
        public event PropertyChangedEventHandler PropertyChanged;
        Geolocator myGeoLocator;


        public SendAlert()
        {
            InitializeComponent();
            Addresses = new List<string>();

            GetGPSLocation();

            ClockValue = App.gDefaultCountdown;
            ClockValueString = ClockValue.ToString(@"\:s");

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        
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
            }
            catch (Exception)
            {
            }
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
