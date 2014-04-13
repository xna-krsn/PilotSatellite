using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace DataProvider
{
    public class HttpRequestHelper
    {
        public static string GetDataFromUrl(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader readStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return readStream.ReadToEnd();
            }
            finally
            {
                readStream.Close();
                response.Close();
            }
        }
    }
}
