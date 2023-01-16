using System.Diagnostics;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace BuberDinner.Api.Common.Errors;

public class BuberDinnerProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options;

    public BuberDinnerProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        statusCode ??= 500;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        statusCode ??= 400;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        if (title != null)
        {
            // For validation problem details, don't overwrite the default title with null.
            problemDetails.Title = title;
        }

        ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

        return problemDetails;
    }

    private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
    {
        problemDetails.Status ??= statusCode;

        if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }

        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        if (httpContext?.Items[HttpContextItemKeys.Errors] is not null)
        {
            List<Error> errors = httpContext.Items[HttpContextItemKeys.Errors] as List<Error>;
            
            // Don't add the errors just yet because they don't contain any useful information yet.
            // problemDetails.Extensions.Add(HttpContextItemKeys.Errors, errors);
            problemDetails.Extensions.Add(HttpContextItemKeys.ErrorCodes, errors.Select(error => error.Code));
            
            // This assumes there is only 1 error message. However, there are additional scenarios.
            problemDetails.Detail = errors.FirstOrDefault().Description;
            
            // For example, let's assume the following scenario.
            // I am importing a large CSV. The CSV may contain multiple errors. All errors should be returned
            // in the response. How can we accomplish this?
            // This one is tricky because we'd have to break it up into 2 steps.
            // Step 1 is to validate each row's data attributes.
            // Step 2 is to validate the business rules. 

            // I think instead of trying to come up with a generic solution, we can come up with a special response
            // for that API in particular. The reason is that we would need some kind of indexing between the errors
            // and the rows in the csv anyway. I would return 1 400 generic response + an additional custom mapping for 
            // validation errors. I would also do the same for a 409 error for the business rules.
            
            // Btw, anything that is JSON serializable can be added. This means Lists and Dictionaries.
            // Dictionaries can also be nested with other values.
            Dictionary<string, string> multiErrors = new Dictionary<string, string>
            {
                { "User.DuplicateEmail", "Email is already in use." },
                { "User.AnotherOne", "Another detailed error message." }
            };
            problemDetails.Extensions.Add("customErrors", multiErrors);
        }
    }
}
