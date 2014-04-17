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
using Microsoft.Phone.UserData;

namespace AlertMe
{
    public partial class Options : PhoneApplicationPage
    {
        public Options()
        {
            InitializeComponent();

            txtSentTextCount.Text = App.gSentTextCount.ToString();

        }
        #region "Events"

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
          
        }

      
      


        #endregion "Events"

        private void chkBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;

            MessageBox.Show("You checked " + chkBox.Content);
        }

        private void chkBoxUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;

            MessageBox.Show("You unchecked " + chkBox.Content);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Contacts cons = new Contacts();

            //Identify the method that runs after the asynchronous search completes.
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(Contacts_SearchCompleted);
            
            //Start the asynchronous search.
            cons.SearchAsync(string.Empty, FilterKind.None, "Contacts Test #1");
        }

        void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            try
            {
                
                //Bind the results to the user interface.
                ContactResultsData.DataContext = e.Results;
            }
            catch (System.Exception)
            {
                //No results
            }

          
        }

    }
}