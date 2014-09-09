﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirlinerModel
{
    //the class for an engine type 
    [Serializable]
    public class EngineType : ISerializable
    {
        #region Constructors and Destructors

        public EngineType(
            string model,
            Manufacturer manufacturer,
            TypeOfEngine engine,
            NoiseLevel noise,
            double consumptation,
            long price,
            int maxspeed,
            int ceiling,
            double runway,
            double range,
            Period<int> produced)
        {
            Model = model;
            Manufacturer = manufacturer;
            Engine = engine;
            ConsumptationModifier = consumptation;
            Price = price;
            MaxSpeed = maxspeed;
            Ceiling = ceiling;
            RunwayModifier = runway;
            RangeModifier = range;
            Produced = produced;
            Types = new List<AirlinerType>();
            Noise = noise;
        }

        private EngineType(SerializationInfo info, StreamingContext ctxt)
        {
            int version = info.GetInt16("version");

            IEnumerable<FieldInfo> fields =
                GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute(typeof (Versioning)) != null);

            IList<PropertyInfo> props =
                new List<PropertyInfo>(
                    GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(p => p.GetCustomAttribute(typeof (Versioning)) != null));

            IEnumerable<MemberInfo> propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (SerializationEntry entry in info)
            {
                MemberInfo prop =
                    propsAndFields.FirstOrDefault(
                        p => ((Versioning) p.GetCustomAttribute(typeof (Versioning))).Name == entry.Name);

                if (prop != null)
                {
                    if (prop is FieldInfo)
                    {
                        ((FieldInfo) prop).SetValue(this, entry.Value);
                    }
                    else
                    {
                        ((PropertyInfo) prop).SetValue(this, entry.Value);
                    }
                }
            }

            IEnumerable<MemberInfo> notSetProps =
                propsAndFields.Where(p => ((Versioning) p.GetCustomAttribute(typeof (Versioning))).Version > version);

            foreach (MemberInfo notSet in notSetProps)
            {
                var ver = (Versioning) notSet.GetCustomAttribute(typeof (Versioning));

                if (ver.AutoGenerated)
                {
                    if (notSet is FieldInfo)
                    {
                        ((FieldInfo) notSet).SetValue(this, ver.DefaultValue);
                    }
                    else
                    {
                        ((PropertyInfo) notSet).SetValue(this, ver.DefaultValue);
                    }
                }
            }
        }

        #endregion

        #region Enums

        public enum NoiseLevel
        {
            VeryHigh,

            High,

            Medium,

            Low,

            VeryLow
        }

        public enum TypeOfEngine
        {
            Jet,

            Turboprop
        }

        #endregion

        #region Public Properties

        [Versioning("ceiling")]
        public int Ceiling { get; set; }

        [Versioning("consumptation")]
        public double ConsumptationModifier { get; set; }

        [Versioning("engine")]
        public TypeOfEngine Engine { get; set; }

        [Versioning("manufacturer")]
        public Manufacturer Manufacturer { get; set; }

        [Versioning("speed")]
        public int MaxSpeed { get; set; }

        [Versioning("model")]
        public string Model { get; set; }

        [Versioning("noise")]
        public NoiseLevel Noise { get; set; }

        [Versioning("price")]
        public long Price { get; set; }

        [Versioning("produced")]
        public Period<int> Produced { get; set; }

        [Versioning("range")]
        public double RangeModifier { get; set; }

        [Versioning("runway")]
        public double RunwayModifier { get; set; }

        [Versioning("types")]
        public List<AirlinerType> Types { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 1);

            Type myType = GetType();

            IEnumerable<FieldInfo> fields =
                myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                      .Where(p => p.GetCustomAttribute(typeof (Versioning)) != null);

            IList<PropertyInfo> props =
                new List<PropertyInfo>(
                    myType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                          .Where(p => p.GetCustomAttribute(typeof (Versioning)) != null));

            IEnumerable<MemberInfo> propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (MemberInfo member in propsAndFields)
            {
                object propValue;

                if (member is FieldInfo)
                {
                    propValue = ((FieldInfo) member).GetValue(this);
                }
                else
                {
                    propValue = ((PropertyInfo) member).GetValue(this, null);
                }

                var att = (Versioning) member.GetCustomAttribute(typeof (Versioning));

                info.AddValue(att.Name, propValue);
            }
        }

        public void AddAirlinerType(AirlinerType type)
        {
            Types.Add(type);
        }

        #endregion
    }

    //the list of engine types
    public class EngineTypes
    {
        #region Static Fields

        private static List<EngineType> _types = new List<EngineType>();

        #endregion

        #region Public Methods and Operators

        public static void Clear()
        {
            _types = new List<EngineType>();
        }

        public static void AddEngineType(EngineType type)
        {
            _types.Add(type);
        }

        //returns the list of engine types
        public static List<EngineType> GetEngineTypes()
        {
            return _types;
        }

        //returns the list of engine types for an airliner type
        public static List<EngineType> GetEngineTypes(AirlinerType type, int year)
        {
            return GetEngineTypes(type).FindAll(t => t.Produced.From <= year && t.Produced.To >= year);
        }

        public static List<EngineType> GetEngineTypes(AirlinerType type)
        {
            var ttypes = new List<EngineType>();

            foreach (EngineType t in _types)
            {
                foreach (AirlinerType at in t.Types)
                    if (at.Name == type.Name)
                        ttypes.Add(t);
            }

            return ttypes;
        }

        //returns the standard engine for an airliner type
        public static EngineType GetStandardEngineType(AirlinerType type, int year)
        {
            List<EngineType> allTypes = GetEngineTypes(type, year);

            if (allTypes.Count > 0)
            {
                return allTypes.OrderBy(t => t.Price).First();
            }

            return null;
        }

        public static EngineType GetStandardEngineType(AirlinerType type)
        {
            List<EngineType> allTypes = GetEngineTypes(type);

            if (allTypes.Count > 0)
            {
                return allTypes.OrderBy(t => t.Price).First();
            }

            return null;
        }

        #endregion

        //adds an engine type to the list
    }
}