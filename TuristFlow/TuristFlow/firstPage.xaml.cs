﻿using SQLite.Net;
using System;
using System.IO;
using System.Linq;
using TuristFlow.models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


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
        }

        // saveData
        private void SubmitButton_Click_1(object sender, RoutedEventArgs e)
        {
            Person p = new Person();
            p.IDLocal = RandomString(10);
            LocalDBConnection();
        }

        private void LocalDBConnection()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.Person");
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

    }
}
