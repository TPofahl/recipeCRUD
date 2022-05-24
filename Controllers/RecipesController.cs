using RecipeCRUD.Data;
using RecipeCRUD.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecipeCRUD.Controllers
{
    // [Authorize]
    public class RecipesController : Controller
    {
        // GET: Recipes
        public ActionResult Index()
        {
            List<RecipeModel> recipes = new List<RecipeModel>();
            RecipeDAO recipeDAO = new RecipeDAO();

            if (Session["recipeState"] == null)
            {
                Session["recipeState"] = recipeDAO.FetchAll();
                recipes = (List<RecipeModel>)Session["recipeState"];
            } else
            {
                recipes = (List<RecipeModel>)Session["recipeState"];
            }

            return View("Index", recipes);
        }
        public ActionResult Create()
        {
            return View("RecipeForm", new RecipeModel());
        }
        public ActionResult Delete(int id)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            recipeDAO.Delete(id);

            Session["recipeState"] = recipeDAO.FetchAll();

            return View("Index", Session["recipeState"]);
        }
        public ActionResult Details(int id, int spoonacularId, bool isFromApi)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            RecipeModel recipe = recipeDAO.FetchOne(id, spoonacularId, isFromApi);
            return View("Details", recipe);
        }
        public ActionResult Edit(int id, int spoonacularId, bool isFromApi)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            RecipeModel recipe = recipeDAO.FetchOne(id, spoonacularId, isFromApi);

            return View("RecipeForm", recipe);
        }
        public ActionResult ProcessCreate(RecipeModel recipeModel)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            //Save to database
            recipeDAO.CreateOrUpdate(recipeModel);
            return View("Index", Session["recipeState"]);
        }
        public ActionResult SearchForm()
        {
            return View("SearchForm");
        }
        public ActionResult SearchForName(string searchPhrase)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            Session["recipeState"] = recipeDAO.SearchForName(searchPhrase);

            return View("Index", Session["RecipeState"]);
        }
    }
}