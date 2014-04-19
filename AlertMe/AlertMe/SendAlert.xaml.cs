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
using Twilio;

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
                //TestPhoneCall();
                //SmsComposeTask smsComposeTask = new SmsComposeTask();

                //smsComposeTask.To = "123-45-45";
                //smsComposeTask.Body = "My location is: " + Address;
                //smsComposeTask.Show();

                //UpdateSentTextCount();
                //for (int i = 0; i <= 10; i++)
                //{
                string accountSid = "ACd764978b05b047abb19f163cfb45b768";
                string authToken = "c87fff6fb1620158f3a3540de5b80110";

                var twilio = new TwilioRestClient(accountSid, authToken);

                twilio.SendSmsMessage("+18609670403", "+18607514302", "My Location is: " + Address, test => Test(test));
                //}
                
            }

            catch (Exception ex)
            {
            }
        }

        private void TestPhoneCall()
        {
            // Find your Account Sid and Auth Token at twilio.com/user/account
            string AccountSid = "ACd764978b05b047abb19f163cfb45b768";
            string AuthToken = "c87fff6fb1620158f3a3540de5b80110";
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            var options = new CallOptions();
            options.Url = "http://demo.twilio.com/docs/voice.xml";
            options.To = "+18607514302";
            options.From = "+18609670403";
            
            twilio.InitiateOutboundCall(options, results => CallComplete(results));

        }

        private void CallComplete(Call results)
        {
            
        }

        private void Test(SMSMessage message)
        {
            Console.WriteLine(message.Sid);
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
