namespace iScrapper
{
    public class ConvertBookingCodeRequest
    {
        public string BookingCode { get; set; }
        public XPlatforms Source { get; set; }
        public XPlatforms Destination { get; set; }

    }
}
