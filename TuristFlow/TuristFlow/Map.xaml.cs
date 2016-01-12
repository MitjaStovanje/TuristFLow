using Bing.Maps;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TuristFlow.models;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TuristFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Map : Page
    {
        string route;
        FavoritePersonLocation locat;
        Person localPerson;
        private Geolocator locator;
        private ObservableCollection<string> coordinates = new ObservableCollection<string>();
        IEnumerable<Geopoint> wayPoints;
        public Map()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            App.conn.CreateTable<FavoritePersonLocation>();
            locat = new FavoritePersonLocation();
            localPerson = GetPerson();
            Lcation();
            locator = new Geolocator();
            locator.DesiredAccuracy = PositionAccuracy.High;
            locator.DesiredAccuracyInMeters = 0;
            locator.MovementThreshold = 0;
            locator.PositionChanged += Locator_PositionChanged;
        }

        private void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var coord = args.Position;
            string position = string.Format("{0},{1}",
                args.Position.Coordinate.Point.Position.Latitude, //yeah it's this deep! Surprised smile
                args.Position.Coordinate.Point.Position.Longitude);
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                coordinates.Insert(0, position);

            });
        }

        private async void insertPosition(Person id, string position)
        {
            await App.MobileService.GetTable<Person>().InsertAsync(p);
        }

        private ExtendedExecutionSession session;

        private async void StartLocationExtensionSession()
        {
            session = new ExtendedExecutionSession();
            session.Description = "Location Tracker";
            session.Reason = ExtendedExecutionReason.LocationTracking;
            session.Revoked += ExtendedExecutionSession_Revoked;
            var result = await session.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Denied)
            {
                //TODO: handle denied
            }
        }

        private void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            if (session != null)
            {
                session.Dispose();
                session = null;
            }
        }

        private Person GetPerson()
        {
            Person query = App.conn.Table<Person>().First();
            return query;
        }

        private void Lcation()
        {
            var query = App.conn.Table<FavoritePersonLocation>().Where(x => x.Town != "");
            String priljubljene = "";
            foreach (FavoritePersonLocation l in query)
            {
                priljubljene += l.Addres + " " + l.Town + ",";
            }
            // Nov textarea
            // Favorite.Text = "Priljubljeni kraji"+"\n";
            // Favorite.Text += priljubljene;

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

            App.geoposition = await location();


            InputMap.Center = App.geoposition.Coordinate.Point;
            InputMap.ZoomLevel = 16;
            InputMap.TrafficFlowVisible = true;

            Windows.UI.Xaml.Shapes.Ellipse fence =
            new Windows.UI.Xaml.Shapes.Ellipse();
            fence.Width = 10;
            fence.Height = 10;
            fence.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkOrange);
            fence.StrokeThickness = 2;
            MapControl.SetLocation(fence, App.geoposition.Coordinate.Point);
            MapControl.SetNormalizedAnchorPoint(fence, new Point(.5, .5));
            InputMap.Children.Add(fence);
            //ShowRouteOnMap();

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

            InputMap.Children.Clear();
            await GetCurentLocation();
        }

        private async void ShowRouteOnMap()
        {
            Geoposition current = await location();
            models.Location l = new models.Location();
            l.LAT = current.Coordinate.Latitude.ToString();
            l.LOT = current.Coordinate.Longitude.ToString();
            models.Location destination = await GetShortestLocation(l);
            BasicGeoposition startLocation = new BasicGeoposition() { Latitude = Convert.ToDouble(l.LAT), Longitude = Convert.ToDouble(l.LOT) };

            BasicGeoposition endLocation = new BasicGeoposition() { Latitude = Convert.ToDouble(destination.LAT), Longitude = Convert.ToDouble(destination.LOT) };

            
            // Get the route between the points.
            /*MapRouteFinderResult routeResult =
                await MapRouteFinder.GetWalkingRouteAsync(new Geopoint(startLocation),
                  new Geopoint(endLocation));*/

            
            MapRouteFinderResult routeD = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(
               wayPoints);


            route = "Minutes = " + routeD.Route.EstimatedDuration.TotalMinutes.ToString()
                + "  Kilometers = " + (routeD.Route.LengthInMeters / 1000).ToString();

            //Content.Text = route;


            if (routeD.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeD.Route);
                viewOfRoute.RouteColor = Windows.UI.Colors.AliceBlue;
                viewOfRoute.OutlineColor = Windows.UI.Colors.Black;

                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                InputMap.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await InputMap.TrySetViewBoundsAsync(
                      routeD.Route.BoundingBox,
                      null,
                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }
            places(endLocation);
        }

        private void places(BasicGeoposition endLocation)
        {
            GooglePlacesAPI gpa = new GooglePlacesAPI(endLocation.Latitude.ToString(), endLocation.Longitude.ToString());
            var test = gpa.getDirections();

            var tocka = test.Result.results;
            foreach (var t in tocka)
            {
                MapIcon MapIcon1 = new MapIcon();
                MapIcon1.Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(t.geometry.location.lat), Longitude = Convert.ToDouble(t.geometry.location.lng) });
                MapIcon1.Title = t.name;
                MapIcon1.Image = RandomAccessStreamReference.CreateFromUri(new Uri(t.icon));
                InputMap.MapElements.Add(MapIcon1);
            }

        }

        private async Task<models.Location> GetShortestLocation(models.Location start)
        {
            models.Location t = new models.Location();
            IMobileServiceTable<models.Location> table = App.MobileService.GetTable<models.Location>();
            List<models.Location> res = await table.ToListAsync();
            var listPoint = new List<Geopoint>();
            listPoint.Add(new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(start.LAT), Longitude = Convert.ToDouble(start.LOT) }));
            foreach (var p in res)
            {
                listPoint.Add(new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(p.LAT), Longitude = Convert.ToDouble(p.LOT) }));
            }
            wayPoints = (IEnumerable<Geopoint>)listPoint;
            t = await CalculateDistance(start, res);
            return t;
        }

        private async Task<models.Location> CalculateDistance(models.Location l, List<models.Location> res)
        {
            Geoposition current = await location();
            models.Location destination = new models.Location();
            var min = DistanceHeveraste(l, res.First(), "Kilometers");
            foreach (models.Location point in res.Skip(1))
            {

                var distance = DistanceHeveraste(l, point, "Kilometers");
                if (distance < min)
                {
                    min = distance;
                    destination.LAT = point.LAT;
                    destination.LOT = point.LOT;
                    destination.Name = point.Name;

                }
            }
            return destination;

        }

        public double DistanceHeveraste(models.Location pos1, models.Location pos2, string type)
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

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        private async void InputMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            //buttonAdd.IsEnabled = true;
            Geopoint pointToReverseGeocode = new Geopoint(args.Location.Position);
            // Reverse geocode the specified geographic location.  
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);
            var resultText = new StringBuilder();
            if (result.Status == MapLocationFinderStatus.Success)
            {
                locat.IDPerson = localPerson.IDLocal;
                locat.Addres = result.Locations[0].Address.District;
                locat.Town = result.Locations[0].Address.Town;
                locat.Longitude = result.Locations[0].Point.Position.Longitude;
                locat.Latitude = result.Locations[0].Point.Position.Latitude;
                resultText.Append(locat.Addres + "," + locat.Town + "\n");
            }

        }
    }
}
