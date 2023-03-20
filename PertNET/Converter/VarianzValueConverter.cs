//-----------------------------------------------------------------------
// <copyright file="VarianzValueConverter.cs" company="Lifeprojects.de">
//     Class: VarianzValueConverter
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>10.07.2022 10:13:57</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class VarianzValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "0.00";

            if (values.Length == 3)
            {
                double minEffort = 0;
                double maxEffort = 0;
                double factor = 0;
                double tryResult = 0;

                if (values[0] != null && double.TryParse(values[0].ToString(), out tryResult) == true)
                {
                    minEffort = System.Convert.ToDouble(values[0]);
                }

                if (values[1] != null && double.TryParse(values[1].ToString(), out tryResult) == true)
                {
                    maxEffort = System.Convert.ToDouble(values[1]);
                }

                if (values[2] != null && double.TryParse(values[2].ToString(), out tryResult) == true)
                {
                    factor = System.Convert.ToDouble(values[2]);
                }

                double standardVariation  = ((maxEffort * factor) - (minEffort * factor)) / 6;
                double varianz = standardVariation * standardVariation;
                result = varianz.ToString("0.00");
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back to what you had isn't supported.");
        }
    }
}
