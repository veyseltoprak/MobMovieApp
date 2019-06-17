using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MobMovie.Models;
using MobMovie.Repository;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobMovie.Schedule
{
    public class ScheduledJob : IJob
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ScheduledJob> logger;
        private readonly IMovieRepository movieRepository;

        public ScheduledJob(IConfiguration configuration, ILogger<ScheduledJob> logger, IMovieRepository movieRepository)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.movieRepository = movieRepository;
        }

        /// <summary>
        /// This method,  returns many movie data from ombdapi by 'title' each time it is triggered.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            string[] wordArray = { "Breaking", "Game", "Catch", "Cat", "Dog", "System", "Heaven", "World", "Seven", "Remember" };
            Random random = new Random();
            string randomWord = wordArray[random.Next(0,10)];
            SearchModel searchModel = await movieRepository.GetMoviesFromOmdbByTitle(randomWord);

            if (searchModel.Response)
            {
                var movieList = await movieRepository.GetByTitle(randomWord);
                foreach (var movieItem in searchModel.Search)
                {
                    if (!movieList.Any(x => x.Title.Equals(movieItem.Title)))
                    {
                        movieRepository.SaveMovie(movieItem);
                    }
                }
            }

            //Schedule log
            this.logger.LogWarning($"Movies was fetched. {DateTime.Now.ToLongTimeString()}");

            await Task.CompletedTask;

        }
    }
}
