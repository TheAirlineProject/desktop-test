﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirlinerModel
{
    //the class for an airliner manufacturer
    [Serializable]
    public class Manufacturer : ISerializable
    {
        #region Constructors and Destructors

        public Manufacturer(string name, string shortname, Country country, Boolean isReal)
        {
            Name = name;
            ShortName = shortname;
            Country = country;
            IsReal = isReal;
        }

        private Manufacturer(SerializationInfo info, StreamingContext ctxt)
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

        #region Public Properties

        [Versioning("country")]
        public Country Country { get; set; }

        [Versioning("isreal")]
        public Boolean IsReal { get; set; }

        [Versioning("logo")]
        public string Logo { get; set; }

        [Versioning("name")]
        public string Name { get; set; }

        [Versioning("shortname")]
        public string ShortName { get; set; }

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

        #endregion
    }

    //the collection of manufacturers
    public class Manufacturers
    {
        #region Static Fields

        private static List<Manufacturer> manufacturers = new List<Manufacturer>();

        #endregion

        #region Public Methods and Operators

        public static void AddManufacturer(Manufacturer manufacturer)
        {
            manufacturers.Add(manufacturer);
        }

        public static void Clear()
        {
            manufacturers = new List<Manufacturer>();
        }

        //returns a manufacturer

        //returns the list manufacturers
        public static List<Manufacturer> GetAllManufacturers()
        {
            return manufacturers;
        }

        public static Manufacturer GetManufacturer(string name)
        {
            return manufacturers.Find(m => m.Name == name || m.ShortName == name);
        }

        public static List<Manufacturer> GetManufacturers(Predicate<Manufacturer> match)
        {
            return manufacturers.FindAll(match);
        }

        #endregion

        //clears the list

        //adds a manufacturer to the collection
    }
}