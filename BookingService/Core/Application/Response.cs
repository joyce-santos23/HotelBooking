namespace Application
{
    public enum ErrorCode
    {
        //Guest
        NOT_FOUND = 1,
        COULD_NOT_STORE_DATA = 2,
        INVALID_PERSON_ID = 3,
        MISSING_REQUIRED_INFORMATION = 4,
        INVALID_EMAIL = 5,
        GUEST_NOT_FOUND = 6,
        COULD_NOT_DELETE = 7,

        //Room
        ROOM_NOT_FOUND = 8,
        INVALID_PRICE = 9,
        ROOM_IN_MAINTENANCE = 10,
        ROOM_NOT_AVAILABLE = 11,
        CANNOT_DELETE_ROOM_WITH_BOOKINGS = 12,
        CANNOT_DELETE_GUEST_WITH_BOOKINGS = 13,


        //Booking
        BOOKING_NOT_FOUND = 14

        //Payment
    }
    public abstract class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ErrorCode ErrorCode { get; set; }

    }
}
