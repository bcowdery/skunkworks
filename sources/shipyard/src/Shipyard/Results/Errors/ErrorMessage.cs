namespace Shipyard.Results.Errors
{
    /// <summary>
    /// API error message
    /// </summary>
    public class ErrorMessage 
    {
        public ErrorMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
