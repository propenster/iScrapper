namespace iScrapper
{
    public interface ICoreService
    {
        Task<GenericResponse<IEnumerable<Stats24Fixture>>> GetStat24Predictions(string url);
        Task<MatchSearchResponse> MatchSearch(MatchSearchRequest request);


        Task<GenericResponse<string>> RunBot(MarketType marketType, int count, string amount, bool includeUnder19And20, bool includeWomen);
        Task<GenericResponse<string>> Run22BetBot();
        Task<GenericResponse<string>> RunMsportBetBot(MarketType marketType, int count, string amount, bool includeUnder19And20, bool includeWomen);


        Task<GenericResponse<string>> ConvertBookingCode(ConvertBookingCodeRequest request);

        Task<List<DrawPredictorModel>> GetDrawPredictionsXbet();

    }
}
