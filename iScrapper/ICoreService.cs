namespace iScrapper
{
    public interface ICoreService
    {
        Task<GenericResponse<IEnumerable<Stats24Fixture>>> GetStat24Predictions();
        Task<MatchSearchResponse> MatchSearch(MatchSearchRequest request);


        Task<GenericResponse<string>> RunBot(int count, string amount, bool includeUnder19And20, bool includeWomen);

    }
}
