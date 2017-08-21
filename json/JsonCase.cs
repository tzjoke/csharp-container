using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace hwapp
{
    public static class JsonCase
    {
        public static void TestBaseSerialize()
        {
            var person = new Person
            {
                name = "Lily",
                hobbies = new string[] {"swimming", "singing"},
                birth = new DateTime(1999, 1, 2)
            };

            var stringOutput = JsonConvert.SerializeObject(person);
            var objectOutput = JsonConvert.DeserializeObject<Person>(stringOutput);
        }

        public static void TestSerializeSetting()
        {
            var errors = new List<string>();
            var dts = JsonConvert.DeserializeObject<List<DateTime>>(@"[
                '2017-01-01T00:00:00Z',
                '123',
                null,
                [123] 
            ]",
            new JsonSerializerSettings
            {
                Error = delegate(object sender, ErrorEventArgs arg)
                {
                    errors.Add(arg.ErrorContext.Error.Message);
                    arg.ErrorContext.Handled = true;
                },
                Converters = {new IsoDateTimeConverter()}
            });  
        }

        public static void TestJsonString()
        {
            var jobject = JObject.Parse(@"{
              'CPU': 'Intel',
              'Drives': [
                'DVD read/writer',
                '500 gigabyte hard drive'
              ]
            }");

            string cpu = (string)jobject["CPU"];
            IList<string> allDrives = jobject["Drives"].Select(t => (string)t).ToList();

            var jarray = JArray.Parse(@"['1','2',3]");
        }

        public static void TestWriteLinq()
        {
            var posts = new List<Post>
            {
                new Post{Title="a", Description="aaa", Link="alink", Categories=new string[]{"A", "B"}},
                new Post{Title="b", Description="bbb", Link="blink", Categories=new string[]{"C"}}
            };
            var rss =
                new JObject(
                    new JProperty("channel",
                        new JObject(
                            new JProperty("title", "James Newton-King"),
                            new JProperty("link", "http://james.newtonking.com"),
                            new JProperty("description", "James Newton-King's blog."),
                            new JProperty("item",
                                new JArray(
                                    from p in posts
                                    orderby p.Title
                                    select new JObject(
                                        new JProperty("title", p.Title),
                                        new JProperty("description", p.Description),
                                        new JProperty("link", p.Link),
                                        new JProperty("category",
                                            new JArray(
                                                from c in p.Categories
                                                select new JValue(c)))))))));
        }

        public static void TestMergeJson()
        {
            var o1 = JObject.Parse(@"{
              'FirstName': 'John',
              'LastName': 'Smith',
              'Enabled': false,
              'Roles': [ 'User' ]
            }");
            var o2 = JObject.Parse(@"{
              'Enabled': true,
              'Roles': [ 'User', 'Admin' ]
            0}");
            
            o1.Merge(o2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            string mergedJson = o1.ToString();  
        }

        public static void TestModifyJson()
        {
            string json = @"{
              'channel': {
                'title': 'Star Wars',
                'link': 'http://www.starwars.com',
                'description': 'Star Wars blog.',
                'obsolete': 'Obsolete value',
                'item': []
              }
            }";
            
            JObject rss = JObject.Parse(json);
            
            JObject channel = (JObject)rss["channel"];
            
            channel["title"] = ((string)channel["title"]).ToUpper();
            channel["description"] = ((string)channel["description"]).ToUpper();
            
            channel.Property("obsolete").Remove();
            
            channel.Property("description").AddAfterSelf(new JProperty("new", "New value"));
            
            JArray item = (JArray)channel["item"];
            item.Add("Item 1");
            item.Add("Item 2");  

            Console.WriteLine(rss.ToString());
        }

        public static void TestXmlJson()
        {
            string xml = @"<person xmlns:json='http://james.newtonking.com/projects/json' id='1'>
            <name>Alan</name>
            <url>http://www.google.com</url>
            <role json:Array='true'>Admin</role>
            </person>";

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(doc);
            var xmlDoc = JsonConvert.DeserializeXmlNode(json);
        }
        public static void Test()
        {
                                       
        }
    }
}