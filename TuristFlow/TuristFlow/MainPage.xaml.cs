using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TuristFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(firstPage));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Geoposition geoposition = null;

            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

            var position = await locator.GetGeopositionAsync();

            var lat = position.Coordinate.Latitude;
            var log = position.Coordinate.Longitude;

            Content.Text = lat + "-lat;::: " + log + "-log;::: ";

            await InputMap.TrySetViewAsync(position.Coordinate.Point, 18D);

            
        }
    }
}
