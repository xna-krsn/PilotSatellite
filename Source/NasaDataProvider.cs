using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataProvider
{
    public class NasaDataProvider
    {
        private Dictionary<string, List<NasaData>> tagNasaData = new Dictionary<string, List<NasaData>>();

        public class NasaData
        {
            public string description = string.Empty;
            public List<string> links = new List<string>();
        };

        public NasaDataProvider(Dictionary<string, NasaDataKey> nasaRequestsData)
        {
            foreach (var item in nasaRequestsData)
            {
                tagNasaData.Add(
                    item.Key,
                    ParseJsonData(HttpRequestHelper.GetDataFromUrl(GetRequestString(item.Value.Id)))
                );
            }
        }

        public List<NasaData> GetNasaDataForTag(string tag)
        {
            if (!tagNasaData.ContainsKey(tag))
                throw new Exception("Get nasa data failed: tag not found");
            return tagNasaData[tag];
        }

        private string GetRequestString(string param)
        {
            return "http://data.nasa.gov/api/get_tag_datasets/?id=" + param;
        }

        private List<NasaData> ParseJsonData(string data)
        {
            List<NasaData> nasaDataList = new List<NasaData>();

            JObject parseResult = JObject.Parse(data);
            if (parseResult == null)
                throw new Exception("Parse data failed");

            var postsToken = parseResult["posts"];
            if (postsToken != null)
            {
                List<JToken> posts = postsToken.Children().ToList();
                if (posts == null)
                    throw new Exception("No posts found");

                foreach (var post in posts)
                {
                    NasaData result = new NasaData();
                    var content = (string)post.SelectToken("content");
                    if (content != null)
                    {
                        Regex reg = new Regex("[\\s]*<[\\s]*p[\\s]*>[\\s]*([\\s\\S]*?)[\\s]*<[\\s]*\\/[\\s]*p[\\s]*>[\\s]*");
                        Match matched = reg.Match(content);
                        if (matched.Success)
                            result.description = matched.Groups[1].Value;
                        else
                            result.description = content;
                    }
                    var customFields = post["custom_fields"];
                    if (customFields != null)
                    {
                        var moreInfoLink = customFields["more_info_link"];
                        if (moreInfoLink != null)
                        {
                            List<JToken> links = moreInfoLink.Children().ToList();
                            if (links != null)
                                foreach (var link in links)
                                    result.links.Add(((string)link).Replace("\\/", "/"));
                        }
                    }
                    nasaDataList.Add(result);
                }
            }
            return nasaDataList;
        }
    }
}
