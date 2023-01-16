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
    // Abstracts error handling in controllers.
    // I don't want to repeat this code in each API function call when there is an error.
    protected IActionResult Problem(List<Error> errors)
    {
        // Handle validation errors only when NOT using Fluent Validation
        if (errors.Any(error => error.Type == ErrorType.Validation))
        {
            return HandleValidationProblem(errors);
        }
        
        // Handle service level errors only.
        if (errors.Any(error => error.Type == ErrorType.Conflict))
        {
            return HandleConflictProblem(errors);
        }
        
        // Handle not found errors (can also be service level errors)
        if (errors.Any(error => error.Type == ErrorType.NotFound))
        {
            return HandleConflictProblem(errors);
        }
        
        return Problem(statusCode: StatusCodes.Status500InternalServerError); 
    }

    private IActionResult HandleConflictProblem(List<Error> errors)
    {
        // Extract all the validation errors and return those only
        var validationErrors = errors.Where(error => error.Type == ErrorType.Conflict).ToList();
        // This is bad thing about globals. It's not intuitive to remember that setting
        // this here will automagically get picked up by the custom Problem Detail Factory.
        HttpContext.Items[HttpContextItemKeys.Errors] = validationErrors;
        return Problem(statusCode: StatusCodes.Status409Conflict);
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
    
    private IActionResult HandleNotFoundProblem(List<Error> errors)
    {
        // Extract all the validation errors and return those only
        var validationErrors = errors.Where(error => error.Type == ErrorType.NotFound).ToList();
        HttpContext.Items[HttpContextItemKeys.Errors] = validationErrors;
        return Problem(statusCode: StatusCodes.Status404NotFound);
    }
    
    private IActionResult HandleValidationProblem(List<Error> errors)
    {
        // Extract all the validation errors and return those only
        var validationErrors = errors.Where(error => error.Type == ErrorType.Validation);
        // This is bad thing about globals. It's not intuitive to remember that setting
        // this here will automagically get picked up by the custom Problem Detail Factory.
        HttpContext.Items[HttpContextItemKeys.Errors] = validationErrors;
        return Problem(statusCode: StatusCodes.Status400BadRequest);
    }
}