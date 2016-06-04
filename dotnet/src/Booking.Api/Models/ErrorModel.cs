using Booking.Common.Extensions;

namespace Booking.Api.Models
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
        public ErrorModel(ErrorCode code)
        {
            this.Code = code;
            this.Reason = code.ToString();
        }
        public ErrorModel(ErrorCode code, string message) : this(code)
        {
            this.Message = message;
        }
        public ErrorModel(ErrorCode code, string message, ErrorModel[] innerErrors) : this(code, message)
        {
            this.InnerErrors = innerErrors;
        }
        
        public void Normalize()
        {
            Code = Code ?? ErrorCode.Unknown;
            Reason = Reason.NullIfWhiteSpace();
            
            Message = Message.NullIfWhiteSpace();
            
            InnerErrors = (InnerErrors.Length == 0 ? null : InnerErrors);
        }
    }
}