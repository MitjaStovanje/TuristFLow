﻿using SQLite.Net;
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


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TuristFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class firstPage : Page
    {
        Person p;
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
            }
            Frame.Navigate(typeof(MainPage));
        }

        private void LocalDBConnection()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.TursiFlow");
            SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
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
            var radio = sender as RadioButton;
            p.Age =radio.Content.ToString();
        }
    }
}
