using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TuristFlow.models;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Services.Maps;
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
        string route;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.StartTimer(1, async () => await this.GetCurentLocation());
            
            //this.places();
        }

        private async Task<Geoposition> location()
        {
            Geoposition geoposition = null;
            Geolocator geolocator = new Geolocator();

            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                // Handle errors like unauthorized access to location
                // services or no Internet access.
            }
            return geoposition;
        }

        private async Task GetCurentLocation()
        {
            Geoposition geoposition = await location();
            

            InputMap.Center = geoposition.Coordinate.Point;
            InputMap.ZoomLevel = 16;
            InputMap.TrafficFlowVisible = true;

            Windows.UI.Xaml.Shapes.Ellipse fence =
            new Windows.UI.Xaml.Shapes.Ellipse();
            fence.Width = 10;
            fence.Height = 10;
            fence.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkOrange);
            fence.StrokeThickness = 2;
            MapControl.SetLocation(fence, geoposition.Coordinate.Point);
            MapControl.SetNormalizedAnchorPoint(fence, new Point(.5, .5));
            InputMap.Children.Add(fence);
            ShowRouteOnMap();
            places(); 
            
           
        }

        private void StartTimer(int v, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, v, 0);
            timer.Tick += (s, e) => action();
            timer.Start();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await GetCurentLocation();
        }

        private async void ShowRouteOnMap()
        {
            Geoposition current = await location();
            Location l = new Location();
            l.LAT = current.Coordinate.Latitude.ToString();
            l.LOT = current.Coordinate.Longitude.ToString();
            Location destination = await GetShortestLocation(l);
            BasicGeoposition startLocation = new BasicGeoposition() { Latitude = Convert.ToDouble(l.LAT), Longitude = Convert.ToDouble(l.LOT) };

            BasicGeoposition endLocation = new BasicGeoposition() { Latitude = Convert.ToDouble(destination.LAT), Longitude = Convert.ToDouble(destination.LOT)};


            // Get the route between the points.
            MapRouteFinderResult routeResult =
                await MapRouteFinder.GetWalkingRouteAsync(new Geopoint(startLocation),
                  new Geopoint(endLocation));


            route = "Minutes = " + routeResult.Route.EstimatedDuration.TotalMinutes.ToString()
                + "  Kilometers = " + (routeResult.Route.LengthInMeters / 1000).ToString();

            Content.Text = route;
            

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Windows.UI.Colors.AliceBlue;
                viewOfRoute.OutlineColor = Windows.UI.Colors.Black;

                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                InputMap.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await InputMap.TrySetViewBoundsAsync(
                      routeResult.Route.BoundingBox,
                      null,
                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }
        }

        private async Task<Location> GetShortestLocation(Location start)
        {
            Location t = new Location();
            IMobileServiceTable<Location> table = App.MobileService.GetTable<Location>();
            List<Location> res = await table.ToListAsync();
            t= await CalculateDistance(start, res);
            return t;
        }

        private async Task<Location> CalculateDistance(Location l,List<Location> res)
        {
            Geoposition current = await location();
            Location destination = new Location();
            var min = DistanceHeveraste(l,res.First(),"Kilometers");
            foreach (Location point in res.Skip(1)) {
                                                    
                var distance = DistanceHeveraste(l, point, "Kilometers");
                if (distance < min) {
                    min = distance;
                    destination.LAT = point.LAT;
                    destination.LOT = point.LOT;
                    destination.Name = point.Name;
                    
                }
            }
            return destination;

        }



        public double DistanceHeveraste(Location pos1, Location pos2, string type)
        {
            double R = (type == "Miles") ? 3960 : 6371;
            double dLat = this.toRadian(Convert.ToDouble(pos2.LAT) - Convert.ToDouble(pos1.LAT));
            double dLon = this.toRadian(Convert.ToDouble(pos2.LOT) - Convert.ToDouble(pos1.LOT));
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(this.toRadian(Convert.ToDouble(pos1.LAT))) * Math.Cos(this.toRadian(Convert.ToDouble(pos2.LAT))) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }
    

        private void Search_Click(object sender, RoutedEventArgs e)
        {

            ShowRouteOnMap();    
        }
        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        public void places()
        {
            GooglePlacesAPI gpa = new GooglePlacesAPI("46.047924", "14.506234");
            gpa.getDirections();
          
        }
        //check internet
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
