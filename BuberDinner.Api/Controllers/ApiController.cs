using BuberDinner.Api.Common;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Authorize]
public class ApiController : ControllerBase
{
    // Abstracts error handling in controllers. I don't want to repeat this code in each API function call when
    // there is an error.
    protected IActionResult Problem(List<Error> errors)
    {
        
        // This needs to be refactored to handle different types of errors.
        
        // Handle validation errors only when NOT using Fluent Validation
        // if (errors.Any(error => error.Type == ErrorType.Validation))
        // {
        //     // Extract all the validation errors and return those only
        //     var validationErrors = errors.Where(error => error.Type == ErrorType.Validation);
        //     // This is bad thing about globals. It's not intuitive to remember that setting
        //     // this here will automagically get picked up by the custom Problem Detail Factory.
        //     HttpContext.Items[HttpContextItemKeys.Errors] = validationErrors;
        //     var statusCode = StatusCodes.Status400BadRequest;
        //     return Problem(statusCode: statusCode);
        // }
        
        // Handle service level errors only.
        if (errors.Any(error => error.Type == ErrorType.Conflict))
        {
            // Extract all the validation errors and return those only
            var validationErrors = errors.Where(error => error.Type == ErrorType.Conflict).ToList();
            // This is bad thing about globals. It's not intuitive to remember that setting
            // this here will automagically get picked up by the custom Problem Detail Factory.
            HttpContext.Items[HttpContextItemKeys.Errors] = validationErrors;
            var statusCode = StatusCodes.Status409Conflict;
            return Problem(statusCode: statusCode);
        }
        
        
        // HttpContext.Items[HttpContextItemKeys.Errors] = errors;
        //
        // var firstError = errors[0];
        //
        // var statusCode = firstError.Type switch
        // {
        //     ErrorType.Conflict => StatusCodes.Status409Conflict,
        //     ErrorType.Validation => StatusCodes.Status400BadRequest,
        //     ErrorType.NotFound => StatusCodes.Status404NotFound,
        //     _ => StatusCodes.Status500InternalServerError
        // };
        
        return Problem(statusCode: StatusCodes.Status500InternalServerError); 
    }

    protected IActionResult ValidationProblem(ValidationResult validationResult)
    {
        // Need to convert the validation result to model state and return ValidationProblem
        ModelStateDictionary modelState = new ModelStateDictionary();
        
        List<ValidationFailure> validationFailures = validationResult.Errors;

        foreach (ValidationFailure error in validationFailures)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        
        return ValidationProblem(modelState);
    }
}