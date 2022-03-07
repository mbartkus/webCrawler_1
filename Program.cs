using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper;
using System.Diagnostics;

namespace webCrawler_1
{
    class Program
    {
        static  void Main(string[] args)
        {
            //---variables--
            string CsvPath = "C:\\"; //csv file save location.
            var daysFromToday = 10;// number of days from the current date for the departure
            var daysFromToday_2 = 20; //number of days from the current date for the departure (second request)
            var returnAfter = 7; // amount of days from arrival to return

        
            Task task = AsyncStartCrawl(daysFromToday, daysFromToday_2, returnAfter, CsvPath);
            Console.ReadLine();
       
        }

        private static async Task AsyncStartCrawl(int daysFromToday, int daysFromToday_2, int returnAfter, string path)
        {
            //First request
            try
            {
                var url = ConstructURL(daysFromToday, returnAfter);

                //Setting up HttpClient and GET request
                var http_client = new HttpClient();
                var httpGet = await http_client.GetStringAsync(url);
                Console.WriteLine("test");
                var html_Doc = new HtmlDocument();
                html_Doc.LoadHtml(httpGet);
                //Gathering and Extracting data
                var Departures = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-flights fly5-depart th")).ToList();
                var Arrivals = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-flights fly5-return th")).ToList();

                var Departure_List = Departures[0].Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("fly5-result ")).ToList();
                var Arrival_List = Arrivals[0].Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("fly5-result fly")).ToList();
                if ((Departures != null && Arrivals != null) || (Departures.Count != 0 && Arrivals.Count != 0))
                {

                  var flightData = new FlightData();
                    foreach (var objDeparture in Departure_List)
                    {
                        flightData.outbound_arrival_airport = null;
                        flightData.outbound_arrival_time = null;
                        flightData.outbound_departure_airport = null;
                        flightData.outbound_departure_time = null;

                        flightData.outbound_departure_airport = objDeparture.SelectSingleNode("table/tbody/tr/td[1]/span[5]/text()[2]").InnerText.Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");

                        flightData.outbound_departure_time = objDeparture.SelectSingleNode("table/tbody/tr/td[1]/span[3]").InnerText.Replace("(", "").Replace(")", "")
                        + " " + objDeparture.SelectSingleNode("table/tbody/tr/td[1]/span[4]").InnerText.Replace("(", "").Replace(")", "");

                        flightData.outbound_arrival_airport = objDeparture.SelectSingleNode("//table/tbody/tr/td[3]/span[4]/text()[2]").InnerText.Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");

                        flightData.outbound_arrival_time = objDeparture.SelectSingleNode("table/tbody/tr/td[3]/span[2]").InnerText.Replace("(", "").Replace(")", "")
                        + " " + objDeparture.SelectSingleNode("table/tbody/tr/td[3]/span[3]").InnerText.Replace("(", "").Replace(")", "");

                        flightData.depart_price = objDeparture.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("flprice")).FirstOrDefault().InnerText;

                        foreach (var objArrival in Arrival_List)
                        {
                            flightData.inbound_arrival_airport = null;
                            flightData.inbound_departure_airport = null;
                            flightData.inbound_arrival_time = null;
                            flightData.inbound_departure_time = null;

                            flightData.inbound_departure_airport = objArrival.SelectSingleNode("//tr/td[3]/span[4]/text()[2]").InnerText.Replace("\n", "").Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");
                                                       
                            flightData.inbound_departure_time = objArrival.SelectSingleNode("table/tbody/tr/td[1]/span[3]").InnerText.Replace("(", "").Replace(")", "")
                            + " " + objArrival.SelectSingleNode("table/tbody/tr/td[1]/span[4]").InnerText.Replace("(", "").Replace(")", "");
                          
                            flightData.inbound_arrival_airport = objArrival.SelectSingleNode("table/tbody/tr/td[3]/span[4]/text()[2]").InnerText.Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");
                                                    
                            flightData.inbound_arrival_time = objArrival.SelectSingleNode("table/tbody/tr/td[3]/span[2]").InnerText.Replace("(", "").Replace(")", "")
                            + objArrival.SelectSingleNode("table/tbody/tr/td[3]/span[3]").InnerText.Replace("(", "").Replace(")", "");

                            flightData.arrive_price = objArrival.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("flprice")).FirstOrDefault().InnerText;
                            flightData.total_price = Calc_Total(flightData.depart_price, flightData.arrive_price);
                            AddToCSV(flightData, path);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No flights found\br");
                }
            }
            catch { Console.WriteLine("Error"); }

            //Second request
            try
            {
                var url = ConstructURL(daysFromToday_2, returnAfter);

                //Setting up HttpClient and GET request
                var http_client = new HttpClient();
                var httpGet = await http_client.GetStringAsync(url);
                //Gathering and Extracting data
                Console.WriteLine("test");
                var html_Doc = new HtmlDocument();
                html_Doc.LoadHtml(httpGet);


                var Departures = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-flights fly5-depart th")).ToList();
                var Arrivals = html_Doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("fly5-flights fly5-return th")).ToList();

                var Departure_List = Departures[0].Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("fly5-result ")).ToList();
                var Arrival_List = Arrivals[0].Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("fly5-result fly")).ToList();

                if ((Departures != null && Arrivals != null) || (Departures.Count != 0 && Arrivals.Count != 0))
                {

                  var flightData = new FlightData();
                    foreach (var objDeparture in Departure_List)
                    {

                        flightData.outbound_arrival_airport = null;
                        flightData.outbound_arrival_time = null;
                        flightData.outbound_departure_airport = null;
                        flightData.outbound_departure_time = null;

                        flightData.outbound_departure_airport = objDeparture.SelectSingleNode("table/tbody/tr/td[1]/span[5]/text()[2]").InnerText.Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");

                        flightData.outbound_departure_time = objDeparture.SelectSingleNode("table/tbody/tr/td[1]/span[3]").InnerText.Replace("(", "").Replace(")", "")
                        + " " + objDeparture.SelectSingleNode("table/tbody/tr/td[1]/span[4]").InnerText.Replace("(", "").Replace(")", "");

                        flightData.outbound_arrival_airport = objDeparture.SelectSingleNode("//table/tbody/tr/td[3]/span[4]/text()[2]").InnerText.Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");

                        flightData.outbound_arrival_time = objDeparture.SelectSingleNode("table/tbody/tr/td[3]/span[2]").InnerText.Replace("(", "").Replace(")", "")
                        + " " + objDeparture.SelectSingleNode("table/tbody/tr/td[3]/span[3]").InnerText.Replace("(", "").Replace(")", "");

                        flightData.depart_price = objDeparture.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("flprice")).FirstOrDefault().InnerText;

                        foreach (var objArrival in Arrival_List)
                        {
                            flightData.inbound_arrival_airport = null;
                            flightData.inbound_departure_airport = null;
                            flightData.inbound_arrival_time = null;
                            flightData.inbound_departure_time = null;


                            flightData.inbound_departure_airport = objArrival.SelectSingleNode("//tr/td[3]/span[4]/text()[2]").InnerText.Replace("\n", "").Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");
                           
                            
                            flightData.inbound_departure_time = objArrival.SelectSingleNode("table/tbody/tr/td[1]/span[3]").InnerText.Replace("(", "").Replace(")", "")
                            + " " + objArrival.SelectSingleNode("table/tbody/tr/td[1]/span[4]").InnerText.Replace("(", "").Replace(")", "");
                            //span[@title='input']

                            flightData.inbound_arrival_airport = objArrival.SelectSingleNode("table/tbody/tr/td[3]/span[4]/text()[2]").InnerText.Replace("\n", "").Replace("\r", "").Replace("(", "").Replace(")", "");

                            
                            flightData.inbound_arrival_time = objArrival.SelectSingleNode("table/tbody/tr/td[3]/span[2]").InnerText.Replace("(", "").Replace(")", "")
                            + objArrival.SelectSingleNode("table/tbody/tr/td[3]/span[3]").InnerText.Replace("(", "").Replace(")", "");

                            flightData.arrive_price = objArrival.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("flprice")).FirstOrDefault().InnerText;
                            flightData.total_price = Calc_Total(flightData.depart_price, flightData.arrive_price);
                            AddToCSV(flightData, path);
                        }
                    }

                }
                else
                {
                    Console.WriteLine("No flights found\br");
                }
            }
            catch { Console.WriteLine("Error"); }
            Environment.Exit(0);
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
        
        public static string Adjust_month_URL(int month)
        {
            if (month == 1)
            {
                return "Jan";
            }
            else if (month == 2)
            {
                return "Feb";
            }
            else if (month == 3)
            {
                return "Mar";
            }
            else if (month == 4)
            {
                return "Apr";
            }
            else if (month == 5)
            {
                return "May";
            }
            else if (month == 6)
            {
                return "Jun";
            }
            else if (month == 7)
            {
                return "Jul";
            }
            else if (month == 8)
            {
                return "Aug";
            }
            else if (month == 9)
            {
                return "Sep";
            }
            else if (month == 10)
            {
                return "Oct";
            }
            else if (month == 11)
            {
               return "Nov";
            }
            else
            {
                return "Dec";
            }

            }

        public static string ConstructURL(int days, int return_length)
        {
            var date = DateTime.Today.AddDays(days);
            var date_return = date.AddDays(return_length);

            //---needs editing for airport selection
             return "https://www.fly540.com/flights/nairobi-to-mombasa?isoneway=0&depairportcode=NBO&arrvairportcode=MBA" +
              "&date_from=" + Adjust_weekDay_URL(date.DayOfWeek) + "+" + date.Day + "+" + Adjust_month_URL(date.Month) + "+" + date.Year +
              "&date_to=" + Adjust_weekDay_URL(date_return.DayOfWeek) + "+" + date_return.Day + "+" + Adjust_month_URL(date_return.Month) + "+" + date_return.Year  +
              "&adult_no=1&children_no=0&infant_no=0&currency=USD&searchFlight=";
        }
        public static void AddToCSV(FlightData flData, string path)
        {
            string finalString = (flData.outbound_departure_airport
                        + ";" + flData.outbound_arrival_airport
                        + ";" + flData.outbound_departure_time
                        + ";" + flData.outbound_arrival_time
                        + ";" + flData.inbound_departure_airport
                        + ";" + flData.inbound_arrival_airport
                        + ";" + flData.inbound_departure_time
                        + ";" + flData.inbound_arrival_time
                        + ";" + flData.total_price + ";" + Calc_Taxes(flData.total_price));

            string filePath = path+"FlightList.csv";
            using (StreamWriter file = new StreamWriter(filePath, true))
            {     //true - to not overwrite
                var csvFileLenth = new System.IO.FileInfo(filePath).Length;
                if (csvFileLenth == 0) { 
                    file.WriteLine("outbound_departure_airport;outbound_arrival_airport;outbound_departure_time;outbound_arrival_time;inbound_departure_airport;inbound_arrival_airport;inbound_departure_time;inbound_arrival_time;total_price;taxes");
            }
                finalString= finalString.Replace(" ", "").Replace(",","").Trim();
                file.WriteLine(finalString);
                finalString = null;
            }
        }
            /**
            using (var streamWriter = new StreamWriter("FlightInformation.csv"))

            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords("test");
                csv.WriteRecords(flData.outbound_departure_airport + ";" + flData.outbound_arrival_airport + ";" + flData.outbound_departure_time + ";" + flData.outbound_arrival_time + ";" +
                 flData.inbound_departure_airport + ";" + flData.inbound_arrival_airport + ";" + flData.inbound_departure_time + ";" + flData.inbound_arrival_time + ";" +
                flData.depart_price + ";" + flData.arrive_price);

                Debug.WriteLine(flData.outbound_departure_airport+";"+ flData.outbound_arrival_airport + ";" + flData.outbound_departure_time + ";" + flData.outbound_arrival_time + ";" +
                 flData.inbound_departure_airport + ";" + flData.inbound_arrival_airport + ";" + flData.inbound_departure_time + ";" + flData.inbound_arrival_time + ";" +
                flData.total_price + ";" + Calc_Taxes(flData.total_price) ); 
            }
             **/
        
        public static string Calc_Taxes(string a)
        {
            //double _a = Convert.ToDouble(a);
            int constTax = 12; 
            return constTax.ToString();
        }
        public static string Calc_Total(string a, string b)
        {
            double _a = Convert.ToDouble(a);
            double _b = Convert.ToDouble(b);
            double sum = _a + _b;
            return sum.ToString();
    }
}}
   


