using System.Linq;
using System.Net;
using Shipyard.Results.Errors;
using Shipyard.Results.Internal;
using Shipyard.Results.Validation;

namespace Shipyard.Results
{
    /// <summary>
    /// Factory for creating result types.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Constructs an 400 BadRequest response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<T> BadRequest<T>(string message) =>
            new PayloadResult<T>(HttpStatusCode.BadRequest, new ErrorMessage(message));

        /// <summary>
        /// Constructs an 400 BadRequest response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult BadRequest(string message) =>
            new StatusResult(HttpStatusCode.BadRequest, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 400 BadRequest response with an object content payload and validation result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        public static IResult<T> BadRequest<T>(T payload, ValidationResult validationResult) =>
            new PayloadResult<T>(HttpStatusCode.BadRequest, payload, validationResult);

        /// <summary>
        /// Constructs a 400 BadRequest response with an object content payload and validation result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        public static IResult<T> BadRequest<T>(T payload, FluentValidation.Results.ValidationResult validationResult) =>
            new PayloadResult<T>(HttpStatusCode.BadRequest, payload, new ValidationResult
            {
                Errors = validationResult.Errors.Select((error) => new ValidationError
                {
                    Source = error.PropertyName.ToCamelCase(),
                    AttemptedValue = error.AttemptedValue,
                    Details = error.ErrorMessage
                }).ToList()
            });

        /// <summary>
        /// Constructs an 409 Conflict response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<T> Conflict<T>(string message) =>
            new PayloadResult<T>(HttpStatusCode.Conflict, new ErrorMessage(message));
        
        /// <summary>
        /// Constructs an 409 Conflict response with an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult Conflict(string message) =>
            new StatusResult(HttpStatusCode.Conflict, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 401 Unauthorized response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<T> UnAuthorized<T>(string message) =>
            new PayloadResult<T>(HttpStatusCode.Unauthorized, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 401 Unauthorized response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult UnAuthorized(string message) =>
            new StatusResult(HttpStatusCode.Unauthorized, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 404 NotFound response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<T> NotFound<T>(string message) =>
            new PayloadResult<T>(HttpStatusCode.NotFound, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 404 NotFound response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult NotFound(string message) =>
            new StatusResult(HttpStatusCode.NotFound, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 200 OK response with an object content payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static IResult<T> Ok<T>(T payload) => new PayloadResult<T>(HttpStatusCode.OK, payload);

        /// <summary>
        /// Constructs a 200 OK response
        /// </summary>
        /// <returns></returns>
        public static IResult Ok() => new StatusResult(HttpStatusCode.OK);

        /// <summary>
        /// Constructs a 204 NoContent response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IResult<T> NoContent<T>() => new PayloadResult<T>(HttpStatusCode.NoContent, new ErrorMessage(string.Empty));

        /// <summary>
        /// Constructs a 204 NoContent response.
        /// </summary>
        /// <returns></returns>
        public static IResult NoContent() => new StatusResult(HttpStatusCode.NoContent);

        /// <summary>
        /// Constructs an 500 Server Error response with an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult ServerError(string message) =>
            new StatusResult(HttpStatusCode.InternalServerError, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 500 Server Error response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<T> ServerError<T>(string message) =>
            new PayloadResult<T>(HttpStatusCode.InternalServerError, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 500 InternalServerError response with an error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<T> InternalServerError<T>(string message) => new PayloadResult<T>(HttpStatusCode.InternalServerError, new ErrorMessage(message));

        /// <summary>
        /// Constructs a 500 InternalServerError response with an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult InternalServerError(string message) => new StatusResult(HttpStatusCode.InternalServerError, new ErrorMessage(message));
    }

    internal static class ResultStringExtensions
    {
        /// <summary>
        /// Convert TitleCase string to CamelCase
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string stringToConvert)
        {
            if (stringToConvert == null || stringToConvert.Length < 2)
            {
                return stringToConvert;
            }
            return char.ToLower(stringToConvert[0]) + stringToConvert.Substring(1);
        }
    }
}
