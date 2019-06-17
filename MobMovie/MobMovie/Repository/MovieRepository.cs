using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MobMovie.Models;
using Newtonsoft.Json;

namespace MobMovie.Repository
{
    /// <summary>
    /// In this class, database and ombdapi operations are performed.
    /// </summary>
    public class MovieRepository : IMovieRepository
    {
        private readonly IConfiguration _config;
        private string connectionString = String.Empty;

        public MovieRepository(IConfiguration config)
        {
            _config = config;
            //appsettings.json / MyConnectionString
            connectionString = _config.GetConnectionString("MyConnectionString");
        }

        //This constructor is written for unit testing.
        public MovieRepository()
        {
            connectionString = "Server=127.0.0.1,1400;Database=moviedb;User=sa;Password=Your_password123;";
        }

        public IDbConnection Connection
        {
            get
            {
                SqlConnection conn = new SqlConnection(connectionString);
                return conn;
            }
        }

        /// <summary>
        /// This method returns a single movie data according to the 'id' field.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Movie> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT Id, Title, Year, ImdbID, Type, Poster FROM Movies WHERE Id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<Movie>(sQuery, new { Id = id });
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// This method, returns many movie data according to the 'title' field.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<List<Movie>>GetByTitle(string title)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT Id, Title, Year, ImdbID, Type, Poster FROM Movies WHERE Title like @title";
                conn.Open();
                var result = await conn.QueryAsync<Movie>(sQuery, new { Title = "%" + title + "%" });
                return result.ToList();
            }
        }

        /// <summary>
        /// This method, returns many movie data according to the 'title' field from ombdapi.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<SearchModel> GetMoviesFromOmdbByTitle(string title)
        {
            SearchModel search = new SearchModel();
            using (WebClient client = new WebClient())
            {
                // client.Headers.Add(RequestConstants.UserAgent, RequestConstants.UserAgentValue);  // <<=asp.net ile yapacagin zaman bunu kullan
                //client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                var response = client.DownloadString("http://www.omdbapi.com/?i=tt3896198&apikey=c8c30b37&s="+title+"&type=movie");
                search = JsonConvert.DeserializeObject<SearchModel>(response);
            }

            return search;
        }

        /// <summary>
        /// This method records movie data.
        /// </summary>
        /// <param name="model"></param>
        [HttpPost()]
        public async void SaveMovie(Movie model)
        {
            using (var conn = Connection)
            {
                string iQuery = @"
                                    INSERT INTO Movies 
                                    (Title
                                    ,Year
                                    ,ImdbID
                                    ,Type
                                    ,Poster)
                                    VALUES 
                                    (@Title
                                    ,@Year
                                    ,@ImdbID
                                    ,@Type
                                    ,@Poster)";
                conn.Open();
                await conn.ExecuteAsync(iQuery, model);
            }
        }

    }
}
