using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Web.Extentions
{
    public static class ModelStateExtentions
    {
        public static List<string> GetAllErrors(this ModelStateDictionary modelState)
        {
            var result = new List<string>();
            var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any())
                                            .Select(x => new { x.Key, x.Value.Errors });

            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors
                                   .Select(error => error.ErrorMessage);
                result.AddRange(fieldErrors);
            }

            return result;
        }
        public static string GetSingleLineErrorMessages(this ModelStateDictionary modelstate)
        {
            return 
                string.Join(',', modelstate.Values.Where(v => v.Errors.Count > 0)
                      .SelectMany(v => v.Errors)
                      .Select(v => v.ErrorMessage));
        }
        public static string GetMultiLineErrorMessages(this ModelStateDictionary modelstate)
        {
            var message = string.Join("\\n", modelstate.Values
                                          .SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage));

            return message;
        }
        public static string GetHtmlMultiLineErrorMessages(this ModelStateDictionary modelState)
        {
            var message = string.Join("<br />", modelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));

            return message;
        }

    }
}
