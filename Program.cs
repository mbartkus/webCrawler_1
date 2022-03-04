using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper;


//using ;

namespace webCrawler_1
{
    class Program
    {

        
        static void Main(string[] args)
        {

        
                


           
            AsyncStartCrawl(10, 7);

            Console.WriteLine("test");
            Console.ReadLine();


        }

        private static async Task AsyncStartCrawl(int daysFromToday, int returnAfter)
        {
            /**
      * 
      * from NBO (Nairobi) to MBA (Mombasa)
 
      *
      * 
      * **/
            var url = ConstructURL(daysFromToday, returnAfter);

            var ListOfFlightData = new List<FlightData>();
            //Setting up HttpClient and GET request
            var http_client = new HttpClient();
            var httpGet = await http_client.GetStringAsync(url);
        //Gathering and Extracting data
            var html_Doc = new HtmlDocument();
            html_Doc.LoadHtml(httpGet);
            string xPath = "/html/body/div[1]/div/section/div[3]/form/div[1]/div[2]/div[1]";
            ///html/body/div[1]/div/section/div[3]/form/div[1]/div[2]
          
                      
            var List_Departures = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-flights fly5-depart th")).ToList();
            var trip_table = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-results")).ToList();
            //var all_trips = trip_table[0].Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("flprice")).ToList();
            foreach (var obj_Div in all_trips)
            {
            //    var ListOfFlightData = new List<FlightData>();

            }

            
          using (var streamWriter = new StreamWriter("FlightInformation.csv"))

           using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture)) {
                csv.WriteRecords(ListOfFlightData);
            
            }
            
           
        }


        public static string Adjust_weekDay_URL(DayOfWeek day)
        {
            if (day == DayOfWeek.Monday)
            {
                return "Mon%2C";
            }
            else if (day == DayOfWeek.Tuesday)
            {
                return "Tue%2C";
            }
            else if (day == DayOfWeek.Wednesday)
            {
                return "Wed%2C";
            }
            else if (day == DayOfWeek.Thursday)
            {
                return "Thu%2C";
            }
            else if (day == DayOfWeek.Friday)
            {
                return "Fri%2C";
            }
            else if (day == DayOfWeek.Saturday)
            {
                return "Sat%2C";
            }
            else
            {
                return "Sun%2C";
            }



        }

        public static string ConstructURL(int days, int return_length)
        {

            var date = DateTime.Today.AddDays(days);
            var date_return = date.AddDays(return_length);

            //reference "https://www.fly540.com/flights/nairobi-to-mombasa?isoneway=0&depairportcode=NBO&arrvairportcode=MBA&date_from=Tue%2C+15+Mar+2022&date_to=Thu%2C+17+Mar+2022&adult_no=1&children_no=0&infant_no=0&currency=USD&searchFlight="
            return "https://www.fly540.com/flights/nairobi-to-mombasa?isoneway=1&depairportcode=NBO&arrvairportcode=MBA" +
              "&date_from=" + Adjust_weekDay_URL(date.DayOfWeek) + "+" + date.Day + "+" + date.Month + "+" + date.Year + " \"" +
              "&date_to=" + Adjust_weekDay_URL(date_return.DayOfWeek) + "+" + date_return.Day + "+" + date_return.Month + "+" + date_return.Year + " \"" +
              "&adult_no=1&children_no=0&infant_no=0&currency=USD&searchFlight=";

        }
    }
}
   


