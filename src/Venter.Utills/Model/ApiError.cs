namespace Venter.Utilities.Model
{

    public class ApiError
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string Detail { get; set; }
        public int StatusCode { get; set; } = 500;

        public ApiError(string message, bool IsError = true)
        {
            this.Message = message;
            this.IsError = IsError;
        }        
    }
}