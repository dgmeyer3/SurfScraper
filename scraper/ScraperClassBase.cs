using HtmlAgilityPack;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Text;

namespace scraper
{
    class ScraperClassBase
    {

        string ScrapeURI;
        string Webhook;
        public ScraperClassBase(string scrapeURI, string webhook)
        {
            ScrapeURI = scrapeURI;
            Webhook = webhook;
        }
        public string[] getSurfData()
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(ScrapeURI);

                var waveTable = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]" +
                    "/div/div[2]/div[2]/div[2]/div[3]/div[5]/div[1]/div[2]/section/div/table");
                var childNodes = waveTable.ChildNodes;

                List<string> waveInfo = new List<string>();

                foreach (var child in childNodes)
                {
                    waveInfo.Add(child.InnerText);
                }
                //removes parentheses content. parentheses only contain dates
                string waveRegex = Regex.Replace(waveInfo[1], @"\((.*?)\)", "");
                //splits string when a lowercase appears in front of uppercase
                string[] splitWaveRegex = Regex.Split(waveRegex, @"(?<!^)(?=[A-Z])");

                return splitWaveRegex;
            }

            catch (IndexOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("index parameter is out of range.", e);
            }
        }

        public string[][] windAndSwellAngle()
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(ScrapeURI);

            //wind speed is in Kilometers
            var morningWind = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[2]/" +
                "div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[9]/td[1]/div/svg/text").InnerText;
            var afternoonWind = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[2]" +
                "/div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[9]/td[4]/div/svg/text").InnerText;
            var eveningWind = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[2]/" +
                "div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[9]/td[5]/div/svg/text").InnerText;

            var morningDirection = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/" +
                "div[2]/div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[10]/td[1]").InnerText;
            var afternoonDirection = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/" +
                "div/div[2]/div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[10]/td[4]").InnerText;
            var eveningDirection = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/" +
                "div[2]/div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[10]/td[6]").InnerText;

            var morningSwellDir = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[2]/" +
                "div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[5]/td[1]/div/div").InnerText;
            var afternoonSwellDir = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[2]/" +
                "div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[5]/td[4]/div/div").InnerText;
            var eveningSwellDir = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[2]/" +
                "div[2]/div[2]/div[3]/div[5]/div[2]/div/div[3]/div/table/tbody/tr[5]/td[5]/div/div").InnerText;

            string[] morningArray = {morningWind, morningDirection, morningSwellDir};
            string[] afternoonArray = { afternoonWind, afternoonDirection, afternoonSwellDir};
            string[] eveningArray = { eveningWind, eveningDirection, eveningSwellDir};

            string[][] windDirections = {morningArray, afternoonArray, eveningArray};
            return windDirections;

        }

        public string WaveQuality(string waveString, string[] windSwellArray)
        {
            return "define the wave quality with a single function";
        }

        public async Task MessageAsync(string waveMessage, string[] windSwellArray)
        {
            //try catch block was going to go here, final section of array may be null. 
            string[] surfArray = waveMessage.Split(" ");

            //update wavemessage to make a better string
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Webhook);

            Message body = new Message();
            body.text = $"The waves at Cupsogue this {surfArray[0]} are {surfArray[1]} high with a period of {surfArray[2]}," +
                $" with a {windSwellArray[2]} swell direction. There is a  {windSwellArray[0]} km/h {windSwellArray[1]} wind.";

            string serializeJson = JsonConvert.SerializeObject(body);
            StringContent content = new StringContent(serializeJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(client.BaseAddress, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseContent);
        }
    }
}