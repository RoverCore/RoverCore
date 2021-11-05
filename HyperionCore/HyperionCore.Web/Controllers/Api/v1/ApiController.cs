using Hyperion.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hyperion.Web.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1")]
    public class ApiController : Controller
    {
        private readonly Services.Configuration Configuration;
        private readonly Services.Cache _cache;

        public ApiController(Services.Configuration configuration, Services.Cache cache)
        {
            Configuration = configuration;
            _cache = cache;
        }

        // GET: api/v1/News
        [AllowAnonymous]  // Don't require JWT authentication to access this method
        [HttpGet("News")]
        public async Task<List<NewsFeedItem>> GetNewsFeed()
        {
            var newsUrl = new Uri("https://www.eastonsd.org/apps/news/news_rss.jsp");

            string sourceUrl = newsUrl.GetLeftPart(UriPartial.Authority);
            string endpoint = newsUrl.PathAndQuery;

            Task<List<NewsFeedItem>> fetchNewsFromSource() => Util.RSS.GetNewsFeed(sourceUrl, endpoint);

            var feedItems = await _cache.GetAsync("RSS", fetchNewsFromSource, TimeSpan.FromMinutes(5));
            return feedItems.OrderByDescending(x => x.PublishDate).ToList();
        }

        // GET: api/Config
        [AllowAnonymous]
        [HttpGet("Config")]
        public ConfigResponse GetConfig()
        {
            // TODO: extend this object to include some configuration items
            return new ConfigResponse();
        }
    }
}