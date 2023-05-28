using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;
using OpenQA.Selenium.DevTools.V111.Storage;
using static System.Net.Mime.MediaTypeNames;
using OpenQA.Selenium.Interactions;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace iScrapper
{
    public class CoreService : ICoreService
    {
        private readonly IConfiguration _configuration;

        public CoreService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetSearchUrlByPlatform(XPlatforms platform)
        {
            switch (platform)
            {
                case XPlatforms.XBet:
                    return "https://1xbet.ng/en/line";
                case XPlatforms.Sporty:
                    return "https://www.sportybet.com/ng/";


                default:
                    return string.Empty;
            }
        }
        public async Task<MatchSearchResponse> MatchSearch(MatchSearchRequest request)
        {
            var response = new MatchSearchResponse();

            try
            {
                switch (request.XPlatform)
                {
                    case XPlatforms.XBet:
                        response = await XBetSearch(request);
                        break;
                    case XPlatforms.Sporty:
                        response = await SportySearch(request);
                        break;

                    default:
                        response = new MatchSearchResponse();
                        break;
                }




                return response;


            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return response;
            }
        }

        private async Task<MatchSearchResponse> SportySearch(MatchSearchRequest request)
        {
            var response = new MatchSearchResponse();
            IWebElement inputBox = default;
            IWebElement searchBtn = default;
            IWebElement output = default;
            WebDriver driver = new ChromeDriver();

            try
            {
                //driver.Url = "https://1xbet.ng/en/line";
                //driver.manage().timeouts().implicitlyWait(4, TimeUnit.SECONDS);
                var url = GetSearchUrlByPlatform(request.XPlatform);
                if (string.IsNullOrWhiteSpace(url)) return response;

                driver.Navigate().GoToUrl(url);
                //LOGIN to sporty first....
                //var dir = driver.FindElement(By.XPath(String.Format("//*[contains(text(), '{0}')]/ancestor::a[1]", path.Directory)));
                //driver.FindElement(By.XPath("//div[contains(text(), 'Target Text')]/.."));
                driver.FindElement(By.XPath("//div[contains(text(), 'Login')]")).Click();

                //FILL LOGIN FORM...
                var mobileNumberInput = driver.FindElement(By.XPath("//input[@placeholder='Mobile Number']")) ?? null;
                if (mobileNumberInput != null) mobileNumberInput.SendKeys(_configuration["sportyusername"]);
                var passwordInput = driver.FindElement(By.XPath("//input[@placeholder='Set Password']")) ?? null;
                //click login button 
                //driver.FindElement(By.XPath("//*[@gl-command='transaction']")).Click();
                driver.FindElement(By.XPath("//span[@data-cms-key='log_in']")).Click();
                await Task.Delay(1500);

                //https://www.sportybet.com/ng/m/search?key=djokovic







                inputBox = driver.FindElement(By.XPath("//input[@placeholder='Search by match']"));

            }
            catch (NoSuchElementException ex)
            {

                inputBox = driver.FindElement(By.XPath("//input[@placeholder='Search']"));
                Console.WriteLine(ex.Message);
            }

            if (inputBox != null)
            {
                inputBox.SendKeys(request.SearchTerm);

            }

            //button click
            try
            {
                searchBtn = driver.FindElement(By.XPath("//div[contains(@class, 'btn btn_blue g-green btn_no-text')]"));

            }
            catch (NoSuchElementException ex)
            {
                searchBtn = driver.FindElement(By.XPath("//button[contains(@class, 'search-button search-button--size-xxs has-tooltip')]"));
                Console.Write(ex);
                //throw;
            }
            if (searchBtn is null)
                return response;

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
                return response; //return GenericResponse
            }


            return response;
        }

        private async Task<MatchSearchResponse> XBetSearch(MatchSearchRequest request)
        {
            var response = new MatchSearchResponse();
            IWebElement inputBox = default;
            IWebElement searchBtn = default;
            IWebElement output = default;
            WebDriver driver = new ChromeDriver();

            try
            {
                //driver.Url = "https://1xbet.ng/en/line";
                //driver.manage().timeouts().implicitlyWait(4, TimeUnit.SECONDS);
                var url = GetSearchUrlByPlatform(request.XPlatform);
                if (string.IsNullOrWhiteSpace(url)) return response;

                driver.Navigate().GoToUrl(url);

                inputBox = driver.FindElement(By.XPath("//input[@placeholder='Search by match']"));

            }
            catch (NoSuchElementException ex)
            {

                inputBox = driver.FindElement(By.XPath("//input[@placeholder='Search']"));
                Console.WriteLine(ex.Message);
            }

            if (inputBox != null)
            {
                inputBox.SendKeys(request.SearchTerm);

            }

            //button click
            try
            {
                searchBtn = driver.FindElement(By.XPath("//div[contains(@class, 'btn btn_blue g-green btn_no-text')]"));

            }
            catch (NoSuchElementException ex)
            {
                searchBtn = driver.FindElement(By.XPath("//button[contains(@class, 'search-button search-button--size-xxs has-tooltip')]"));
                Console.Write(ex);
                //throw;
            }
            if (searchBtn is null)
                return response;

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
                return response; //return GenericResponse
            }


            return response;
        }

        public async Task<GenericResponse<IEnumerable<Stats24Fixture>>> GetStat24Predictions()
        {
            var response = new List<Stats24Fixture>();
            IWebElement inputBox = default;
            IWebElement searchBtn = default;
            IWebElement output = default;
            WebDriver driver = new EdgeDriver();
            try
            {
                var url = "https://www.stats24.com/football/home-team-to-win/29";

                driver.Navigate().GoToUrl(url);
                //List<string> elementTexts = driver.FindElements(By.ClassName("comments")).Select(iw => iw.Text);

                //var date = driver.FindElement(By.XPath("//ul[contains(@class='cycle-slideshow')]")).Text ?? DateTime.Now.ToString();
                var date = driver.FindElement(By.XPath("//*[@class='cycle-slideshow']/li[1]")).Text ?? DateTime.Now.ToString();
                var tableRows = driver.FindElements(By.XPath("//table[class='general_table_style responsive_tbl1 matchProbList']/tbody/tr"));
                //var tableRows1 = driver.FindElements(By.XPath("/html/body/div[2]/div/div[1]/div[2]/div[6]/div[3]/div/table/tbody/tr[2]"));
                var tableRows1 = driver.FindElements(By.CssSelector("body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0"));
                //general_table_style responsive_tbl1 matchProbList
                //var elements = driver.FindElements(By.XPath("//tr[contains(@class='table_td pattern1 Page-Index-0')]"));


                if (tableRows1.Any())
                {

                    ParallelLoopResult result = Parallel.ForEach(tableRows1, new ParallelOptions { MaxDegreeOfParallelism = 7 }, item => response.Add(FormatStat24(item, date)));

                    //foreach (var item in tableRows1.Take(2))
                    //{
                    //    var time = item.FindElement(By.XPath($"//span[@data-date='{date}']")).Text ?? string.Empty;
                    //    //var home = item.FindElement(By.XPath($"//div[contains(@class='team_name team_name1')]")).Text ?? string.Empty;
                    //    var home = item.FindElement(By.CssSelector($"body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_team_wrap_td.left_align > div > div.col_team_left > div > a > div.team_name.team_name1")).Text ?? string.Empty;
                    //    var away = item.FindElement(By.CssSelector($"body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_team_wrap_td.left_align > div > div.col_team_left > div > a > div.team_name.team_name2")).Text ?? string.Empty;
                    //    //var away = item.FindElement(By.XPath($"//div[contains(@class='team_name team_name2')]")).Text ?? string.Empty;
                    //    var probability = item.FindElement(By.CssSelector("body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_probability_wrap_td > div > span")).Text ?? string.Empty;
                    //    var market = item.FindElement(By.CssSelector("body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_market_wrap_td.left_align > div > span:nth-child(1) > a")).Text ?? string.Empty;
                    //    var res = new Stats24Fixture
                    //    {
                    //        Time = time, ///html/body/div[2]/div/div[1]/div[2]/div[6]/div[3]/div/table/tbody/tr[2]/td[1]/div/span
                    //        Date = date,
                    //        Home = home,
                    //        Away = away,
                    //        Probability = probability,
                    //        Market = market
                    //    };
                    //    response.Add(res);
                    //    //var allTds = item.FindElements(By.XPath(".//child::td"));

                    //}

                }

                driver.Close();





                //response = driver.FindElements(By.XPath("//tr[contains(@class='table_td pattern1 Page-Index-0')]")).Select(x => new Stats24Fixture
                //{


                //}).ToList();

                return new GenericResponse<IEnumerable<Stats24Fixture>>
                {
                    Status = true,
                    Message = "Fetched stat24 successfully",
                    Data = response,
                };
            }
            catch (Exception ex)
            {

                Console.Write($"Error: {ex}");
                return new GenericResponse<IEnumerable<Stats24Fixture>>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = response
                };
            }
        }

        private Stats24Fixture FormatStat24(IWebElement item, string date)
        {
            var time = item.FindElement(By.XPath($"//span[@data-date='{date}']")).Text ?? string.Empty;
            //var home = item.FindElement(By.XPath($"//div[contains(@class='team_name team_name1')]")).Text ?? string.Empty;
            var home = item.FindElement(By.CssSelector($"body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_team_wrap_td.left_align > div > div.col_team_left > div > a > div.team_name.team_name1")).Text ?? string.Empty;
            var away = item.FindElement(By.CssSelector($"body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_team_wrap_td.left_align > div > div.col_team_left > div > a > div.team_name.team_name2")).Text ?? string.Empty;
            //var away = item.FindElement(By.XPath($"//div[contains(@class='team_name team_name2')]")).Text ?? string.Empty;
            var probability = item.FindElement(By.CssSelector("body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_probability_wrap_td > div > span")).Text ?? string.Empty;
            var market = item.FindElement(By.CssSelector("body > div.main_page_wrap > div > div.page_wrap_inner > div.page_right_side > div.right_inner_content > div.table_repeat_block.match-list > div > table > tbody > tr.table_td.pattern1.Page-Index-0 > td.col_market_wrap_td.left_align > div > span:nth-child(1) > a")).Text ?? string.Empty;
            var res = new Stats24Fixture
            {
                Time = time, ///html/body/div[2]/div/div[1]/div[2]/div[6]/div[3]/div/table/tbody/tr[2]/td[1]/div/span
                Date = date,
                Home = home,
                Away = away,
                Probability = probability,
                Market = market
            };
            return res;
        }

        public async Task<GenericResponse<string>> RunBot(int count, string amount, bool includeUnder19And20, bool includeWomen)
        {
            var filtered = new List<Stats24Fixture>();
            var take = 0;
            try
            {
                var getTodaysPredictions = await GetStat24Predictions();
                if (!getTodaysPredictions.Status)
                {
                    await Task.Delay(2000);
                    getTodaysPredictions = await GetStat24Predictions();
                }

                if (!getTodaysPredictions.Status) return null;

                //filtered = getTodaysPredictions.Data.Any() ? getTodaysPredictions.Data.Where(x => !x.Home.Contains("(w)") || !x.Away.Contains("(w)")).ToList() : new List<Stats24Fixture>();
                if (getTodaysPredictions.Data.Any() && !includeUnder19And20) filtered = getTodaysPredictions.Data.Where(x => !x.Home.Contains("U19") && !x.Away.Contains("U19") && !x.Home.Contains("U20") && !x.Away.Contains("U20")).ToList();
                if (getTodaysPredictions.Data.Any() && !includeWomen) filtered = getTodaysPredictions.Data.Where(x => !x.Home.Contains("(w)") || !x.Away.Contains("(w)")).ToList();

                if (count > getTodaysPredictions.Data.Count()) take = getTodaysPredictions.Data.Count();
                else take = count;

                //final compiled list... on randomixing...
                //filtered = filtered.Where(x => DateTime.Parse(x.Time).AddHours(-2) >= DateTime.Now).ToList();

                filtered = filtered.OrderBy(x => Guid.NewGuid()).Take(take).ToList();

                //Stake 
                var stakeSporty = await StakeSporty(amount, filtered);
                Console.WriteLine($"Successfully ran Sporty bet stake >>> {stakeSporty}");

                return new GenericResponse<string>
                {
                    Data = stakeSporty,
                    Status = true,
                    Message = "Successfully staked sporty bet",
                };

                //list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
            }
            catch (Exception ex)
            {

                return new GenericResponse<string>
                {
                    Data = string.Empty,
                    Message = ex.Message,
                    Status = false,
                };
            }
        }
        /// <summary>
        /// This method automates searching an event and staking it on SportyBet based on predictions fetched from stats24.com for HomeToWin
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        private async Task<string> StakeSporty(string amount, List<Stats24Fixture> stats)
        {
            var betCode = string.Empty;
            IWebElement inputBox = default;
            IWebElement searchBtn = default;
            IWebElement output = default;
            //WebDriver driver = new EdgeDriver();
            WebDriver driver = new ChromeDriver();

            try
            {
                var url = "https://www.sportybet.com/ng/";

                driver.Navigate().GoToUrl(url);
                Actions actions = new Actions(driver);

                //Mobile Number
                //LOGIN..
                driver.FindElement(By.XPath("//input[@placeholder='Mobile Number']")).SendKeys(_configuration["sportyusername"]); //e.g 80929373461
                driver.FindElement(By.XPath("//input[@placeholder='Password']")).SendKeys(_configuration["sportypassword"]); //sportLoginPassword

                driver.FindElement(By.XPath("//button[@name='logIn']")).Click();

                //ceteris paribus, we should have logged in to sporty...
                await Task.Delay(1000);

                //driver.Navigate().GoToUrl($"https://www.sportybet.com/ng/m/search?key={}");
                driver.Navigate().GoToUrl($"https://www.sportybet.com/ng/m/search");
                //grab search input...
                var betBucket = new List<int>();
                foreach (var item in stats)
                {

                    var i = 0;
                    //driver.Navigate().GoToUrl($"https://www.sportybet.com/ng/m/search");

                    var searchBox = driver.FindElement(By.XPath("//input[@placeholder='Teams/Players, Leagues, Game ID']"));




                    searchBox.Clear();


                    searchBox.SendKeys(item.Home);
                    await Task.Delay(300);

                    //if (betBucket.Any())
                    //{
                    //    JavaScriptClickElement(driver, driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")), true);

                    //    break;
                    //}

                    try
                    {
                        var noResult = driver.FindElement(By.XPath("//span[@data-cms-key='search_no_result']")); //.
                        if (noResult != null && noResult.Text.Contains("No results at this time"))
                        {
                            driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")).Click();
                            //JavaScriptClickElement(driver, driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")), true);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is StaleElementReferenceException)
                        {
                            driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")).Click();
                            //JavaScriptClickElement(driver, driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")), true);
                            continue;
                        }
                        Console.WriteLine("Result was returned");
                        driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")).Click();
                        //JavaScriptClickElement(driver, driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")), true);

                        continue;

                    }
                    //var rowDiv = driver.FindElements(By.CssSelector("#notInWebView > div.page-content.page-content--ng > div > div.m-main-mid.m-has-bottom-nav.has-bg-color > div.m-search.m-result-grey > div:nth-child(2) > div:nth-child(1) > div.m-live-upcoming > div:nth-child(2) > div.m-table.m-sports-table.football > div:nth-child(2) > div.m-table-row.m-sports-table")).FirstOrDefault();
                    //var rowDivs = driver.FindElements(By.CssSelector("#notInWebView > div.page-content.page-content--ng > div > div.m-main-mid.m-has-bottom-nav.has-bg-color > div.m-search.m-result-grey > div:nth-child(2) > div:nth-child(1) > div.m-live-upcoming > div:nth-child(2) > div.m-table.m-sports-table.football > div:nth-child(2) > div.m-table-row.m-sports-table"));

                    //var rowDivs = driver.FindElements(By.XPath("//span[@data-op='label-odds-amount']"));
                    IWebElement homeWin = default;
                    IWebElement draw = default;
                    IWebElement awayWin = default;
                    try
                    {
                        //var rowDivs1 = driver.FindElements(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/div[2]/div[2]/div/div[1]"));
                        //var rowDivs = driver.FindElements(By.CssSelector("m-market m-event-market-default.market-id-1"));

                        homeWin = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/div[2]/div[2]/div/div[1]/span"));
                        draw = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/div[2]/div[2]/div/div[2]/span"));
                        awayWin = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/div[2]/div[2]/div/div[3]/span"));

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Result was returned");
                        //driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")).Click();
                        //actions.MoveToElement(driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i"))).Click().Perform();
                        JavaScriptClickElement(driver, driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")), true);

                        continue;
                    }

                    if (Math.Round(decimal.Parse(homeWin.Text), MidpointRounding.AwayFromZero) > Math.Round(decimal.Parse(awayWin.Text), MidpointRounding.AwayFromZero))
                    {

                        actions.MoveToElement(awayWin).Click().Perform();
                        //JavaScriptClickElement(driver, awayWin, true);

                        //awayWin.Click();
                        break;
                    }
                    else
                    {
                        //homeWin.Click();
                        //JavaScriptClickElement(driver, homeWin, true);

                        actions.MoveToElement(homeWin).Click().Perform();
                        break;

                    }


                    //var rowDiv = rowDivs.FirstOrDefault();

                    //var homeWinMarket = rowDiv.FindElements(By.CssSelector("#notInWebView > div.page-content.page-content--ng > div > div.m-main-mid.m-has-bottom-nav.has-bg-color > div.m-search.m-result-grey > div:nth-child(2) > div:nth-child(1) > div.m-live-upcoming > div:nth-child(2) > div.m-table.m-sports-table.football > div:nth-child(2) > div.m-table-row.m-sports-table > div.m-table-cell.m-market-cell > div > div:nth-child(1)")).FirstOrDefault();

                    //homeWinMarket.Click();

                    //actions.MoveToElement(driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i"))).Click().Perform();
                    JavaScriptClickElement(driver, driver.FindElement(By.XPath("//*[@id=\"search\"]/div/i")), true);
                    betBucket.Add(i);
                    i++;

                }

                if (betBucket != null)
                {
                    IWebElement placeBetBtn = default;
                    IWebElement confirmToPayBtn = default;
                    IWebElement betslipDialog = default;
                    //PlaceBet.
                    await Task.Delay(200);
                    //bring the betslip dialog to focus...
                    try
                    {
                        betslipDialog = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div"));
                        JavaScriptClickElement(driver, betslipDialog, false);

                        //actions.MoveToElement(betslipDialog).Perform();
                    }
                    catch (Exception ex)
                    {

                        Console.Write(ex);
                    }

                    try
                    {
                        placeBetBtn = driver.FindElement(By.XPath("//*[@id=\"betslip-container\"]/div[1]/div[3]/div[1]/div[2]/div/div[7]/div[2]/span"));
                        //JavaScriptClickElement(driver, placeBetBtn, true);

                        actions.MoveToElement(placeBetBtn).Click().Perform();

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                        placeBetBtn = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div/div/div[4]/div[1]/div[2]"));
                        //JavaScriptClickElement(driver, placeBetBtn, true);

                        actions.MoveToElement(placeBetBtn).Click().Perform();
                    }
                    //COnfirm to pay...
                    await Task.Delay(200);

                    try
                    {
                        confirmToPayBtn = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div/div/div[4]/div[2]/div[2]/div[2]/button[2]"));
                        actions.MoveToElement(confirmToPayBtn).Click().Perform();
                        //JavaScriptClickElement(driver, confirmToPayBtn, true);

                    }
                    catch (Exception ex)
                    {

                        Console.Write(ex);
                        confirmToPayBtn = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div/div/div[4]/div[2]/div[2]/div[2]/button[2]/span"));
                        //JavaScriptClickElement(driver, confirmToPayBtn, true);

                        actions.MoveToElement(confirmToPayBtn).Click().Perform();
                    }

                    await Task.Delay(1500);

                    IWebElement bookingCodeSpan = default;

                    try
                    {
                        bookingCodeSpan = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div/div/div[5]/div[1]/ul/li[3]/span[2]"));
                        JavaScriptClickElement(driver, bookingCodeSpan, false);

                        betCode = bookingCodeSpan.Text;
                    }
                    catch (Exception ex)
                    {
                        betCode = string.Empty;
                        Console.WriteLine(ex);
                    }

                    //betCode = driver.FindElement(By.CssSelector("#betslip-container > div.success-wrap > div.success-dialog > ul > li.share-code > span:nth-child(2) > span")).Text ?? string.Empty;
                    //click final OK button 
                    IWebElement finalOkBtn = default;
                    try
                    {
                        finalOkBtn = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div/div/div[5]/div[2]/button[2]"));
                        JavaScriptClickElement(driver, finalOkBtn, true);

                        //actions.MoveToElement(finalOkBtn).Click().Perform();

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                        finalOkBtn = driver.FindElement(By.XPath("//*[@id=\"notInWebView\"]/div[3]/div/div[1]/div[2]/div/div/div[5]/div[2]/button[2]/span"));
                        //JavaScriptClickElement(driver, finalOkBtn, true);

                        actions.MoveToElement(finalOkBtn).Click().Perform();
                    }
                }

                driver.Navigate().GoToUrl(url);
                driver.Close();



                Console.WriteLine("SUCCESSFULLY PLACED BET ON SPORTY BET");
                return betCode;

            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return betCode;
            }
        }
        private void JavaScriptClickElement(WebDriver driver, IWebElement element, bool isClick)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
            if (isClick == true) { element.Click(); }
        }
    }
}
