﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            this.radius = 100;
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


        public async Task<GooglePlaces> getDirections()
        {
            string tmp = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + this.latitude + "," + this.longitude + "&radius=" + this.radius + "&types=" + this.types + "&name=" + this.name + "&key=AIzaSyBhNJOVs683SBhnuLz0CQ0iz_HzYZ0gohM";
           
            HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            HttpResponseMessage response = client.GetAsync(tmp).Result;
            var places = response.Content.ReadAsStringAsync();
            gp = (GooglePlaces)JsonConvert.DeserializeObject<GooglePlaces>(places.Result);
            return gp;
        }
    }
}
