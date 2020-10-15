namespace PortAuthority.Results.Validation
{
    /// <summary>
    /// Individual validation error
    /// </summary>
    public class ValidationError
    {
        public string Source { get; set; }
        public string Details { get; set; }
        public object AttemptedValue { get; set; }
        
        public override string ToString()
        {
            return $"Source: {Source}, Detail: {Details}, Attempted Value: {AttemptedValue}";
        }
    }
}
