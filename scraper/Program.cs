using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Net.Http;

//using scraper (extract base class) 
//main fcn in driver (this current file)
namespace scraper
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var surfUrl = "https://www.surf-forecast.com/breaks/Cupsogue/forecasts/latest"; 
            string webhookUrl = "";
            var cupsogueScraper = new ScraperClassBase(surfUrl,webhookUrl);

            string[] cWaveInfo = cupsogueScraper.getSurfData();
            string[][] windArray = cupsogueScraper.windAndSwellAngle();

            //Times to check
            var now = DateTime.Now;
            DateTime fiveAM = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0);
            DateTime twelvePM = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
            DateTime fivePM = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);

            Console.WriteLine("Gathering surf info...");

            //morning report
            if (DateTime.Compare(now,fiveAM) == 1 && DateTime.Compare(now, twelvePM) == -1) 
            {
                await cupsogueScraper.MessageAsync(cWaveInfo[0], windArray[0]);
            }
            //afternoon report
            else if (DateTime.Compare(now, twelvePM) == 1 && DateTime.Compare(now, fivePM) == -1)
            {
                await cupsogueScraper.MessageAsync(cWaveInfo[1], windArray[1]);
            }
            //evening report
            else if (DateTime.Compare(now, fivePM) == 1)
            {
                await cupsogueScraper.MessageAsync(cWaveInfo[2], windArray[2]);
            }
            
            else
            {
                Console.WriteLine("something failed!");
            }
        }
    }
}