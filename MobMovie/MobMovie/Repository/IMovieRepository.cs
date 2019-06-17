using MobMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobMovie.Repository
{
    public interface IMovieRepository
    {
        Task<Movie> GetByID(int id);
        Task<List<Movie>> GetByTitle(string title);
        Task<SearchModel> GetMoviesFromOmdbByTitle(string title);
        void SaveMovie(Movie model);
    }
}
