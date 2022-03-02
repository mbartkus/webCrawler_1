using System;
using System.Net.Http;
using HtmlAgilityPack;
using 
namespace webCrawler_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Varibales:
            string url;

       
;


            }

        private static async Task StartCrawler(string url, HtmlDocument htmlDoc)
        {


            //seting up HttpClient
            var http_client = new HttpClient();
            //GET request
            http_client.GetStringAsync(url);


            //settingup doc
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(url);
            htmlDoc.DocumentNode.Descendants("<div>").Where()node => node.get)
        }


     }

