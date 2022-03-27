using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.IO;
using System.Linq;

#nullable enable

namespace APIManager
{
    class F1API
    {
        public static RaceCalender currentCalender;

        public static async Task GetNextSchedule()
        {

            string year = System.DateTime.Today.Year.ToString();

            var webRequest = WebRequest.Create("http://ergast.com/api/f1/current.json");

            webRequest.Method = "GET";
            webRequest.Timeout = -1;
            webRequest.ContentType = "application/json";


            var response = await webRequest.GetResponseAsync();

           // response = (HttpWebRequest)response;

            Console.WriteLine("response request: " + response.ToString());

            var jsonString = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                jsonString = await sr.ReadToEndAsync();

               // Console.WriteLine("response string: " + jsonString);

               
            }

            currentCalender = JsonConvert.DeserializeObject<RaceCalender>(jsonString);
        }

        public static Race GetNextRace()
        {

            if (currentCalender == null) return new Race();

            currentCalender.MRData.RaceTable.Races.ForEach(r =>
            {
                var dateTimeString = r.date + r.time;

                dateTimeString = dateTimeString.Replace("-", "");
                dateTimeString = dateTimeString.Insert(8, "T");

                r.dateTime = DateTime.ParseExact(dateTimeString, "yyyyMMddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
            });

            List<Race> raceOrder = currentCalender.MRData.RaceTable.Races.OrderBy(r => r.dateTime).ToList();
            List<Race> completeRaces = new List<Race>();
            raceOrder.ForEach(r =>
            {
                if (r.dateTime < System.DateTime.Now)
                    completeRaces.Add(r);
            });

            completeRaces.ForEach(r =>
            {
                raceOrder.Remove(r);
            });

            return raceOrder.First();
        }
    }

    

    public class RaceCalender
    {
        public MRData MRData;
    }

    public class MRData
    {
        public string xmlns;
        public string series;
        public string url;
        public int limit;
        public int offset;
        public int total;
        public RaceTable RaceTable;

    }

    public class RaceTable
    {
        public int? season;
        public List<Race> Races = new List<Race>();
    }

    public class Race
    {
        public int? season;
        public int? round;
        public string? url;
        public string? raceName;
        public Circuit Circuit;
        public string? date;
        public string? time;
        public RaceEvent FirstPractice;
        public RaceEvent SecondPractice;
        public RaceEvent ThirdPractice;
        public RaceEvent Qualifying;
        public DateTime dateTime;

    }

    public class Circuit
    {
        public string? circuitId;
        public string? url;
        public string? circuitName;
        public Location Location;
      
    }

    public class Location
    {
        public float? lat;
        public float? longi;
        public string? locality;
        public string? country;
    }

    public class RaceEvent
    {
        public string? date;
        public string? time;
    }
}