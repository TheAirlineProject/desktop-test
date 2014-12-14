﻿namespace TheAirline.Model.PilotModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using TheAirline.Model.AirlineModel;
    using TheAirline.Model.AirlinerModel;
    using TheAirline.Model.GeneralModel;

    //the class for a pilot
    [Serializable]
    public class Pilot : ISerializable
    {
        #region Constants

        public const int RetirementAge = 55;

        #endregion

        #region Constructors and Destructors

        public Pilot(PilotProfile profile, DateTime educationTime, PilotRating rating)
        {
            this.Profile = profile;
            this.EducationTime = educationTime;
            this.Rating = rating;
            this.Aircrafts = new List<string>();
            this.FlownHours = new TimeSpan();
        }

        private Pilot(SerializationInfo info, StreamingContext ctxt)
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
            if (version == 1)
            {
                this.Rating = GeneralHelpers.GetPilotRating();
            }
            if (version < 3)
            {
                this.Aircrafts = GeneralHelpers.GetPilotAircrafts(this);
                this.Training = null;
            }
            if (version < 4)
                this.FlownHours = new TimeSpan();
        }

        #endregion

        #region Public Properties

        [Versioning("aircrafts", Version = 3)]
        public List<string> Aircrafts { get; set; }

        [Versioning("airline")]
        public Airline Airline { get; set; }

        [Versioning("signeddate")]
        public DateTime AirlineSignedDate { get; set; }

        [Versioning("airliner")]
        public FleetAirliner Airliner { get; set; }

        [Versioning("education")]
        public DateTime EducationTime { get; set; }

        public Boolean OnTraining
        {
            get
            {
                return this.Training != null;
            }
            set
            {
            }
        }
        [Versioning("flown")]
        public TimeSpan FlownHours { get; set; }
        [Versioning("profile")]
        public PilotProfile Profile { get; set; }

        [Versioning("pilotrating", Version = 2)]
        public PilotRating Rating { get; set; }

        [Versioning("training", Version = 3)]
        public PilotTraining Training { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 4);

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

        //adds an airliner type family which the pilot expirence
        public void addAirlinerFamily(string family)
        {
            this.Aircrafts.Add(family);
        }

        //sets the airline for a pilot
        public void setAirline(Airline airline, DateTime signDate)
        {
            this.Airline = airline;
            this.AirlineSignedDate = signDate;
        }

        #endregion
    }

    //the list of pilots
    public class Pilots
    {
        #region Static Fields

        private static readonly List<Pilot> pilots = new List<Pilot>();

        #endregion

        //adds a pilot to the list

        #region Public Methods and Operators

        public static void AddPilot(Pilot pilot)
        {
            lock (pilots)
            {
                if (pilot != null)
                {
                    pilots.Add(pilot);
                }
            }
        }

        //clears the list of pilots
        public static void Clear()
        {
            pilots.Clear();
        }

        public static int GetNumberOfPilots()
        {
            return pilots.Count;
        }

        public static int GetNumberOfUnassignedPilots()
        {
            return GetUnassignedPilots().Count;
        }

        //returns all pilots
        public static List<Pilot> GetPilots()
        {
            return pilots;
        }

        //returns all unassigned pilots
        public static List<Pilot> GetUnassignedPilots()
        {
            List<Pilot> unassigned = pilots.FindAll(p => p.Airline == null);

            if (unassigned.Count < 5)
            {
                GeneralHelpers.CreatePilots(10);

                return GetUnassignedPilots();
            }

            return unassigned;
        }

        public static List<Pilot> GetUnassignedPilots(Predicate<Pilot> match)
        {
            return GetUnassignedPilots().FindAll(match);
        }

        //removes a pilot from the list
        public static void RemovePilot(Pilot pilot)
        {
            pilots.Remove(pilot);
        }

        #endregion

        //counts the number of unassigned pilots
    }
}