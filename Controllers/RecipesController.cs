using RecipeCRUD.Data;
using RecipeCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecipeCRUD.Controllers
{
    public class RecipesController : Controller
    {
        // GET: Recipes
        public ActionResult Index()
        {
            List<RecipeModel> recipes = new List<RecipeModel>();
            RecipeDAO recipeDAO = new RecipeDAO();

            recipes = recipeDAO.FetchAll();

            return View("Index", recipes);
        }

        public ActionResult Details(int id)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            RecipeModel recipe = recipeDAO.FetchOne(id);
            return View("Details", recipe);
        }

        public ActionResult Create()
        {
            return View("RecipeForm");
        }

        public ActionResult Edit(int id)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            RecipeModel recipe = recipeDAO.FetchOne(id);

            return View("RecipeForm", recipe);
        }

        public ActionResult ProcessCreate(RecipeModel recipeModel)
        {
            //Save to database
            RecipeDAO recipeDAO= new RecipeDAO();
            recipeDAO.CreateOrUpdate(recipeModel);

            return View("Details", recipeModel);
        }
    }
}