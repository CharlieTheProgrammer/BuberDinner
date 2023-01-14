using ErrorOr;

namespace BuberDinner.Domain.Common.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        // This is what would be used in a production application to stop attackers from guessing logins.
        // But not using for now.
        public static Error LoginFailed = Error.Validation(
            code: "Auth.LoginFailed",
            description: "Login failed.");
        
        public static Error EmailNotFound = Error.Conflict(
            code: "Auth.EmailNotFound",
            description: "Email does not exist.");

        
        public static Error InvalidPassword = Error.Conflict(
            code: "Auth.InvalidPassword",
            description: "Incorrect Password.");
    } 
}