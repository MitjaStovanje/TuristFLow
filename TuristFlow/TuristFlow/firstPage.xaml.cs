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
        Person p;
        SQLiteConnection conn;
        public static MobileServiceClient MobileService =
          new MobileServiceClient("https://turistflow.azure-mobile.net/",
              "EnLxaBpIkCZMxDVNHHmIbnhnnXEXNa60");
        public firstPage()
        {
            this.InitializeComponent();
            p = new Person();
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
            p.IDLocal = RandomString(10);
            LocalDBConnection();
            if (p != null)
            {
                conn.Insert(p);
                insertPerson(p);
                AddLactiion();
                
            }
            // Use this constructor instead after publishing to the cloud
           
            this.Frame.Navigate(typeof(MainPage));
        }
        private static async void AddLactiion()
        {
            Location l = new Location();
            l.LAT = "46.051171";
            l.LOT = "14.506234";
            l.Name = "Triple Bridge";
            await MobileService.GetTable<Location>().InsertAsync(l);
           

        }
        public static async void insertPerson(Person p)
        {

            await MobileService.GetTable<Person>().InsertAsync(p);
        }


        //localDatabase
        private void LocalDBConnection()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.TursiFlow");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<Person>();
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
            p.Age = Ageradio.Content.ToString();
        }

        private void companions_Checked(object sender, RoutedEventArgs e)
        {
            var companions = sender as RadioButton;
            p.group = companions.Content.ToString();
        }

        private void stay_Checked(object sender, RoutedEventArgs e)
        {
            var stay = sender as RadioButton;
            p.sleeping = stay.Content.ToString();
        }

        private void Transport3_Checked(object sender, RoutedEventArgs e)
        {
            var transport = sender as CheckBox;
            p.Transport = transport.Content.ToString();
        }

        private void budget1_Checked(object sender, RoutedEventArgs e)
        {
            var budget = sender as RadioButton;
            p.Budget = budget.Content.Equals("Limited");
        }

        private void tblength_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            var TravelTime = sender as TextBox;
            if (TravelTime.Text != null)
            {
                p.TravelTime = Int32.Parse(TravelTime.Text.ToString());
            }
        }


    }
}