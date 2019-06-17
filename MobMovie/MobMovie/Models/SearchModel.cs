using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobMovie.Models
{
    public class SearchModel
    {
        public List<Movie> Search { get; set; }
        public int TotalResults { get; set; }
        public bool Response { get; set; }
    }
}
