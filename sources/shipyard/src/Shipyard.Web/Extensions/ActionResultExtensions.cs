using System;
using Microsoft.AspNetCore.Mvc;
using Shipyard.Extensions;
using Shipyard.Results;

namespace Shipyard.Web.Extensions
{
    /// <summary>
    /// Extensions for converting <see cref="IResult"/>'s to AspNet WebAPI results.
    /// </summary>
    public static class ActionResultExtensions
    {
        /// <summary>
        /// Converts common API Result responses to the appropriate action result.
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IActionResult ToActionResult<TPayload>(this IResult<TPayload> result)
        {
            if (result.IsNotFound())
            {
                return new NotFoundObjectResult(result.ErrorMessage);
            }

            if (result.IsValid())
            {
                return new BadRequestObjectResult(result.ErrorMessage);
            }

            return new OkObjectResult(result.Payload);
        }

        /// <summary>
        /// Converts common API Result responses to an action result, projecting the result payload to a new format.
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="result"></param>
        /// <param name="contentResultFactory"></param>
        /// <returns></returns>
        public static IActionResult ToActionResult<TPayload>(this IResult<TPayload> result,  Func<TPayload, object> contentResultFactory)
        {
            if (result.IsNotFound())
            {
                return new NotFoundObjectResult(result.ErrorMessage);
            }

            if (result.IsValid())
            {
                return new BadRequestObjectResult(result.ErrorMessage);
            }

            var content = contentResultFactory(result.Payload);
            return new OkObjectResult(content);
        }
    }
}
