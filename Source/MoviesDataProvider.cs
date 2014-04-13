using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataProvider
{
    public class MoviesDataProvider
    {
        private Dictionary<string, List<MovieSearcher.MovieData>> tagMovies = new Dictionary<string, List<MovieSearcher.MovieData>>();
        private Dictionary<string, List<string>> movieTags = new Dictionary<string, List<string>>();

        public MoviesDataProvider(List<string> tags)
        {
            foreach(var tag in tags)
            {
                var movies = MovieSearcher.SearchMovies(tag);
                tagMovies.Add(tag, movies);
                foreach (var movie in movies)
                {
                    if (movieTags.ContainsKey(movie.name))
                        movieTags[movie.name].Add(tag);
                    else
                    {
                        var tagList = new List<string>();
                        tagList.Add(tag);
                        movieTags.Add(movie.name, tagList);
                    }
                }
            }
        }

        public List<MovieSearcher.MovieData> GetMoviesForTag(string tag)
        {
            if (!tagMovies.ContainsKey(tag))
                throw new Exception("Get movies failed: tag not found");
            return tagMovies[tag];
        }

        public List<string> GetTagsForMovie(MovieSearcher.MovieData movie)
        {
            if(!movieTags.ContainsKey(movie.name))
                throw new Exception("Get tag failed: movie not found");
            return movieTags[movie.name];
        }
    }
}
