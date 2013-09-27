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

            ClockValue = App.gDefaultCountdown;
            ClockValueString = ClockValue.ToString(@"\:s");

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            Addresses = new ObservableCollection<string>();

            this.DataContext = this;

            GetGPSLocation();
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

        private ObservableCollection<string> _addresses;
        public ObservableCollection<string> Addresses
        {
            get { return _addresses; }
            set
            {
                _addresses = value;
                NotifyPropertyChanged("Addresses");
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

            }
            catch (Exception)
            {

                throw;
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

            catch (Exception)
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
                    foreach (var adress in e.Result.Select(adrInfo => adrInfo.Information.Address))
                    {
                        Addresses.Add(string.Format("{0} {1} {2} {3} {4}", adress.Street, adress.HouseNumber, adress.PostalCode,
                          adress.City, adress.Country).Trim());
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
