//-----------------------------------------------------------------------
// <copyright file="EffortProjectValidation.cs" company="Lifeprojects.de">
//     Class: EffortProjectValidation
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>11.07.2021</date>
//
// <summary>
// Static Validation Class zur Validierung von Feldinhalten für 
// die Invention per ViewModel
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    using EasyPrototypingNET.LinqExpressions;

    public class EffortProjectValidation<TViewModel> where TViewModel : class
    {
        private static EffortProjectValidation<TViewModel> validation;

        private TViewModel ThisObject { get; set; }

        public static EffortProjectValidation<TViewModel> This(TViewModel thisObject)
        {
            validation = new EffortProjectValidation<TViewModel>();
            validation.ThisObject = thisObject;
            return validation;
        }

        public string NotEmpty(Expression<Func<TViewModel, object>> expression, string message)
        {
            string result = string.Empty;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            string propertyValue = (string)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject);

            if (string.IsNullOrEmpty(propertyValue) == true)
            {
                result = $"Das Feld '{message}' darf nicht leer sein.";
            }

            return result;
        }

        public string InRangeChapter(Expression<Func<TViewModel, object>> expression, int min, int max)
        {
            string result = string.Empty;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            int propertyValue = (int)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject, null);

            if (propertyValue.InRange(min, max) == false)
            {
                result = $"Der Wert für die Kapitelnummerierung muß zwischen {min} und {max} liegen";
            }

            return result;
        }

        public string ValueGreaterThanZero(Expression<Func<TViewModel, object>> expression, double min, double max = 999)
        {
            string result = string.Empty;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            string propertyValue = (string)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject, null);

            if (string.IsNullOrEmpty(propertyValue) == false)
            {
                if (Convert.ToDouble(propertyValue).Between(min, max) == false)
                {
                    result = $"Der Wert für Aufwand muß größer {min} sein";
                }
                else
                {
                    if (Convert.ToDouble(propertyValue) % 0.25 == 0)
                    {
                    }
                    else
                    {
                        result = $"Es können nur Schritte mit '0,25' eingegeben werden";
                    }

                }
            }
            else
            {
                result = $"Der Wert für Aufwand muß größer {min} sein";
            }

            return result;
        }

        public string CheckValueStep25(Expression<Func<TViewModel, object>> expression, double step = 0.25)
        {
            string result = string.Empty;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            string propertyValue = (string)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject, null);

            if (Convert.ToDouble(propertyValue) > 0)
            {
                if (Convert.ToDouble(propertyValue) % 25 == 0)
                {

                }
                else
                {
                    result = $"Es können nur Schritte mit {step} eingegeben werden";
                }
            }

            return result;
        }
    }
}