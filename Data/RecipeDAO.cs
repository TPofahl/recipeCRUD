using RecipeCRUD.Models;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace RecipeCRUD.Data
{
    internal class RecipeDAO
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["SQLCONNSTR_recipeconnectionstring"].ConnectionString;
        readonly string spoonacularKey = ConfigurationManager.AppSettings["KEY_spoonacular"];
        List<RecipeModel> returnList = new List<RecipeModel>();

        public List<RecipeModel> FetchAll()
        {
            List<RecipeModel> returnList = new List<RecipeModel>();

            // Access database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM dbo.Recipes";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RecipeModel recipe = new RecipeModel();
                        recipe.Id = reader.GetInt32(0);
                        recipe.Name = reader.GetString(3);
                        recipe.Image = reader.GetString(5);
                        recipe.Description = reader.GetString(4);
                        recipe.Ingredients = reader.GetString(10);
                        recipe.Steps = reader.GetString(11);

                        returnList.Add(recipe);
                    }
                }
                connection.Close();
            }
            return returnList;
        }

        internal int Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM dbo.Recipes WHERE id = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.Add("@Id", System.Data.SqlDbType.VarChar, 1000).Value = id;

                connection.Open();
                int deletedID = command.ExecuteNonQuery();
                connection.Close();

                return deletedID;
            }
        }

        internal List<RecipeModel> SearchForName(string searchPhrase)
        {
            if (searchPhrase == "")
            {
                return (List<RecipeModel>)HttpContext.Current.Session["recipeState"];
            }
            string result = null;
            string path = string.Format("https://api.spoonacular.com/recipes/complexSearch?query=" + searchPhrase + "&number=100&addRecipeInformation=true&apiKey=" + spoonacularKey);
            WebRequest request = WebRequest.Create(path);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                result = reader.ReadToEnd();
                reader.Close();
            }

            JObject resultParsed = JObject.Parse(result);
            JArray list = (JArray)resultParsed["results"];

            // Step through each recipe, and save recipe instructions
            for (int i = 0; i < list.Count(); i++)
            {
                RecipeModel recipe = new RecipeModel();
                var steps = new List<string>();
                var ingredients = new List<string>();

                JObject objA = (JObject)list[i];
                JArray arrA = (JArray)objA["analyzedInstructions"];
                // Some recipes from the API do not have instructions, exclude from results.
                if (arrA.HasValues)
                {
                    JObject objB = (JObject)arrA[0];
                    JArray arrB = (JArray)objB["steps"];
                    JObject objC = (JObject)arrB[0];
                    JArray arrC = (JArray)objC["ingredients"];

                    // Find all steps
                    foreach (JObject obj in arrB)
                    {
                        steps.Add(obj["step"].ToString());
                    }
                    // Find all ingredients, exclude duplicates
                    foreach (JObject obj in arrC)
                    {
                        string ingredient = obj["name"].ToString();
                        if (ingredients.Contains(ingredient) == false)
                        {
                            ingredients.Add(ingredient.ToString());
                        }
                    }
                    recipe.Id = -1;
                    recipe.SpoonacularId = (int)resultParsed.SelectToken("$..results[" + i + "].id");
                    recipe.Name = (string)resultParsed.SelectToken("$..results[" + i + "].title");
                    recipe.Image = (string)resultParsed.SelectToken("$..results[" + i + "].image");
                    recipe.Description = (string)resultParsed.SelectToken("$..results[" + i + "].summary");
                    recipe.Steps = String.Join("|", steps.ToArray());
                    recipe.Ingredients = String.Join("|", ingredients.ToArray());
                    recipe.IsFromApi = true;

                    returnList.Add(recipe);
                }
            }

            // Access database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM dbo.Recipes WHERE TITLE LIKE @search";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add("@search", System.Data.SqlDbType.NVarChar).Value = "%" + searchPhrase + "%";

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RecipeModel recipe = new RecipeModel(); 
                        recipe.Id = reader.GetInt32(0);
                        recipe.Name = reader.GetString(3);
                        recipe.Image = reader.GetString(5);
                        recipe.Description = reader.GetString(4);
                        recipe.Ingredients = reader.GetString(10);
                        recipe.Steps = reader.GetString(11);
                        recipe.IsFromApi = false;

                        returnList.Add(recipe);
                    }
                }
                connection.Close();
            }
            return returnList;
        }

        public RecipeModel FetchOne(int id, int spoonacularId, bool isFromApi)
        {
            RecipeModel recipe = new RecipeModel();

            List<RecipeModel> recipeList = (List<RecipeModel>)HttpContext.Current.Session["recipeState"];
            RecipeModel selected = recipeList.First(x => x.SpoonacularId == spoonacularId);
            HttpContext.Current.Session["selectedState"] = selected;

            if (selected.IsFromApi)
            {
                recipe.Id = selected.Id;
                recipe.SpoonacularId = selected.SpoonacularId;
                recipe.Name = selected.Name;
                recipe.Image = selected.Image;
                recipe.Description = selected.Description;
                recipe.Ingredients = selected.Ingredients;
                recipe.Steps = selected.Steps;
                recipe.IsFromApi = selected.IsFromApi;
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM dbo.Recipes WHERE Id = @id";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipe.Id = reader.GetInt32(0);
                            recipe.Name = reader.GetString(3);
                            recipe.Image = reader.GetString(5);
                            recipe.Description = reader.GetString(4);
                            recipe.Ingredients = reader.GetString(10);
                            recipe.Steps = reader.GetString(11);
                            recipe.IsFromApi = selected.IsFromApi;
                        }
                    }
                    connection.Close();
                }
            }
            return recipe;
        }

        public int CreateOrUpdate(RecipeModel recipeModel)
        {
            // Access database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "";
                DateTime date = DateTime.ParseExact("14/06/1994", "dd/MM/yyyy", null);
                var httpContext = HttpContext.Current;
                var userId = httpContext.User.Identity.GetUserId();

                if (recipeModel.Id <= 0)
                {
                    // create
                    sqlQuery = "INSERT INTO dbo.Recipes VALUES(@UserId, @Recipe_ID, @Title, @Description, @Image, @Image_Type, @Is_Public, @Date_Created, @Date_Modified, @Ingredients, @Steps)";
                    recipeModel = (RecipeModel)HttpContext.Current.Session["selectedState"];
                } else
                {
                    // update
                    sqlQuery = "UPDATE dbo.Recipes SET Name = @Name, Description = @Description WHERE Id = @Id";
                }
                    
                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.Add("@Id", System.Data.SqlDbType.VarChar, 1000).Value = recipeModel.Id;
                command.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar, 1000).Value = userId;
                command.Parameters.Add("@Recipe_ID", System.Data.SqlDbType.Char, 1000).Value = recipeModel.SpoonacularId;
                command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Name;
                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = recipeModel.Description;
                command.Parameters.Add("@Image", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Image;
                command.Parameters.Add("@Image_Type", System.Data.SqlDbType.Char, 1000).Value = recipeModel.ImageType;
                command.Parameters.Add("@Is_Public", System.Data.SqlDbType.Int, 1000).Value = recipeModel.IsPublic;
                command.Parameters.Add("@Date_Created", System.Data.SqlDbType.DateTime, 1000).Value = date;
                command.Parameters.Add("@Date_Modified", System.Data.SqlDbType.DateTime, 1000).Value = date;
                command.Parameters.Add("@Ingredients", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Ingredients;
                command.Parameters.Add("@Steps", System.Data.SqlDbType.NVarChar).Value = recipeModel.Steps;

                connection.Open();
                int newID = command.ExecuteNonQuery();
                connection.Close();
                HttpContext.Current.Session["selectedState"] = null;

                return newID;
            }
        }
    }
}