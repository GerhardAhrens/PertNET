//-----------------------------------------------------------------------
// <copyright file="CalcPERT.cs" company="Lifeprojects.de">
//     Class: CalcPERT
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>18.07.2022 08:30:25</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using EasyPrototypingNET.Pattern;

    public sealed class CalcPERT : DisposableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalcPERT"/> class.
        /// </summary>
        public CalcPERT()
        {
        }

        public double PERTEffort(double minEffort, double midEffort, double maxEffort, double factor)
        {
            double tempResult = ((minEffort + (midEffort * 4) + maxEffort) / 6) * factor;
            return tempResult;
        }

        public double StandardVariation(double minEffort, double maxEffort, double factor)
        {
            double tempResult = ((maxEffort * factor) - (minEffort * factor)) / 6;
            return tempResult;
        }

        public double VarianzValue(double minEffort, double maxEffort, double factor)
        {
            double standardVariation = ((maxEffort * factor) - (minEffort * factor)) / 6;
            double varianz = standardVariation * standardVariation;
            return varianz;
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
