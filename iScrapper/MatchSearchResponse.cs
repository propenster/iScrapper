namespace iScrapper
{
    public class MatchSearchResponse
    {
        public string GameId { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string Fixture { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;

    }
    public enum XPlatforms
    {
        XBet = 0,
        Sporty = 1,
        TwentyTwo = 2,
        Merry = 3,
        Kings = 4,
        Melbet = 5,
        MSport = 6,
    }
}
