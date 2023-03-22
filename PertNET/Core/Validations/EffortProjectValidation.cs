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

    using EasyPrototypingNET.LinqExpressions;
    using EasyPrototypingNET.Pattern;

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

        public Result<string> NotEmpty(Expression<Func<TViewModel, object>> expression, string message)
        {
            string result = string.Empty;
            bool resultValidError = false;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            string propertyValue = (string)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject);

            if (string.IsNullOrEmpty(propertyValue) == true)
            {
                result = $"Das Feld '{message}' darf nicht leer sein.";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
        }

        public Result<string> InRangeChapter(Expression<Func<TViewModel, object>> expression, int min, int max)
        {
            string result = string.Empty;
            bool resultValidError = false;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            int propertyValue = (int)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject, null);

            if (propertyValue.InRange(min, max) == false)
            {
                result = $"Der Wert für die Kapitelnummerierung muß zwischen {min} und {max} liegen";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
        }

        public Result<string> ValueGreaterThanZero(Expression<Func<TViewModel, object>> expression, double min, double max = 999)
        {
            string result = string.Empty;
            bool resultValidError = false;
            string propertyName = ExpressionPropertyName.For<TViewModel>(expression);
            string propertyValue = (string)validation.ThisObject.GetType().GetProperty(propertyName).GetValue(validation.ThisObject, null);

            if (string.IsNullOrEmpty(propertyValue) == false)
            {
                if (Convert.ToDouble(propertyValue).Between(min, max) == false)
                {
                    result = $"Der Wert für Aufwand muß größer {min} sein";
                    resultValidError = true;
                }
                else
                {
                    if (Convert.ToDouble(propertyValue) % 0.25 == 0)
                    {
                    }
                    else
                    {
                        result = $"Es können nur Schritte mit '0,25' eingegeben werden";
                        resultValidError = true;
                    }

                }
            }
            else
            {
                result = $"Der Wert für Aufwand muß größer {min} sein";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
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