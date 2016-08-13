using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using GeoCoordinatePortable;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
namespace PoGo.PokeMobBot.Logic
{
    public static class Routing
    {
        public static RoutingResponse GetRoute(GeoCoordinate start, GeoCoordinate dest)
        {
            try
            {
                WebRequest request = WebRequest.Create(
                  $"http://www.yournavigation.org" + $"/api/1.0/gosmore.php?format=geojson&flat={start.Latitude}&flon={start.Longitude}&tlat={dest.Latitude}&tlon={dest.Longitude}&v=foot&fast=1&layer=mapnik");
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                //Console.WriteLine(responseFromServer);
                RoutingResponse responseParsed = JsonConvert.DeserializeObject<RoutingResponse>(responseFromServer);
                reader.Close();
                response.Close();

                return responseParsed;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Routing error: " + ex.Message);
            }
            return new RoutingResponse();  
        }
    }

    public class RoutingResponse
    {
        public string type { get; set; }
        public Crs crs { get; set; }
        public List<List<double>> coordinates { get; set; }
        public Properties2 properties { get; set; }
    }
    public class Properties
    {
        public string name { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties2
    {
        public string distance { get; set; }
        public string description { get; set; }
        public string traveltime { get; set; }
    }

    
}
