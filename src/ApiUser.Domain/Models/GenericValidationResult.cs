using System.Net;

namespace ApiUser.Domain.Models;

public class GenericValidationResult 
{
    public GenericValidationResult(HttpStatusCode statusCode, string message )
    {
        StatusCode = statusCode;
        Message = message; 
    }
    public HttpStatusCode StatusCode { get; set; }

    public string Message { get; set; }
     
}
