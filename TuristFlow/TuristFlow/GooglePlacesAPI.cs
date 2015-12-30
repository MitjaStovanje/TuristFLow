using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TuristFlow.models;

namespace TuristFlow
{
    class GooglePlacesAPI
    {
        public GooglePlaces gp { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string types { get; set; }
        public string name { get; set; }
        public int radius { get; set; }

        public GooglePlacesAPI(string latitude, string longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.radius = 500;
            this.name = "";
            this.types = "";
        }

        public GooglePlacesAPI(string latitude, string longitude, int radius)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.radius = radius;
            this.name = "";
            this.types = "";
        }

        public GooglePlacesAPI(string latitude, string longitude, int radius, string types)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.radius = radius;
            this.name = "";
            this.types = types;
        }

        public GooglePlacesAPI(string latitude, string longitude, int radius, string name, string types)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.radius = radius;
            this.name = name;
            this.types = types;
        }


        public async void getDirections()
        {
            string tmp = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + this.latitude + "," + this.longitude + "&radius=" + this.radius + "&types=" + this.types + "&name=" + this.name + "&key=AIzaSyBhNJOVs683SBhnuLz0CQ0iz_HzYZ0gohM";
            HttpClient http = new System.Net.Http.HttpClient();
            string result = await http.GetStringAsync(tmp);
            Task.Delay(TimeSpan.FromSeconds(10));
            gp = (GooglePlaces)JsonConvert.DeserializeObject<GooglePlaces>(result);
        }
    }
}
