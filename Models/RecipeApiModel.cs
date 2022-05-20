using System.Collections.Generic;

namespace RecipeCRUD.Models
{
    public class Ingredients
    {
        public string ingredientName { get; set; }
    }

    public class Steps
    {
        public int number { get; set; }
        public string description { get; set; }
        public IList<Ingredients> ingredients { get; set; }
    }

    public class results
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string imageType { get; set; }
        public IList<Steps> steps { get; set; }
    }

    public class ComplexSearch
    {
        public IList<results> results { get; set; }
    }
}