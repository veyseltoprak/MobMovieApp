using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MobMovie.Models;
using MobMovie.Repository;
using Newtonsoft.Json;

namespace MobMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IDistributedCache _distributedCache;
        private readonly int cacheExpire = 12;

        public MovieController(IMovieRepository movieRepo, IDistributedCache distributedCache)
        {
            _movieRepo = movieRepo;
            _distributedCache = distributedCache;
        }

        // GET api/movie/1
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Movie>> GetByID(int id)
        {
            var cacheKey = "Movies";
            var movies = _distributedCache.GetString(cacheKey);
            string movieString = String.Empty;
            bool cacheStatus = true;
            bool isData = true;
            List<Movie> movieList = new List<Movie>();

            //If the searched movie is not in memory, it is retrieved from the database.
            if (string.IsNullOrEmpty(movies))
            {
                cacheStatus = false;
            }
            else
            {
                movieString = movies;
                movieList = JsonConvert.DeserializeObject<List<Movie>>(movieString);
                if(!movieList.Any(x => x.Id == id))
                {
                    isData = false;
                }
            }
            if (cacheStatus == false || isData == false)
            {
                var movieFirst = await _movieRepo.GetByID(id);
                if (movieFirst != null)
                {
                    movieList.Add(movieFirst);
                    var data = JsonConvert.SerializeObject(movieList);
                    var dataByte = Encoding.UTF8.GetBytes(data);

                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60 * cacheExpire));
                    option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60 * cacheExpire);

                    _distributedCache.Set(cacheKey, dataByte, option);
                }
            }

            movieString = await _distributedCache.GetStringAsync(cacheKey);
            return Ok(JsonConvert.DeserializeObject<List<Movie>>(movieString).FirstOrDefault(x => x.Id == id));
        }

        // GET api/movie/search/breaking
        [HttpGet]
        [Route("search/{title}")]
        public async Task<ActionResult<Movie>> GetOneByTitle(string title)
        {
            var cacheKey = "Movies";
            var movies = _distributedCache.GetString(cacheKey);
            string movieString = String.Empty;
            bool cacheStatus = true;
            bool isData = true;
            List<Movie> movieList = new List<Movie>();

            //If the searched movie is not in memory, it is retrieved from the database.
            if (string.IsNullOrEmpty(movies))
            {
                cacheStatus = false;
            }
            else
            {
                movieString = movies;
                movieList = JsonConvert.DeserializeObject<List<Movie>>(movieString);
                if (!movieList.Any(x => x.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase)))
                {
                    isData = false;
                }
            }

            //If the searched movie is not in database, it is retrieved from the omdbapi.
            if (cacheStatus == false || isData == false)
            {
                var movieSearch = await _movieRepo.GetByTitle(title);
                if (movieSearch != null && movieSearch.Any())
                {
                    movieList.AddRange(movieSearch);
                    var data = JsonConvert.SerializeObject(movieList);
                    var dataByte = Encoding.UTF8.GetBytes(data);

                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60 * cacheExpire));
                    option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60 * cacheExpire);

                    _distributedCache.Set(cacheKey, dataByte, option);
                }
                else
                {
                    var searchModel = await _movieRepo.GetMoviesFromOmdbByTitle(title);
                    if (searchModel.Response)
                    {
                        foreach (var movieItem in searchModel.Search)
                        {
                            if (!movieList.Any(x => x.Title.Equals(movieItem.Title)))
                            {
                                _movieRepo.SaveMovie(movieItem);
                                movieList.Add(movieItem);
                            }
                        }
                        var data = JsonConvert.SerializeObject(movieList);
                        var dataByte = Encoding.UTF8.GetBytes(data);

                        var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60 * cacheExpire));
                        option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60 * cacheExpire);

                        _distributedCache.Set(cacheKey, dataByte, option);
                    }
                }
            }

            movieString = await _distributedCache.GetStringAsync(cacheKey);
            var firstMovie = JsonConvert.DeserializeObject<List<Movie>>(movieString).Where(x => x.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if(firstMovie == null)
            {
                firstMovie = new Movie();
            }
            return Ok(firstMovie);
        }

        // GET api/movie/searchMany/breaking
        [HttpGet]
        [Route("searchMany/{title}")]
        public async Task<ActionResult<List<Movie>>> GetByTitle(string title)
        {
            var cacheKey = "Movies";
            var movies = _distributedCache.GetString(cacheKey);
            string movieString = String.Empty;
            bool cacheStatus = true;
            bool isData = true;
            List<Movie> movieList = new List<Movie>();

            //If the searched movie is not in memory, it is retrieved from the database.
            if (string.IsNullOrEmpty(movies))
            {
                cacheStatus = false;
            }
            else
            {
                movieString = movies;
                movieList = JsonConvert.DeserializeObject<List<Movie>>(movieString);
                if (!movieList.Any(x => x.Title.Contains(title,StringComparison.CurrentCultureIgnoreCase)))
                {
                    isData = false;
                }
            }

            //If the searched movie is not in database, it is retrieved from the omdbapi.
            if (cacheStatus == false || isData == false)
            {
                var movieSearch = await _movieRepo.GetByTitle(title);
                if (movieSearch != null && movieSearch.Any())
                {
                    movieList.AddRange(movieSearch);
                    var data = JsonConvert.SerializeObject(movieList);
                    var dataByte = Encoding.UTF8.GetBytes(data);

                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60 * cacheExpire));
                    option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60 * cacheExpire);

                    _distributedCache.Set(cacheKey, dataByte, option);
                }
                else
                {
                    var searchModel = await _movieRepo.GetMoviesFromOmdbByTitle(title);
                    if(searchModel.Response)
                    {
                        foreach (var movieItem in searchModel.Search)
                        {
                            if(!movieList.Any(x => x.Title.Equals(movieItem.Title)))
                            {
                                _movieRepo.SaveMovie(movieItem);
                                movieList.Add(movieItem);
                            }
                        }
                        var data = JsonConvert.SerializeObject(movieList);
                        var dataByte = Encoding.UTF8.GetBytes(data);

                        var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60 * cacheExpire));
                        option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60 * cacheExpire);

                        _distributedCache.Set(cacheKey, dataByte, option);
                    }
                }
            }

            movieString = await _distributedCache.GetStringAsync(cacheKey);
            return Ok(JsonConvert.DeserializeObject<List<Movie>>(movieString).Where(x => x.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase)).ToList());
        }
    }
}