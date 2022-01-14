using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Http;
using cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsPage.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class HackerNewsController : Controller
    {
        private readonly IMemoryCache _cache;

        //public HackerNewsController(ILogger<HackerNews> logger, IMemoryCache memoryCache)
        public HackerNewsController( IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        // GET: HackerNewsController
        [Microsoft.AspNetCore.Mvc.HttpGet("{page:int}")]
        public async Task<ActionResult> GetPage(int page)
        {
            string BaseUrl = "https://hacker-news.firebaseio.com/";
            HackerNews resp = new HackerNews();
            List<HackerNews> itemList = new List<HackerNews>();
            // List of ID's referencing to the latest posts from Hacker News
            List<int> newestStoriesIds = new List<int>();
            // Indexes used for paging
            int minItemIndex = 20 * page;
            int maxItemIndex = 20 * (page + 1);
            // values returned from cache if any exist
            List<HackerNews> cachedItems = new List<HackerNews>();

            // Check cache for posts
            var items = _cache.TryGetValue<List<HackerNews>>(page, out cachedItems);

            if (!items)
            {
                // Get the newest items from Hacker News
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // This is just getting the item ID's of the newest stories
                    HttpResponseMessage Res = await client.GetAsync("/v0/newstories.json?print=pretty");

                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api
                        var idResponse = Res.Content.ReadAsStringAsync().Result;
                        newestStoriesIds = JsonConvert.DeserializeObject<List<int>>(idResponse);

                        // if the number of ID's returned is less than our page size, set max index to number returned
                        if (newestStoriesIds.Count < maxItemIndex)
                        {
                            maxItemIndex = newestStoriesIds.Count;
                            minItemIndex = maxItemIndex - 20;
                        }

                        for (int index = minItemIndex; index < maxItemIndex; index++)
                        {
                            Res = await client.GetAsync("v0/item/" + newestStoriesIds[index] + ".json?print=pretty");
                            //Storing the response details recieved from web api
                            if (Res.IsSuccessStatusCode)
                            {
                                var itemResponse = Res.Content.ReadAsStringAsync().Result;
                                resp = JsonConvert.DeserializeObject<HackerNews>(itemResponse);
                                itemList.Add(resp);
                            }
                        }
                        // Setup caching options
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                        _cache.Set(page, itemList, cacheEntryOptions);
                    }
                }
            }
            else
            {
                itemList = cachedItems;
            }

            if (itemList.Count > 0)
            {
                return Ok(itemList);
            }
            else 
            {
                return NoContent();
            }

        }



        // GET: HackerNewsController/search
        [Microsoft.AspNetCore.Mvc.HttpGet("search/{search}")]
        public async Task<ActionResult> GetSearch(string search)
        {

            string BaseUrl = "https://hacker-news.firebaseio.com/";
            HackerNews resp = new HackerNews();
            List<HackerNews> itemList = new List<HackerNews>();
            List<HackerNews> allItemsList = new List<HackerNews>();
            // List of ID's referencing to the latest posts from Hacker News
            List<int> newestStoriesIds = new List<int>();
            // values returned from cache if any exist
            List<HackerNews> cachedItems = new List<HackerNews>();


            // Check cache for posts
            var items = _cache.TryGetValue<List<HackerNews>>("search", out cachedItems);

            if (!items)
            {

                // Get the newest items from Hacker News
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("/v0/newstories.json?print=pretty");

                    if (Res.IsSuccessStatusCode)
                    {

                        //Storing the response details recieved from web api
                        var idResponse = Res.Content.ReadAsStringAsync().Result;
                        newestStoriesIds = JsonConvert.DeserializeObject<List<int>>(idResponse);

                        for (int index = 0; index < newestStoriesIds.Count; index++)
                        {
                            Res = await client.GetAsync("v0/item/" + newestStoriesIds[index] + ".json?print=pretty");
                            //Storing the response details recieved from web api
                            if (Res.IsSuccessStatusCode)
                            {
                                var itemResponse = Res.Content.ReadAsStringAsync().Result;
                                resp = JsonConvert.DeserializeObject<HackerNews>(itemResponse);
                                if (resp != null)
                                {
                                    allItemsList.Add(resp);

                                    if (resp.title.Contains(search))
                                    {
                                        itemList.Add(resp);
                                    }
                                }
                            }
                        }
                    }

                    // Setup caching options
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(120));
                    _cache.Set("search", allItemsList, cacheEntryOptions);
                }
            }
            else
            {
                foreach(var item in cachedItems)
                if (item.title.Contains(search))
                {
                    itemList.Add(item);
                }


            }

            if (itemList.Count > 0)
            {
                return Ok(itemList);
            }
            else 
            {
                return NoContent();
            }
        }


        // GET: HackerNewsController/count
        [Microsoft.AspNetCore.Mvc.HttpGet("count")]
        public async Task<ActionResult> GetCount()
        {

            string BaseUrl = "https://hacker-news.firebaseio.com/";
            HackerNews resp = new HackerNews();
            List<HackerNews> itemList = new List<HackerNews>();
            // List of ID's referencing to the latest posts from Hacker News
            List<int> newestStoriesIds = new List<int>();
            // values returned from cache if any exist
            List<HackerNews> cachedItems = new List<HackerNews>();

            // Get the newest items from Hacker News
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("/v0/newstories.json?print=pretty");

                if (Res.IsSuccessStatusCode)
                {

                    //Storing the response details recieved from web api
                    var idResponse = Res.Content.ReadAsStringAsync().Result;
                    newestStoriesIds = JsonConvert.DeserializeObject<List<int>>(idResponse);
                }
            }

            return Ok(newestStoriesIds.Count);
        }


    }
}
