namespace iScrapper
{
    public class RunStakingBotRequest
    {
        public int Count { get; set; }
        public string Amount { get; set; } = string.Empty;
        public bool IncludeUnder19And20 { get; set; }
        public bool includeWomen { get; set; }
        public MarketType MarketType { get; set; }
    }
}
