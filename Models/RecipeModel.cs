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
        public string Name { get; set; }
        [Required]
        [DisplayName ("Description here")]
        public string Description { get; set; }

        public RecipeModel()
        {
            Id = -1;
            Name = "";
            Description = "";
        }

        public RecipeModel(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}