using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TuristFlow.models;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
        FavoritePersonLocation locat;
        Person localPerson;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.StartTimer(1, async () => await this.GetCurentLocation());
            App.conn.CreateTable<FavoritePersonLocation>();
            locat = new FavoritePersonLocation();
            localPerson = GetPerson();
            Lcation();
        }

        private void PointerPressedOverride(MapControl sender, MapElementClickEventArgs args)
        {
           Content.Text= args.Location.ToString();
        }

        private Person GetPerson() {
            Person query = App.conn.Table<Person>().First();
            return query;
        }

        private void Lcation()
        {
            var query = App.conn.Table<FavoritePersonLocation>().Where(x => x.Town != "");
            String priljubljene = "";
            foreach (FavoritePersonLocation l in query) {
                priljubljene += l.Addres + " " + l.Town+",";
            }
            // Nov textarea
            Favorite.Text = "Priljubljeni kraji"+"\n";
            Favorite.Text += priljubljene;

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
            if (App.IsInternet())
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
                
            }
            else {
                new ToastHelper().ShowToastWithTitleAndMessage("Povezava", "Prsimo prverite poezavo.");
                refresh.Visibility = Visibility.Visible;
            }
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

       
  
        private async void InputMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            buttonAdd.IsEnabled = true;
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
                resultText.Append(locat.Addres + "," + locat.Town+"\n");
            }

        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Favorite.Text += locat.Addres + " " + locat.Town;
            App.conn.Insert(locat);
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            GetCurentLocation();
            refresh.Visibility = Visibility.Collapsed;
        }
    }
}
