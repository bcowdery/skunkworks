namespace PortAuthority.Results
{
    /// <summary>
    /// Result of an API operation with a response payload.
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public interface IResult<out TPayload> : 
        IResult
    {
        /// <summary>
        /// Response payload
        /// </summary>
        TPayload Payload { get; }
    }
}
