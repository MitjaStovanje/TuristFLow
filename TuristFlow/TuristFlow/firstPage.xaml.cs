using SQLite.Net;
using System;
using System.IO;
using System.Linq;
using TuristFlow.models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Windows.Networking.Connectivity;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TuristFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class firstPage : Page
    {
        
      
        public firstPage()
        {
            this.InitializeComponent();
            App.p = new Person();
        }

        public static bool CheckForInternetConnection()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        // saveData
        private void SubmitButton_Click_1(object sender, RoutedEventArgs e)
        {
          
            App.p.IDLocal = RandomString(10);
            LocalDBConnection();
            if (App.p != null)
            { 
                if (App.IsInternet())
                {
                    insertPerson(App.p);
                }
                else {
                    new ToastHelper().ShowToastWithTitleAndMessage("Napaka","Preverite povezavo");
                }
                App.conn.Insert(App.p);

            }
            this.Frame.Navigate(typeof(MainPage));
        }

        public static async void insertPerson(Person p)
        {

            await App.MobileService.GetTable<Person>().InsertAsync(p);
        }


        //localDatabase
        private void LocalDBConnection()
        {
           
            App.conn.CreateTable<Person>();
        }


        //generate rendom string for IDLocal
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // check agre.
        private void ageRB_Checked(object sender, RoutedEventArgs e)
        {
            var Ageradio = sender as RadioButton;
            App.p.Age = Ageradio.Content.ToString();
        }

        private void companions_Checked(object sender, RoutedEventArgs e)
        {
            var companions = sender as RadioButton;
            App.p.group = companions.Content.ToString();
        }

        private void stay_Checked(object sender, RoutedEventArgs e)
        {
            var stay = sender as RadioButton;
            App.p.sleeping = stay.Content.ToString();
        }

        private void Transport3_Checked(object sender, RoutedEventArgs e)
        {
            var transport = sender as CheckBox;
            App.p.Transport = transport.Content.ToString();
        }

        private void budget1_Checked(object sender, RoutedEventArgs e)
        {
            var budget = sender as RadioButton;
            App.p.Budget = budget.Content.Equals("Limited");
        }

        private void tblength_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            var TravelTime = sender as TextBox;
            if (TravelTime.Text != null)
            {
                App.p.TravelTime = Int32.Parse(TravelTime.Text.ToString());
            }
        }


    }
}