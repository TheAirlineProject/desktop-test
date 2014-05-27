﻿namespace TheAirline.Model.AirlinerModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using TheAirline.Model.GeneralModel;
    using TheAirline.Model.GeneralModel.StatisticsModel;

    //the class for the statistics for an airliner
    [Serializable]
    public class AirlinerStatistics : GeneralStatistics
    {
        #region Fields

        [Versioning("airliner")]
        private readonly FleetAirliner Airliner;

        #endregion

        #region Constructors and Destructors

        public AirlinerStatistics(FleetAirliner airliner)
        {
            this.Airliner = airliner;
        }

        private AirlinerStatistics(SerializationInfo info, StreamingContext ctxt)
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

        #region Public Properties

        public double Balance
        {
            get
            {
                return this.getBalance();
            }
            set
            {
                ;
            }
        }

        public double FillingDegree
        {
            get
            {
                return this.getFillingDegree();
            }
            set
            {
                ;
            }
        }

        public double IncomePerPassenger
        {
            get
            {
                return this.getIncomePerPassenger();
            }
            set
            {
                ;
            }
        }

        #endregion

        #region Public Methods and Operators

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
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

            base.GetObjectData(info, context);
        }

        #endregion

        #region Methods

        private double getBalance()
        {
            return this.getStatisticsValue(StatisticsTypes.GetStatisticsType("Airliner_Income"));
        }

        //get the degree of filling
        private double getFillingDegree()
        {
            double avgPassengers = this.getStatisticsValue(StatisticsTypes.GetStatisticsType("Passengers"))
                                   / this.getStatisticsValue(StatisticsTypes.GetStatisticsType("Arrivals"));

            double totalPassengers = Convert.ToDouble(this.Airliner.Airliner.getTotalSeatCapacity());

            double fillingDegree = avgPassengers / totalPassengers;

            if (totalPassengers == 0)
            {
                return 0;
            }
            return avgPassengers / totalPassengers;
        }

        //gets the income per passenger
        private double getIncomePerPassenger()
        {
            double totalPassengers =
                Convert.ToDouble(this.getStatisticsValue(StatisticsTypes.GetStatisticsType("Passengers")));

            if (totalPassengers == 0)
            {
                return 0;
            }
            return this.getBalance() / totalPassengers;
        }

        #endregion

        //gets the balance
    }
}