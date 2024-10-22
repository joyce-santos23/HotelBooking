namespace Application
{
    public enum ErrorCode
    {
        //Guest
        NOT_FOUND = 1,
        COULD_NOT_STORE_DATA = 2,
    }
    public abstract class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ErrorCode ErrorCode { get; set; }

    }
}
