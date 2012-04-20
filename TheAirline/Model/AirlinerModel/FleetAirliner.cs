﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirportModel;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.Model.GeneralModel.StatisticsModel;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirlinerModel
{
    public class FleetAirliner
    {
        public Airliner Airliner { get; set; }
        public string Name { get; set; }
        public Airport Homebase { get; set; }
        public enum PurchasedType { Bought, Leased,BoughtDownPayment }
        public PurchasedType Purchased { get; set; }
        public Boolean HasRoute { get { return this.Routes.Count > 0; } set { ;} }//{ get { return this.Route != null; } set { ;} }
        public AirlinerStatistics Statistics { get; set; }

        /*Changed for deleting routeairliner*/
        public enum AirlinerStatus { Stopped, On_route, On_service, Resting, To_homebase, To_route_start }
        public AirlinerStatus Status { get; set; }
        public Coordinates CurrentPosition { get; set; }
        //sprivate Route pRoute;
        //public Route Route { get { return pRoute; } set { setRoute(value); } }
        public List<Route> Routes { get; private set; }
        public Flight CurrentFlight { get; set; }
        public Boolean IsOnTime { get; set; }   
        
        public FleetAirliner(PurchasedType purchased, Airline airline,Airliner airliner, string name, Airport homebase)
        {
            this.Airliner = airliner;
            this.Purchased = purchased;
            this.Airliner.Airline = airline;
            this.Homebase = homebase;
            this.Name = name;
            this.Statistics = new AirlinerStatistics(this);
            this.IsOnTime = true;
            this.Status = AirlinerStatus.Stopped;

            this.CurrentPosition = new Coordinates(this.Homebase.Profile.Coordinates.Latitude, this.Homebase.Profile.Coordinates.Longitude);

            this.Routes = new List<Route>();
        }
        //adds a route to the airliner
        public void addRoute(Route route)
        {
            this.Routes.Add(route);

            foreach (RouteTimeTableEntry e in route.TimeTable.Entries)
                e.Airliner = this;
        }
        //removes a route from the airliner
        public void removeRoute(Route route)
        {
            this.Routes.Remove(route);

            foreach (RouteTimeTableEntry e in route.TimeTable.Entries.FindAll(t => t.Airliner == this))
                e.Airliner = null;

        }
        /*
        //sets the route
        private void setRoute(Route value)
        {
            pRoute = value;

            foreach (RouteTimeTableEntry e in this.Route.TimeTable.Entries)
                e.Airliner = this;
        }
         * */
        /*
        //returns the next destination
        public Airport getNextDestination()
        {
            return this.CurrentFlight.Entry.Destination.Airport == this.Route.Destination1 ? this.Route.Destination2 : this.Route.Destination1;
        }
        //returns the departure location
        public Airport getDepartureAirport()
        {
            return getNextDestination();

        }
         * */
    }
  
}
