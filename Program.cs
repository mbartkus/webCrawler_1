using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;
//using ;

namespace webCrawler_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Varibales:
            AsyncStartCrawl();

            Console.WriteLine("test");
            Console.ReadLine();


        }






        private static async Task AsyncStartCrawl()
        {
            var xpath = "/html/body/div[1]/div/section/div[3]/form/div[1]/div[2]/div[1]/table/tbody/tr/td[4]/span[2]";
            string url = "https://www.fly540.com/flights/nairobi-to-mombasa?isoneway=1&depairportcode=NBO&arrvairportcode=MBA&date_from=Mon%2C+2+May+2022&date_to=Wed%2C+4+May+2022&adult_no=1&children_no=0&infant_no=0&currency=USD&searchFlight=";
            //string url = "https://www.fly540.com/flights";


            //seting up HttpClient
            var http_client = new HttpClient();
            //GET request
            var httpGet = await http_client.GetStringAsync(url);

            //settingup doc
            var html_Doc = new HtmlDocument();
            html_Doc.LoadHtml(httpGet);

            //AllTrips
            var listed_trips = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-results")).ToList();
            // <span class="flprice"
            var single_trip = listed_trips[0].Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("flprice")).ToList();
            foreach (var obj_Div in listed_trips)
            {
                var listed_Divs = html_Doc.DocumentNode.SelectNodes(xpath);
            }
        //   foreach (var obj_Div in listed_Divs)
          //  {
             //  obj_Div.Descendants("tr-link").

            }

        }

    }
