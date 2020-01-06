namespace Horseshoe.NET.Email.SMS
{
    public class SMSInfo
    {
        public string MobileNumber { get; set; }
        public Carrier? Carrier { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
        public Credential? Credential { get; set; }
    }
}
