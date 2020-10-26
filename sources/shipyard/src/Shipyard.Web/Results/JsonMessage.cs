namespace Shipyard.Web.Results
{
    /// <summary>
    /// JSON message payload for custom error responses.
    /// </summary>
    public class JsonMessage
    {
        public string Message { get; }

        public JsonMessage(string message)
        {
            Message = message;
        }
    }
}
