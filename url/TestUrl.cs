using System;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace testurl
{
    class TsetUrl
    {
        static void Main(string[] args)
        {
            var address = "http://aaa.com/ShowReport?sys=bbb&name=Demo状态&default={}";

            var uri = new Uri(address);
            var baseUri = uri.GetLeftPart(UriPartial.Path);
            var qs = HttpUtility.ParseQueryString(uri.Query);

            var defaultFilter = qs.Get("default");

            var filter = new Demo
            {
                Foo = "foo",
                Bar = "null"
            };

            var filterString = JsonConvert.SerializeObject(filter);

            var o1 = JObject.Parse(defaultFilter);
            var o2 = JObject.Parse(filterString);

            o1.Merge(o2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            qs.Set("default", Regex.Replace(o1.ToString(), @"\t|\n|\r|\s", ""));

            var newAddress = string.Concat(baseUri, "?", qs.ToString());

            var last = new Uri(HttpUtility.UrlDecode(newAddress)).AbsoluteUri;
            var last2 = Uri.EscapeUriString(HttpUtility.UrlDecode(newAddress));
            var last3 = Uri.EscapeDataString(HttpUtility.UrlDecode(newAddress));

            Console.ReadLine();
        }

        class Demo
        {
            public string Foo { get; set; }
            public string Bar { get; set; }
        }
    }

}
