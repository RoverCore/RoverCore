using Hyperion.Web.Models.ApiModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hyperion.Web.Util
{
    public class RSS
    {
        public static async Task<List<NewsFeedItem>> GetNewsFeed(string sourceUrl, string endpoint)
        {
            var client = new RestClient(sourceUrl);
            var request = new RestRequest(endpoint);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var posts = ParseFeed(response.Content);

                if(posts.Count == 0)
                {
                    return null;
                }

                return posts;
            }

            return null;
        }

        private static List<NewsFeedItem> ParseFeed(string rss)
        {
            var list = new List<NewsFeedItem>();

            try
            {
                var xdoc = XDocument.Parse(rss);
                var items = xdoc.Descendants("item");
                var id = 0;

                foreach (var item in items)
                {
                    NewsFeedItem article = new NewsFeedItem();

                    // Fri, 04 May 2018 15:31:44 EDT <-- district version
                    // Wed, 02 May 2018 12:58:20 -0000 <-- RSS aggregator version
                    string dateString = item.Element("pubDate").Value;
                    DateTime pubDate = DateTime.MinValue;

                    RSSDateTimeUtility.TryParseRfc822DateTime(dateString, out pubDate);

                    if (pubDate == DateTime.MinValue)
                    {
                        RSSDateTimeUtility.TryParseRfc3339DateTime(dateString, out pubDate);
                    }

                    article.Author = "";
                    article.Title = item.Element("title") != null ? item.Element("title").Value : "";
                    article.Description = item.Element("description") != null ? item.Element("description").Value : "";
                    article.Link = item.Element("link") != null ? item.Element("link").Value : "";
                    article.PublishDate = item.Element("pubDate") != null ? pubDate : DateTime.MinValue;
                    article.Id = (id++).ToString();

                    list.Add(article);
                }
            }
            catch { }

            return list;
        }
    }
}
