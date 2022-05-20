using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecipeCRUD.Models
{
    public class RecipeModel
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName ("Description here")]
        public string Description { get; set; }
        public string Ingredients { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string ImageType { get; set; }
        [Required]
        public int IsPublic { get; set; }
        public bool IsFromApi { get; set; }
        [Required]
        public string Steps { get; set; }



        public RecipeModel()
        {
            Id = -1;
            UserId = "";
            Name = "";
            Description = "";
            Image = "https://spoonacular.com/recipeImages/654958-312x231.jpg";
            ImageType = "jpg";
            IsPublic = 1;
            IsFromApi = false;
            Ingredients = "";
            Steps = "";
        }

        public RecipeModel(int id, string name, string description, string ingredients, string steps)
        {
            Id = id;
            UserId = "";
            Name = name;
            Description = description;
            Image = "https://spoonacular.com/recipeImages/654958-312x231.jpg";
            ImageType = "jpg";
            IsPublic = 1;
            IsFromApi= false;
            Ingredients = ingredients;
            Steps = steps;
        }
    }
}