using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PortAuthority.Results;

namespace PortAuthority.Extensions
{
    /// <summary>
    /// Extensions for testing <see cref="IResult"/> statuses.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Returns true if the result contains no validation errors.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsValid(this IResult result) => result.ValidationResult.IsValid();

        /// <summary>
        /// Returns true if the result has a BadRequest status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsBadRequest(this IResult result) => result.StatusCode == HttpStatusCode.BadRequest;

        /// <summary>
        /// Returns true if the result has a Conflict status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsConflict(this IResult result) => result.StatusCode == HttpStatusCode.Conflict;

        /// <summary>
        /// Returns true if the result has an Unauthorized status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsUnauthorized(this IResult result) => result.StatusCode == HttpStatusCode.Unauthorized;

        /// <summary>
        /// Returns true if the result has a NotFound status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsNotFound(this IResult result) => result.StatusCode == HttpStatusCode.NotFound;

        /// <summary>
        /// Returns true if the result has an OK status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsOk(this IResult result) => result.StatusCode == HttpStatusCode.OK;

        /// <summary>
        /// Returns true if the result has an OK status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsServerError(this IResult result) => result.StatusCode == HttpStatusCode.InternalServerError;

        /// <summary>
        /// Returns true if the result has a NoContent status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsNoContent(this IResult result) => result.StatusCode == HttpStatusCode.NoContent;

        /// <summary>
        /// Returns true if the result has a InternalServerError status code
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsInternalServerError(this IResult result) => result.StatusCode == HttpStatusCode.InternalServerError;
    }
}
