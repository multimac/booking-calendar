using Booking.Common.Extensions;

namespace Booking.Common.Mvc.Models
{
    public enum ErrorCode
    {
        /* 0 - 29: System errors */
        Unknown = 0,
        InternalServerError,

        /* 30 - 49: Request errors */
        ParseError = 30,
        ValidationError,

        /* 50 - 69: Login errors */
        FailedLogin = 50,
        TwoFactorRequired,
    }

    public class ErrorModel : IModel
    {
        public ErrorCode? Code { get; set; } = ErrorCode.Unknown;
        public string Reason { get; set; } = null;

        public string Message { get; set; } = null;

        public ErrorModel[] InnerErrors { get; set; } = null;

        public ErrorModel() { }

        public void Normalize()
        {
            Code = Code ?? ErrorCode.Unknown;
            Reason = Reason.NullIfWhiteSpace();

            Message = Message.NullIfWhiteSpace();

            InnerErrors = InnerErrors ?? new ErrorModel[] { };
        }
    }
}