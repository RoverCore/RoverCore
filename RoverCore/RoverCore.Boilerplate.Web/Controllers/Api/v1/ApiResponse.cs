using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace RoverCore.Boilerplate.Web.Controllers.Api.v1;

[DataContract]
public class ApiResponse
{
    [DataMember]
    public string Version { get { return "1"; } }

    [DataMember]
    public int StatusCode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int ErrorCount { get; set; } = 0;

    [DataMember(EmitDefaultValue = false)]
    public List<ApiError> Errors { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public object Result { get; set; }

    public ApiResponse(HttpStatusCode statusCode, object result = null, string errorMessage = null, ModelStateDictionary state = null)
    {
        StatusCode = (int)statusCode;
        Result = result;

        if (errorMessage?.Length > 0)
        {
            ErrorCount = 1;
            ErrorMessage = errorMessage;
        }

        if (state != null)
        {
            Errors = new List<ApiError>();

            foreach (var modelStateKey in state.Keys)
            {
                var modelStateVal = state[modelStateKey];

                if (modelStateVal.Errors.Count > 0)
                {
                    Errors.Add(new ApiError
                    {
                        Key = modelStateKey,
                        Errors = modelStateVal.Errors.Select(m => m.ErrorMessage).ToList()
                    });

                }
            }

        }
    }

}