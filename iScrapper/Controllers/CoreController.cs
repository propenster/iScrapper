using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace iScrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoreController : ControllerBase
    {
        private readonly ICoreService _coreService;

        public CoreController(ICoreService coreService)
        {
            _coreService = coreService;
        }

        [HttpGet("getstat24")]
        public async Task<IActionResult> GetStat24()
        {
            var result = await _coreService.GetStat24Predictions("https://www.stats24.com/football/home-team-to-win/29");

            return Ok(result);
        }
        [HttpPost("bookingcodeconverter")]
        public async Task<IActionResult> ConvertBookingCode([FromBody] ConvertBookingCodeRequest request)
        {
            var result = await _coreService.ConvertBookingCode(request);

            Console.WriteLine($"RESULT of the SPORTYBET STAKING BOT RUNNING >>> {result}");

            return Ok(result);
        }

        [HttpGet("drawpredictions")]
        public async Task<IActionResult> DrawPredictions()
        {
            var result = await _coreService.GetDrawPredictionsXbet();

            Console.WriteLine($"RESULT of the 1XBET DRAW PREDICTIONS RUNNING >>> {result}");

            return Ok(result);
        }

        [HttpPost("runbot")]
        public async Task<IActionResult> RunStakingBot([FromBody] RunStakingBotRequest request)
        {
            var result = await _coreService.RunBot(request.MarketType, request.Count, request.Amount, request.IncludeUnder19And20, request.includeWomen);

            Console.WriteLine($"RESULT of the SPORTYBET STAKING BOT RUNNING >>> {result}");

            return Ok(result);  
        }

        [HttpPost("runmsportbot")]
        public async Task<IActionResult> RunMsportBetBot([FromBody] RunStakingBotRequest request)
        {

            var result = await _coreService.RunMsportBetBot(request.MarketType, request.Count, request.Amount, request.IncludeUnder19And20, request.includeWomen);
            Console.WriteLine($"RESULT of the MSPORT STAKING BOT RUNNING >>> {result}");

            return Ok(result);
        }

        [HttpGet("getsearch")]
        public async Task<IActionResult> GetSearch()
        {
            var response = new MatchSearchResponse();
            WebDriver driver = new ChromeDriver();
            //driver.Url = "https://1xbet.ng/en/line";
            //driver.manage().timeouts().implicitlyWait(4, TimeUnit.SECONDS);
            driver.Navigate().GoToUrl("https://1xbet.ng/en/line");
            //driver.FindElement(By.XPath("//button[contains(text(), 'Add to Basket')]")).Click();
            IWebElement inputBox = default;
            IWebElement searchBtn = default;
            IWebElement output = default;
            try
            {
                inputBox = driver.FindElement(By.XPath("//input[@placeholder='Search by match']"));

            }
            catch (NoSuchElementException ex)
            {

                inputBox = driver.FindElement(By.XPath("//input[@placeholder='Search']"));
                Console.WriteLine(ex.Message);
            }

            if (inputBox != null)
            {
                inputBox.SendKeys("Djokovic");

            }

            //button click
            //btn btn_blue g-green btn_no-text
            try
            {
                //searchBtn = driver.FindElement(By.CssSelector("a.btn btn_blue g-green btn_no-text")); 
                searchBtn = driver.FindElement(By.XPath("//div[contains(@class, 'btn btn_blue g-green btn_no-text')]"));

            }
            catch (NoSuchElementException ex)
            {
                searchBtn = driver.FindElement(By.XPath("//button[contains(@class, 'search-button search-button--size-xxs has-tooltip')]"));
                Console.Write(ex);
                //throw;
            }
            if (searchBtn is null)
                return Ok();

            searchBtn.Click();

            await Task.Delay(100);
            //Harvest the results...
            try
            {
                var gameDate = driver.FindElement(By.XPath("//span[contains(@class, 'caption games-search-modal-card-date__caption caption--size-m')]")).Text;
                //var eventName = driver.FindElement(By.XPath("//span[contains(@class, 'caption__label')]/parent::span[contains(@class, 'caption games-search-modal-card-info__additional caption--size-m')]"));
                var eventName = driver.FindElement(By.XPath("//span[contains(@class, 'caption games-search-modal-card-info__additional caption--size-m')]")).Text;
                var fixture = driver.FindElement(By.XPath("//span[contains(@class, 'caption games-search-modal-card-info__main caption--size-m')]")).Text;

                response = new MatchSearchResponse
                {
                    EventName = eventName,
                    Fixture = fixture,
                    GameId = Guid.NewGuid().ToString(),
                    Date = gameDate,
                };

            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return BadRequest(ex.Message);
            }
            //output = driver.FindElement(By.XPath("//div[@class='events__team']"));
            //Console.WriteLine($"Results \n{output?.Text}");


            return Ok(response);
        }




    }
}
