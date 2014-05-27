﻿namespace TheAirline.Model.AirlinerModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using TheAirline.Model.AirlineModel;
    using TheAirline.Model.AirlinerModel.RouteModel;
    using TheAirline.Model.AirportModel;
    using TheAirline.Model.GeneralModel;
    using TheAirline.Model.PilotModel;

    [Serializable]
    public class FleetAirliner : ISerializable
    {
        #region Constructors and Destructors

        public FleetAirliner(
            PurchasedType purchased,
            DateTime purchasedDate,
            Airline airline,
            Airliner airliner,
            Airport homebase)
        {
            this.Airliner = airliner;
            this.Purchased = purchased;
            this.PurchasedDate = purchasedDate;
            this.Airliner.Airline = airline;
            this.Homebase = homebase;
            this.Name = airliner.TailNumber;
            this.Statistics = new AirlinerStatistics(this);
            this.LastCMaintenance = this.Airliner.BuiltDate;
            this.LastAMaintenance = this.Airliner.BuiltDate;
            this.LastBMaintenance = this.Airliner.BuiltDate;
            this.LastDMaintenance = this.Airliner.BuiltDate;
            this.Status = AirlinerStatus.Stopped;
            this.MaintRoutes = new List<Route>();

            this.CurrentPosition = this.Homebase;
                //new GeoCoordinate(this.Homebase.Profile.Coordinates.Latitude,this.Homebase.Profile.Coordinates.Longitude);

            this.Routes = new List<Route>();
            this.Pilots = new List<Pilot>();
            this.InsurancePolicies = new List<AirlinerInsurance>();
            this.MaintenanceHistory = new Dictionary<Invoice, string>();
        }

        private FleetAirliner(SerializationInfo info, StreamingContext ctxt)
        {
            int version = info.GetInt16("version");

            IEnumerable<FieldInfo> fields =
                this.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute(typeof(Versioning)) != null);

            IList<PropertyInfo> props =
                new List<PropertyInfo>(
                    this.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            IEnumerable<MemberInfo> propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (SerializationEntry entry in info)
            {
                MemberInfo prop =
                    propsAndFields.FirstOrDefault(
                        p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Name == entry.Name);

                if (prop != null)
                {
                    if (prop is FieldInfo)
                    {
                        ((FieldInfo)prop).SetValue(this, entry.Value);
                    }
                    else
                    {
                        ((PropertyInfo)prop).SetValue(this, entry.Value);
                    }
                }
            }

            IEnumerable<MemberInfo> notSetProps =
                propsAndFields.Where(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Version > version);

            foreach (MemberInfo notSet in notSetProps)
            {
                var ver = (Versioning)notSet.GetCustomAttribute(typeof(Versioning));

                if (ver.AutoGenerated)
                {
                    if (notSet is FieldInfo)
                    {
                        ((FieldInfo)notSet).SetValue(this, ver.DefaultValue);
                    }
                    else
                    {
                        ((PropertyInfo)notSet).SetValue(this, ver.DefaultValue);
                    }
                }
            }
        }

        #endregion

        #region Enums

        public enum AirlinerStatus
        {
            Stopped,

            On_route,

            On_service,

            Resting,

            To_homebase,

            To_route_start
        }

        public enum PurchasedType
        {
            Bought,

            Leased,

            BoughtDownPayment
        }

        #endregion

        #region Public Properties

        [Versioning("ainterval")]
        public int AMaintenanceInterval { get; set; }

        [Versioning("airliner")]
        public Airliner Airliner { get; set; }

        [Versioning("binterval")]
        public int BMaintenanceInterval { get; set; }

        [Versioning("cinterval")]
        public int CMaintenanceInterval { get; set; }

        [Versioning("currentflight")]
        public Flight CurrentFlight { get; set; }

        [Versioning("currentposition")]
        public Airport CurrentPosition { get; set; }

        [Versioning("dinterval")]
        public int DMaintenanceInterval { get; set; }

        [Versioning("duecmaintenance")]
        public DateTime DueCMaintenance { get; set; }

        [Versioning("duedmaintenance")]
        public DateTime DueDMaintenance { get; set; }

        [Versioning("groundedto")]
        public DateTime GroundedToDate { get; set; }

        public Boolean HasRoute
        {
            get
            {
                return this.Routes.Count > 0;
            }
            set
            {
                ;
            }
        }

        [Versioning("homebase")]
        public Airport Homebase { get; set; }

        [Versioning("insurancepolicies")]
        public List<AirlinerInsurance> InsurancePolicies { get; set; }

        [Versioning("lastamaintenance")]
        public DateTime LastAMaintenance { get; set; }

        [Versioning("lastbmaintenance")]
        public DateTime LastBMaintenance { get; set; }

        [Versioning("lastcmaintenance")]
        public DateTime LastCMaintenance { get; set; }

        [Versioning("lastdmaintenance")]
        public DateTime LastDMaintenance { get; set; }

        [Versioning("maintroutes")]
        public List<Route> MaintRoutes { get; set; }

        [Versioning("maintenancehistory")]
        public IDictionary<Invoice, String> MaintenanceHistory { get; set; }

        [Versioning("name")]
        public string Name { get; set; }

        public int NumberOfPilots
        {
            get
            {
                return this.Pilots.Count;
            }
            private set
            {
                ;
            }
        }

        [Versioning("oosdate")]
        public DateTime OOSDate { get; set; }

        [Versioning("pilots")]
        public List<Pilot> Pilots { get; set; }

        [Versioning("purchased")]
        public PurchasedType Purchased { get; set; }

        [Versioning("date")]
        public DateTime PurchasedDate { get; set; }

        [Versioning("routes")]
        public List<Route> Routes { get; private set; }

        [Versioning("schedamaintenance")]
        public DateTime SchedAMaintenance { get; set; }

        [Versioning("schedbmaintenance")]
        public DateTime SchedBMaintenance { get; set; }

        [Versioning("schedcmaintenance")]
        public DateTime SchedCMaintenance { get; set; }

        [Versioning("scheddmaintenance")]
        public DateTime SchedDMaintenance { get; set; }

        [Versioning("statistics")]
        public AirlinerStatistics Statistics { get; set; }

        [Versioning("status")]
        public AirlinerStatus Status { get; set; }

        #endregion

        #region Public Methods and Operators

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 1);

            Type myType = this.GetType();

            IEnumerable<FieldInfo> fields =
                myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute(typeof(Versioning)) != null);

            IList<PropertyInfo> props =
                new List<PropertyInfo>(
                    myType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            IEnumerable<MemberInfo> propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (MemberInfo member in propsAndFields)
            {
                object propValue;

                if (member is FieldInfo)
                {
                    propValue = ((FieldInfo)member).GetValue(this);
                }
                else
                {
                    propValue = ((PropertyInfo)member).GetValue(this, null);
                }

                var att = (Versioning)member.GetCustomAttribute(typeof(Versioning));

                info.AddValue(att.Name, propValue);
            }
        }

        //adds a pilot to the airliner
        public void addPilot(Pilot pilot)
        {
            lock (this.Pilots)
            {
                this.Pilots.Add(pilot);
                pilot.Airliner = this;
            }
        }

        //removes a pilot from the airliner

        //adds a route to the airliner
        public void addRoute(Route route)
        {
            this.Routes.Add(route);
        }

        public void removePilot(Pilot pilot)
        {
            lock (this.Pilots)
            {
                this.Pilots.Remove(pilot);
                pilot.Airliner = null;
            }
        }

        //removes a route from the airliner
        public void removeRoute(Route route)
        {
            this.Routes.Remove(route);

            route.TimeTable.Entries.RemoveAll(e => e.Airliner == this);
        }

        #endregion
    }
}