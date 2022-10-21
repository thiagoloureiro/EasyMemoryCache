using Microsoft.AspNetCore.Mvc;

namespace EasyMemoryCache.SampleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheTestController : ControllerBase
    {
        private readonly ILogger<CacheTestController> _logger;
        private readonly ICaching _caching;

        public CacheTestController(ILogger<CacheTestController> logger, ICaching caching)
        {
            _logger = logger;
            _caching = caching;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _caching.GetOrSetObjectFromCacheAsync("cacheKey", 10, ReturnListOfStringAsync);
            return Ok(data);
        }

        [HttpGet("FlushAll")]
        public async Task<IActionResult> FlushAll()
        {
            await _caching.InvalidateAllAsync();
            return Ok();
        }

        private Task<List<string>> ReturnListOfStringAsync()
        {
            return Task.Run(GenerateList);
        }

        private List<string> GenerateList()
        {
            Console.WriteLine("Generating the list...");
            return new List<string> { "foo", "bar", "easy", "caching" };
        }
    }
}