using Newtonsoft.Json;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace CheckTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = "2000:2,5000:2,7000:2,10000:2";
            Delivery delivery = new Delivery();
            LinkedList<LinkedListNode<DriverSearchTier>> linkedList = new LinkedList<LinkedListNode<DriverSearchTier>>();
            foreach (var obj in config.Split(','))
            {
                DriverSearchTier driverSearchTier = new DriverSearchTier();
                var isConvetedd = double.TryParse(obj.Split(':')[0], out var radiusInMeter);
                if (isConvetedd)
                    driverSearchTier.RadiusInMeters = radiusInMeter;
                var isintconvert = int.TryParse(obj.Split(':')[1], out var runCycles);
                if (isintconvert)
                    driverSearchTier.RunCycles = runCycles;
                linkedList.AddLast(new LinkedListNode<DriverSearchTier>(driverSearchTier)
                {
                    Value = driverSearchTier
                });
            }
            delivery.driverSearchTiers = linkedList;
            delivery.driverSearchCurrentTier = linkedList.First().Value;
            var xyz = linkedList.GetEnumerator().MoveNext;
            //delivery.driverSearchCurrentTier = ;
            //var date = DateTime.UtcNow.AddSeconds(-100);
            //var inputs = new ProblemDelivery()
            //{
            //    LastEventName = "enroute_to_delivery",
            //    MovingAwayFromTime = date,
            //    SameLocationFromTime = DateTime.MinValue
            //};
            //var path = Directory.GetCurrentDirectory();
            //var files = Directory.GetFiles(path, "PDS.json", SearchOption.AllDirectories);
            //if (files == null || files.Length == 0)
            //    throw new Exception("Rules not found.");

            //var fileData = File.ReadAllText(files[0]);
            //var workflowRules = JsonConvert.DeserializeObject<List<WorkflowRules>>(fileData);

            //var bre = new RulesEngine.RulesEngine(workflowRules.ToArray(), null);

            //string discountOffered = "No discount offered.";

            //List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("ProblemDeliveryRules", inputs).Result;

            //if (resultList.Any(x => x.IsSuccess))
            //{
            //    var count = resultList.Count(x => x.IsSuccess);
            //    var result = resultList.Where(x => x.IsSuccess).OrderByDescending(x => x.Rule.SuccessEvent).ToList();
            //}
            //else
            //{

            //}

            //Console.WriteLine(discountOffered);
            //double flat = 29.5830267131779;
            //double flong = -98.2656787791801;
            //double slat = 29.5826556790684;
            //double slong = -98.2654315060773;
            //var result = Calculate(flat, flong, slat, slong).Result;
        }
        public static Task<double> Calculate(double fromLat, double fromLong, double toLat, double toLong, char unit = 'T')
        {
            if (Math.Abs(fromLat - toLat) < 0 && Math.Abs(fromLong - toLong) < 0) return Task.FromResult(0.0);

            var theta = fromLong - toLong;
            var dist = Math.Sin(deg2rad(fromLat)) * Math.Sin(deg2rad(toLat)) +
                       Math.Cos(deg2rad(fromLat)) * Math.Cos(deg2rad(toLat)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
                dist = dist * 1.609344;
            else if (unit == 'N')
                dist = dist * 0.8684;
            else if (unit == 'T') dist = dist * 1609.34;
            return Task.FromResult(dist);
        }
        private static double deg2rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return rad / Math.PI * 180.0;
        }
    }
    public class ProblemDelivery
    {
        public long JobId { get; set; }
        public long DriverId { get; set; }
        public string JobStatus { get; set; }
        public string LastEventName { get; set; }
        public DateTime LastEventDate { get; set; }
        public double TotalSecondsFromConfirmed { get { return (DateTime.UtcNow - LastEventDate).TotalSeconds; } }
        public DateTime SameLocationFromTime { get; set; }
        public double SameLocationDurationInSecond { get { return (DateTime.UtcNow - SameLocationFromTime).TotalSeconds; } }
        public DateTime MovingAwayFromTime { get; set; }
        public double MovingAwayDurationInSecond { get { return (DateTime.UtcNow - MovingAwayFromTime).TotalSeconds; } }
        public string AppliedRule { get; set; }
        public bool IsProblemDeliveryEnable { get; set; } = true;
        public bool IsNotFoundGeofenceAtPickupEvent { get; set; }
        public bool IsNotFoundGeofenceAtDropoffEvent { get; set; }
        public string RuleStatus { get; set; }
        public DateTime? ActualEta { get; set; }
    }
    public class DriverSearchTier
    {
        public double RadiusInMeters { get; set; } = 48280.30;
        public int RunCycles { get; set; } = 3;
    }
    public class Delivery
    {
        public LinkedList<LinkedListNode<DriverSearchTier>> driverSearchTiers { get; set; }
        public DriverSearchTier driverSearchCurrentTier { get; set; }
    }
}
