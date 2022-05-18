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

namespace RecipeCRUD.Data
{
    internal class RecipeDAO
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["SQLCONNSTR_recipeconnectionstring"].ConnectionString;

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
            List<RecipeModel> returnList = new List<RecipeModel>();

            // string strUrlTest = String.Format("https://jsonplaceholder.typicode.com/posts/1/comments");
            string path = string.Format("https://api.spoonacular.com/recipes/complexSearch?query=pizza&apiKey=" + "key goes here");
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
            // remove this later
            Debug.WriteLine(result);

            
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            ComplexSearch objList = (ComplexSearch) serializer.Deserialize(result, typeof(ComplexSearch));
            
            foreach (results list in objList.results)
            {
                RecipeModel recipe = new RecipeModel();
                recipe.Id = list.id;
                recipe.Name = list.title;
                recipe.Image = list.image;
                string imageType = list.imageType;

                returnList.Add(recipe);
            }

            // List<RecipeModel> returnList = new List<RecipeModel>();
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

                        returnList.Add(recipe);
                    }
                }
                connection.Close();
            }
            return returnList;
        }

        internal List<RecipeModel> SearchForDescription(string searchPhrase)
        {
            List<RecipeModel> returnList = new List<RecipeModel>();

            // Access database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM dbo.Recipes WHERE DESCRIPTION LIKE @searchForMe";
                 

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add("@searchForMe", System.Data.SqlDbType.NVarChar).Value = "%" + searchPhrase + "%";


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

        public RecipeModel FetchOne(int id)
        {
            RecipeModel recipe = new RecipeModel();

            // Access database
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
                    }
                }
                connection.Close();
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

                Debug.WriteLine("This is a test 2");
                Debug.WriteLine(userId);

                if (recipeModel.Id <= 0)
                {
                    // create
                    sqlQuery = "INSERT INTO dbo.Recipes VALUES(@UserId, @Recipe_ID, @Title, @Description, @Image, @Image_Type, @Is_Public, @Date_Created, @Date_Modified, @Ingredients, @Steps)";
                } else
                {
                    // update
                    sqlQuery = "UPDATE dbo.Recipes SET Name = @Name, Description = @Description WHERE Id = @Id";
                }
                    
                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.Add("@Id", System.Data.SqlDbType.VarChar, 1000).Value = recipeModel.Id;
                command.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar, 1000).Value = userId;
                command.Parameters.Add("@Recipe_ID", System.Data.SqlDbType.Char, 1000).Value = "RecipeIDhere";
                command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Name;
                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Description;
                command.Parameters.Add("@Image", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Image;
                command.Parameters.Add("@Image_Type", System.Data.SqlDbType.Char, 1000).Value = recipeModel.ImageType;
                command.Parameters.Add("@Is_Public", System.Data.SqlDbType.Int, 1000).Value = recipeModel.IsPublic;
                command.Parameters.Add("@Date_Created", System.Data.SqlDbType.DateTime, 1000).Value = date;
                command.Parameters.Add("@Date_Modified", System.Data.SqlDbType.DateTime, 1000).Value = date;
                command.Parameters.Add("@Ingredients", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Ingredients;
                command.Parameters.Add("@Steps", System.Data.SqlDbType.NVarChar, 1000).Value = recipeModel.Steps;

                connection.Open();
                int newID = command.ExecuteNonQuery();
                connection.Close();

                return newID;
            }
        }
    }
}