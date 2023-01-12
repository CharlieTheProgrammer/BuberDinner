using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuberDinner.Api.Filters;

public class ErrorHandlingFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        
        // Extract the error into a standard object
        // context.Result = new ObjectResult(new { error = "An error occurred while processing your request." })
        // {
        //     StatusCode = 500
        // };

        // ProblemDetails is a standard MS class that models RFC 7231
        var problemDetails = new ProblemDetails()
        {
            Title = "An error occurred while processing your request.",
            Status = (int)HttpStatusCode.InternalServerError
        };

        context.Result = new ObjectResult(problemDetails);

        context.ExceptionHandled = true;
    }
}