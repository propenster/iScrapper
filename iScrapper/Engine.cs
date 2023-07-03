namespace iScrapper
{
    public class Engine : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public Engine(IConfiguration config, IServiceProvider provider, ILogger<Engine> logger)
        {
            _config = config;
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background Engine Service is running ");

                //to start strategy... Home or Away Wins .... Over0/5HalfTime
                using(var scope = _provider.CreateScope())
                {
                    var _coreService = scope.ServiceProvider.GetRequiredService<ICoreService>();

                    //MSport Home Wins...
                    var result1 = await _coreService.RunMsportBetBot(MarketType.HomeWins, 1, "100", false, false);
                    _logger.LogInformation($"RESULT of RUNNING MSPORT @ {DateTime.Now} Booking Code >>> {result1?.Data}");
                    //

                    var result2 = await _coreService.RunMsportBetBot(MarketType.HalfTimeOverPoint5, 1, "100", false, false);
                    _logger.LogInformation($"RESULT of RUNNING MSPORT @ {DateTime.Now} Booking Code >>> {result2?.Data}");



                }

                await Task.Delay(3600000); //run every hour in the day... 


            }
        }
        
    }
}
