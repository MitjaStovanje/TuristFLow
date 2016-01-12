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
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Core;
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
        
        public MainPage()
        {
            this.InitializeComponent();
            MyFrame.Navigate(typeof(Map));
            
            //coords.ItemsSource = coordinates;
        }


        /*private void PointerPressedOverride(MapControl sender, MapElementClickEventArgs args)
        {
           Content.Text= args.Location.ToString();
        }*/

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void TextBlock_DoubleTapped_1(object sender, DoubleTappedRoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Priljubljeno));
        }

        private void TextBlock_DoubleTapped_2(object sender, DoubleTappedRoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Obiskano));
        }

        private void TextBlock_DoubleTapped_3(object sender, DoubleTappedRoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Podrobnosti));
        }

        private void TextBlock_DoubleTapped_4(object sender, DoubleTappedRoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Map));
        }


        /*private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Favorite.Text += locat.Addres + " " + locat.Town;
            App.conn.Insert(locat);
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            GetCurentLocation();
            refresh.Visibility = Visibility.Collapsed;
        }*/
    }
}
