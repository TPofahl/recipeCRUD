using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RecipeCRUD.Controllers
{
    public class PublicApiController : ApiController
    {
        readonly string spoonacularKey = ConfigurationManager.AppSettings["KEY_spoonacular"];

        static List<string> strings = new List<string>()
        {
            "value0", "value1", "value2"
        };

        // GET: api/PublicApi
        public IEnumerable<string> Get()
        {
            return strings;
        }

        // GET: api/PublicApi/5
        public string Get(string id)
        {
            // string strUrlTest = String.Format("https://jsonplaceholder.typicode.com/posts/1/comments");
            string path = string.Format("https://api.spoonacular.com/recipes/complexSearch?query=pizza&apiKey=" + spoonacularKey);
            WebRequest request = WebRequest.Create(path);
            HttpWebResponse response = null;
            response = (HttpWebResponse)request.GetResponse();

            string result = null;
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                result = reader.ReadToEnd();
                reader.Close();
            }
            System.Diagnostics.Debug.WriteLine(result);

            return result;
        }

        // POST: api/PublicApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/PublicApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PublicApi/5
        public void Delete(int id)
        {
        }
    }
}
