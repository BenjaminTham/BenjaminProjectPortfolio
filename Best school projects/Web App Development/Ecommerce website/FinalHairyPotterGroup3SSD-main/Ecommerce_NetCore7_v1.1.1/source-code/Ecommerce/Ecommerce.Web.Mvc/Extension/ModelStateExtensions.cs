using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;

namespace Ecommerce.Web.Mvc.Extension;

public static class ModelStateExtensions
{
    public static IDictionary ToSerializedDictionary(this ModelStateDictionary modelState)
    {
        return modelState.ToDictionary(k => k.Key, v => v.Value.Errors.Select(x => x.ErrorMessage).ToArray());
    }

    //public static IEnumerable ToSerializedDictionary(this ModelStateDictionary modelState)
    //{
    //    var modelErrors = new List<string>();
    //    foreach (var state in modelState.Values)
    //    {
    //        foreach (var s in state.Errors)
    //        {
    //            modelErrors.Add(s.ErrorMessage);
    //        }
    //    }
    //    return modelErrors;
    //}



    //public static IEnumerable ToSerializedDictionary(this ModelStateDictionary modelState)
    //{
    //    return modelState.Values.SelectMany(v => v.Errors.Select(x => x.ErrorMessage)).ToArray();
    //}
}
