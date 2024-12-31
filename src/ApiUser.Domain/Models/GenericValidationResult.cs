using ApiUser.Domain.Entities;
using FluentValidation.Results;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiUser.Domain.Models
{
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
}
