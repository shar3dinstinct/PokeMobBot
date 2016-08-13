#region using directives

#region using directives

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using PoGo.PokeMobBot.Logic.Utils;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;
using System.Collections.Generic;
using PoGo.PokeMobBot.Logic.State;
using System.Diagnostics;

#endregion

// ReSharper disable RedundantAssignment

#endregion



namespace PoGo.PokeMobBot.Logic
{
    public class HumanNavigation
    {
        private readonly Client _client;
        public HumanNavigation(Client client)
        {
            _client = client;
        }

        public async Task<PlayerUpdateResponse> Move(GeoCoordinate destination, double walkingSpeedInKilometersPerHour, Func<Task<bool>> functionExecutedWhileWalking, Func<Task<bool>> functionExecutedWhileWalking2,
            CancellationToken cancellationToken, ISession session)
        {
            //double metersPerInterval = 0.5; //approximate meters for each interval/waypoint to be spaced from the last.

            ////initial coordinate generaton
            GeoCoordinate currentLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude, _client.CurrentAltitude);
            ////prepare the result object for further manipulation + return
            PlayerUpdateResponse result = new PlayerUpdateResponse();
            ////get distance ofc
            //var distanceToTarget = LocationUtils.CalculateDistanceInMeters(currentLocation, destination);
            ////initial time
            //var requestSendDateTime = DateTime.Now;


            ////create segments
            //var segments = Math.Floor(distanceToTarget / metersPerInterval);
            List<GeoCoordinate> waypoints = new List<GeoCoordinate>();
            //get differences in lat/long
            //var latDiff = Math.Abs(currentLocation.Latitude - destination.Latitude);
            //var lonDiff = Math.Abs(currentLocation.Longitude - destination.Longitude);
            //var latAdd = latDiff / segments;
            //var lonAdd = latDiff / segments;
            //var lastLat = currentLocation.Latitude;
            //var lastLon = currentLocation.Longitude;
            //generate waypoints old code - goes in straight line basically
            //for (int i = 0; i < segments; i++)
            //{
            //    //TODO: add altitude calculations into everything
            //    lastLat += latAdd;
            //    lastLon += lonAdd;
            //    waypoints.Add(new GeoCoordinate(lastLat, lastLon, currentLocation.Altitude));
            //}

            //TODO: refactor the generation of waypoint code to break the waypoints given to us by the routing information down into segements like above.
            //generate waypoints new code
            var RoutingResponse = Routing.GetRoute(currentLocation, destination);
            foreach(var item in RoutingResponse.coordinates)
            {
                //0 = lat, 1 = long (MAYBE NOT THO?)
                waypoints.Add(new GeoCoordinate(item.ToArray()[1], item.ToArray()[0]));
            }

            //var timeSinceMoveStart = DateTime.Now.Ticks;
            //double curAcceleration = 1.66; //Lets assume we accelerate at 1.66 m/s ish. TODO: Fuzz this a bit
            //double curWalkingSpeed = 0;
            //double maxWalkingSpeed = (session.LogicSettings.WalkingSpeedInKilometerPerHour / 3.6); //Get movement speed in meters

            ////TODO: Maybe update SensorInfo to replicate/mimic movement, or is it fine as is?
            //bool StopWalking = false;
            //double TimeToAccelerate = GetAccelerationTime(curWalkingSpeed, maxWalkingSpeed, curAcceleration);
            ////double InitialMove = getDistanceTraveledAccelerating(TimeToAccelerate, curAcceleration, curWalkingSpeed);


            //double MoveLeft = curWalkingSpeed;
            //bool NeedMoreMove = false;
            //bool StopMove = false;
            //int UpdateInterval = 1; // in seconds - any more than this breaks the calculations for distance and such. It all relys on taking ~1 second to perform the actions needed, pretty much.

            //makes you appear to move slower if you're catching pokemon, hitting stops, etc.
            //This feels like more human behavior. Dunnomateee
            Navigation navi = new Navigation(_client);
            
            //MILD REWRITE TO USE HUMANPATHWALKING;
            for (int x = 0; x < waypoints.Count; x++)
            {
                await navi.HumanPathWalking(waypoints.ToArray()[x], session.LogicSettings.WalkingSpeedInKilometerPerHour,
                    functionExecutedWhileWalking, functionExecutedWhileWalking2, cancellationToken);
                UpdatePositionEvent?.Invoke(waypoints.ToArray()[x].Latitude, waypoints.ToArray()[x].Longitude);
                //Console.WriteLine("Hit waypoint " + x);
            }
            var curcoord = new GeoCoordinate(session.Client.CurrentLatitude, session.Client.CurrentLongitude);
            if(LocationUtils.CalculateDistanceInMeters(curcoord,destination) > 40)
            {
                await navi.HumanPathWalking(destination, session.LogicSettings.WalkingSpeedInKilometerPerHour,
                    functionExecutedWhileWalking, functionExecutedWhileWalking2, cancellationToken);
            }


            //var MovePerInterval = maxWalkingSpeed * UpdateInterval;
            //Console.WriteLine("Starting main movement loop");
            //bool moved = false;
            //double timeDeficit = 0;
            //var watch2 = new Stopwatch();
            //bool ScannedForPokemon = false;
            //watch2.Start();
            //while (!StopMove)
            //{
                
            //    var pointsHit = 0;
            //    var curWaypoint = waypoints.ToArray()[0];
            //    var distToNext = LocationUtils.CalculateDistanceInMeters(currentLocation, curWaypoint);
            //    if (MoveLeft > distToNext)
            //        NeedMoreMove = false;

            //    while (!NeedMoreMove)
            //    {


            //        curWaypoint = waypoints.ToArray()[0];
            //        distToNext = LocationUtils.CalculateDistanceInMeters(currentLocation, curWaypoint);

            //        if (distToNext <= MoveLeft)
            //        {

            //            var timestamp = DateTime.Now.ToString("H-mm-ss-fff");
            //            Console.WriteLine($"{timestamp} moved from {currentLocation.Latitude}, {currentLocation.Longitude} to {curWaypoint.Latitude}, {curWaypoint.Longitude}, \n  {distToNext} meters moved {waypoints.Count} remaining.");

            //            currentLocation = curWaypoint;
            //            MoveLeft -= distToNext;
            //            waypoints.RemoveAt(0);
            //            moved = true;
            //        }
            //        else
            //        {
            //            if (moved)
            //            {
            //                Stopwatch watch = new Stopwatch();
            //                watch.Start();
            //                if (RuntimeSettings.CheckScan())
            //                {
            //                    await functionExecutedWhileWalking();
            //                    ScannedForPokemon = true;
            //                }
            //                result = await _client.Player.UpdatePlayerLocation(currentLocation.Latitude, currentLocation.Longitude, currentLocation.Altitude);

            //                UpdatePositionEvent?.Invoke(currentLocation.Latitude, currentLocation.Longitude);
            //                moved = false;
            //                watch.Stop();
            //                timeDeficit = watch.ElapsedMilliseconds;
            //            }
            //            NeedMoreMove = true;
            //        }

            //        if (waypoints.Count == 0)//arbitrary number
            //        {
            //            break;
            //        }
            //    }

            //    //increase curmovement by acceleration and also add x amount of time
            //    if(curWalkingSpeed < maxWalkingSpeed)
            //    {//we need to add our acceleration to the current walking speed, capping it at maxWalkingSpeed
            //        if((maxWalkingSpeed - curWalkingSpeed) < curAcceleration)
            //            curWalkingSpeed = maxWalkingSpeed;
            //        else
            //            curWalkingSpeed += curAcceleration;
            //    }
            //    if (NeedMoreMove)
            //    {
            //        MoveLeft += curWalkingSpeed;
            //        NeedMoreMove = false;
            //    }
            //    if (waypoints.Count == 0)
            //    {
            //        result = await _client.Player.UpdatePlayerLocation(currentLocation.Latitude, currentLocation.Longitude, currentLocation.Altitude);

            //        UpdatePositionEvent?.Invoke(currentLocation.Latitude, currentLocation.Longitude);
            //        StopMove = true;
            //        break;
            //    }

            //    if (!ScannedForPokemon)
            //    {
            //        Thread.Sleep(UpdateInterval * 1000);
            //    }
            //    ScannedForPokemon = false;
            //    timeDeficit = 0;
            //}
            //watch2.Stop();

            return result;


        }

        public static double GetAccelerationTime(double curV, double maxV, double acc)
        {
            if (acc == 0)
                return 9001;
            else
                return (maxV - curV) / acc;
        }

        public static double getDistanceTraveledAccelerating(double time, double acc, double curV)
        {
            return ((curV * time) + ((acc * Math.Pow(time, 2)) / 2));
        }

        public event UpdatePositionDelegate UpdatePositionEvent;
    }
}
