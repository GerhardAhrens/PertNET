//-----------------------------------------------------------------------
// <copyright file="WorkUserInfoValidation.cs" company="Lifeprojects.de">
//     Class: WorkUserInfoValidation
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

namespace PERT.Core
{
    using System;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    using EasyPrototypingNET.LinqExpressions;
    using EasyPrototypingNET.Pattern;

    public class WorkUserInfoValidation<TViewModel> where TViewModel : class
    {
        private static WorkUserInfoValidation<TViewModel> validation;

        private TViewModel ThisObject { get; set; }

        public static WorkUserInfoValidation<TViewModel> This(TViewModel thisObject)
        {
            validation = new WorkUserInfoValidation<TViewModel>();
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
    }
}