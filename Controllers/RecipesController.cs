using RecipeCRUD.Data;
using RecipeCRUD.Models;
using System;
using System.Collections.Generic;
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

            recipes = recipeDAO.FetchAll();

            return View("Index", recipes);
        }

        public ActionResult Details(int id, bool isFromApi)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            RecipeModel recipe = recipeDAO.FetchOne(id, isFromApi);
            return View("Details", recipe);
        }
        public ActionResult Create()
        {
            return View("RecipeForm", new RecipeModel());
        }
        public ActionResult Edit(int id, bool isFromApi)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            RecipeModel recipe = recipeDAO.FetchOne(id, isFromApi);

            return View("RecipeForm", recipe);
        }
        public ActionResult Delete(int id)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            recipeDAO.Delete(id);

            List<RecipeModel> recipes = recipeDAO.FetchAll();

            return View("Index", recipes);
        }
        public ActionResult ProcessCreate(RecipeModel recipeModel)
        {
            //Save to database
            RecipeDAO recipeDAO= new RecipeDAO();
            recipeDAO.CreateOrUpdate(recipeModel);

            return View("Details", recipeModel);
        }
        public ActionResult SearchForm()
        {
            return View("SearchForm");
        }
        public ActionResult SearchForName(string searchPhrase)
        {
            RecipeDAO recipeDAO = new RecipeDAO();
            List<RecipeModel> searchResults = recipeDAO.SearchForName(searchPhrase);

            return View("Index", searchResults);
        }
    }
}