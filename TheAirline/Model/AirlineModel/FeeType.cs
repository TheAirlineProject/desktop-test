﻿namespace TheAirline.Model.AirlineModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using TheAirline.Model.GeneralModel;

    /*! Fee Type.
     * This class is used for a fee for an airline.
     * The class needs parameters for type, name, defaultvalue, minvalue, maxvalue, percentage
     */

    [Serializable]
    public class FeeType : ISerializable
    {
        #region Fields

        [Versioning("default")]
        private double ADefaultValue;

        [Versioning("maxvalue")]
        private double AMaxValue;

        [Versioning("minvalue")]
        private double AMinValue;

        #endregion

        #region Constructors and Destructors

        public FeeType(
            eFeeType type,
            string name,
            double defaultValue,
            double minValue,
            double maxValue,
            int percentage,
            int fromYear)
        {
            this.Type = type;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.DefaultValue = defaultValue;
            this.Name = name;
            this.Percentage = percentage;
            this.FromYear = fromYear;
        }

        public FeeType(
            eFeeType type,
            string name,
            double defaultValue,
            double minValue,
            double maxValue,
            int percentage)
            : this(type, name, defaultValue, minValue, maxValue, percentage, 1900)
        {
        }

        private FeeType(SerializationInfo info, StreamingContext ctxt)
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

        public enum eFeeType
        {
            Fee,

            Wage,

            FoodDrinks,

            Discount
        }

        #endregion

        #region Public Properties

        public double DefaultValue
        {
            get
            {
                return GeneralHelpers.GetInflationPrice(this.ADefaultValue);
            }
            set
            {
                this.ADefaultValue = value;
            }
        }

        [Versioning("fromyear")]
        public int FromYear { get; set; }

        public double MaxValue
        {
            get
            {
                return GeneralHelpers.GetInflationPrice(this.AMaxValue);
            }
            set
            {
                this.AMaxValue = value;
            }
        }

        public double MinValue
        {
            get
            {
                return GeneralHelpers.GetInflationPrice(this.AMinValue);
            }
            set
            {
                this.AMinValue = value;
            }
        }

        [Versioning("name")]
        public string Name { get; set; }

        [Versioning("percentage")]
        public int Percentage { get; set; }

        [Versioning("type")]
        public eFeeType Type { get; set; }

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

        #endregion
    }

    public class FeeTypes
    {
        #region Static Fields

        private static Dictionary<string, FeeType> types = new Dictionary<string, FeeType>();

        #endregion

        //adds a type to the list

        #region Public Methods and Operators

        public static void AddType(FeeType type)
        {
            types.Add(type.Name, type);
        }

        //clears the list
        public static void Clear()
        {
            types = new Dictionary<string, FeeType>();
        }

        public static FeeType GetType(string name)
        {
            return types[name];
        }

        //returns the list of fees of a specific type
        public static List<FeeType> GetTypes(FeeType.eFeeType type)
        {
            return GetTypes().FindAll(delegate(FeeType t) { return t.Type == type; });
        }

        //returns the list of fee types
        public static List<FeeType> GetTypes()
        {
            return types.Values.ToList();
        }

        #endregion

        //returns a fee type
    }
}