using System.Collections.Generic;

namespace RecipeCRUD.Models
{
    public class results
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string imageType { get; set; }
    }
    public class ComplexSearch
    {
        public IList<results> results { get; set; }
    }
}