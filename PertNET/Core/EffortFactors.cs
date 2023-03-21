//-----------------------------------------------------------------------
// <copyright file="EffortFactors.cs" company="Lifeprojects.de">
//     Class: EffortFactors
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>08.07.2022 14:12:50</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{

    using EasyPrototypingNET.Pattern;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Versioning;

    [SupportedOSPlatform("windows")]
    public sealed class EffortFactors : DisposableBase
    {
        private readonly Dictionary<double,string> _factors;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffortFactors"/> class.
        /// </summary>
        public EffortFactors()
        {
            this._factors = new Dictionary<double,string>();
            if (this._factors.IsNullOrEmpty() == true)
            {
                this._factors.Add(0,string.Empty);
                this._factors.Add(0.25, "0.25");
                this._factors.Add(0.50, "0.50");
                this._factors.Add(0.75, "0.75");
                this._factors.Add(1.00, "1.00");
                this._factors.Add(1.25, "1.25");
                this._factors.Add(1.50, "1.50");
                this._factors.Add(1.75, "1.75");
                this._factors.Add(2.00, "2.00");
            }
        }

        public Dictionary<double, string> Get()
        {
            return this._factors;
        }

        public double GetByKey(int key)
        {
            double result = 0;
            bool rangeOK = key.InRange(0,this._factors.Count);
            if (rangeOK == true)
            {
                result = Convert.ToDouble(this._factors[key], CultureInfo.CurrentCulture);
            }

            return result;
        }

        protected override void DisposeManagedResources()
        {
            /* Behandeln von Managed Resources bem verlassen der Klasse */
        }

        protected override void DisposeUnmanagedResources()
        {
            /* Behandeln von UnManaged Resources bem verlassen der Klasse */
        }
    }
}
