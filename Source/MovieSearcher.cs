using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft;
using System.Net;
using System.IO;

namespace DataProvider
{
    public class MovieSearcher
    {
        public struct MovieData
        {
            public string number;
            public string imdbId;
            public string name;
            public string year;
        };

        private static List<MovieData> GetFoundMovies(string str)
        {
            Regex reg = new Regex(
                "<[\\s]*td[\\s]*class[\\s]*=[\\s]*\"number\"[\\s]*>[\\s]*([1-9]+)[\\s\\S]*?<[\\s]*/[\\s]*td[\\s]*>" +
                "[\\s\\S]*?<[\\s]*a[\\s]*href[\\s]*=[\\s]*\"/title/([\\S]+?)/\"[\\s]*>[\\s]*([\\s\\S]+?)[\\s]*<[\\s]*/[\\s]*a[\\s]*>" +
                "[\\s\\S]*?<[\\s]*span[\\s]*class[\\s]*=[\\s]*\"year_type\"[\\s]*>[\\s\\S]*?([0-9]+)[\\s\\S]*?<[\\s]*/[\\s]*span[\\s]*>"
            );
            List<MovieData> result = new List<MovieData>();
            Match matched = reg.Match(str);
            while (matched.Success)
            {
                result.Add(new MovieData
                {
                    number = matched.Groups[1].Value,
                    imdbId = matched.Groups[2].Value,
                    name = matched.Groups[3].Value,
                    year = matched.Groups[4].Value
                });
                matched = matched.NextMatch();
            }
            return result;
        }

        private static string GetRequestString(string param)
        {
            return "http://www.imdb.com/search/title?count=100&genres=sci_fi&plot=" + param + "&title_type=feature,documentary";
        }

        public static List<MovieData> SearchMovies(string param)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader readStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(GetRequestString(param));
                response = (HttpWebResponse)request.GetResponse();
                readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return GetFoundMovies(readStream.ReadToEnd());
            }
            finally
            {
                readStream.Close();
                response.Close();
            }
        }
    }
}
