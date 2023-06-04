namespace iScrapper
{
    public class DrawPredictorModel
    {
        public string Home { get; set; } = string.Empty;
        public string Away { get; set; } = string.Empty;

        public string HomeWin { get; set; } = string.Empty;
        public string Draw { get; set;} = string.Empty;
        public string AwayWin { get; set;} = string.Empty;
        public string HomeWinOrDraw { get; set; } = string.Empty;  
        public string HomeOrAway { get; set; } = string.Empty;
        public string AwayWinOrDraw { get;set; } = string.Empty;

        public bool IsDrawPossible { get; set; }
        public string DrawPercentageProbability { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
