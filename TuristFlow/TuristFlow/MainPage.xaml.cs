using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

            this.StartTimer(5, async () => await this.GetCurentLocation());
            
            //this.places();
        }

        private async Task GetCurentLocation()
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

            //Content.Text = geoposition.Coordinate.Point.Position.Latitude + "Latitude..........." + geoposition.Coordinate.Point.Position.Longitude + "Lonitude";
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
        private async void ShowRouteOnMap(double startLatitude, double startLongitude, double finalLatitude, double finalLongitude)
        {
            BasicGeoposition startLocation = new BasicGeoposition() { Latitude = startLatitude, Longitude = startLongitude };

            BasicGeoposition endLocation = new BasicGeoposition() { Latitude = finalLatitude, Longitude = finalLongitude };


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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            this.ShowRouteOnMap(46.051468, 14.506031, 46.036905, 14.488618);
            places();
        }

        public void places()
        {
            GooglePlacesAPI gpa = new GooglePlacesAPI("46.036905", "14.488618", 500);
            gpa.getDirections();
          


        }
    }
}
