namespace Multiblog.Utilities.Exception
{
    public class WebMessageException : System.Exception
    {
        public WebMessageException()
        { }
        
        public WebMessageException(string message, int statusCode = 500)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public WebMessageType Type { get; set; } = WebMessageType.Error;
        public string HelpUrl { get; set; }
        public int StatusCode { get; set; } = 500;
    }

    public enum WebMessageType
    {
        Info,
        WrongFormat,
        Error
    }
}
