using AutoFixture;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MobMovie.Controllers;
using MobMovie.Models;
using MobMovie.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MobMovieTest
{
    /// <summary>
    /// This class controls the result types of the methods.
    /// </summary>
    public class MovieTests
    {

        IMovieRepository _movieRepository;

        public MovieTests()
        {
            _movieRepository = new MovieRepository();
        }

        /// <summary>
        /// The result type should be of 'Movie' type.
        /// </summary>
        [Fact]
        public async void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var result = await _movieRepository.GetByID(1);

            // Assert
            Assert.IsType<Movie>(result);
        }

        /// <summary>
        /// The result type should be of 'List<Movie>' type.
        /// </summary>
        [Fact]
        public async void Get_WhenCalled_ReturnsAllItems()
        {
            // Act
            var result = await _movieRepository.GetByTitle("breaking");

            // Assert
            Assert.IsType<List<Movie>>(result);
        }

    }
}
