﻿using System;

namespace TheAirline.Model.GeneralModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
    public class Versioning : Attribute
    {
        #region Fields

        private readonly string _name;

        #endregion

        #region Constructors and Destructors

        public Versioning(string name)
        {
            _name = name;
            Version = 1;
            AutoGenerated = true;
        }

        #endregion

        #region Public Properties

        public bool AutoGenerated { get; set; }

        public object DefaultValue { get; set; }

        public string Name
        {
            get { return _name; }
        }

        public double Version { get; set; }

        #endregion

        // This property is readonly (it has no set accessor)
        // so it cannot be used as a named argument to this attribute.
    }
}