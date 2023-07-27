using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using HtmlAgilityPack;
using RecruiterSearch.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RecruiterSearch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PerformSearch(string keywords)
        {
            // Replace spaces in the keywords with "+"
            string encodedKeywords = keywords.Replace(" ", "+");

            // Construct the Google search URL
            string searchUrl = $"https://www.google.com/search?q={encodedKeywords}";

            try
            {
                // Fetch the HTML content using HttpClient
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetStringAsync(searchUrl).Result;

                    // Check if the HTML content is empty
                    if (response.Length == 0)
                    {
                        return null;
                    }

                    // Parse the search results using regular expressions
                    var searchResults = ParseSearchResults(response);

                    // Return the search results to the view
                    return View("Index", searchResults);
                }
            }
            catch (Exception ex)
            {
                // Log the error to the Output window for debugging
                System.Diagnostics.Debug.WriteLine("Error during search: " + ex.Message);

                // Return an error view or message to the user
                return View("Error");
            }
        }

        private List<SearchResult> ParseSearchResults(string htmlContent)
        {
            // Check if the HTML content is empty
            if (htmlContent.Length == 0)
            {
                return null;
            }
            // Define regular expressions to extract titles, URLs, and snippets
            var titleRegex = new Regex(@"<h3 class=""LC20lb DKV0Md"">(.*?)<\/h3>");
            var urlRegex = new Regex(@"<a href=""\/url\?q=(.*?)&");
            var snippetRegex = new Regex(@"<span class=""aCOpRe"">(.*?)<\/span>");

            // Find all matches for titles, URLs, and snippets
            var titleMatches = titleRegex.Matches(htmlContent);
            var urlMatches = urlRegex.Matches(htmlContent);
            var snippetMatches = snippetRegex.Matches(htmlContent);

            // Create a list to store search results
            var searchResults = new List<SearchResult>();

            // Combine matched results into SearchResult objects
            for (int i = 0; i < titleMatches.Count; i++)
            {
                var title = titleMatches[i].Groups[1].Value;
                var url = HttpUtility.UrlDecode(urlMatches[i].Groups[1].Value);
                var snippet = HttpUtility.HtmlDecode(snippetMatches[i].Groups[1].Value);

                searchResults.Add(new SearchResult { Title = title, Url = url, Snippet = snippet });
            }

            return searchResults;
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}