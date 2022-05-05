using RecipeCRUD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

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
                        recipe.Name = reader.GetString(1);
                        recipe.Description = reader.GetString(2);

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

            // Access database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM dbo.Recipes WHERE NAME LIKE @searchForMe";


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
                        recipe.Name = reader.GetString(1);
                        recipe.Description = reader.GetString(2);

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
                        recipe.Name = reader.GetString(1);
                        recipe.Description = reader.GetString(2);

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
                        recipe.Name = reader.GetString(1);
                        recipe.Description = reader.GetString(2);
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
                Console.WriteLine(recipeModel.Id);
                if (recipeModel.Id <= 0)
                {
                    // create
                    sqlQuery = "INSERT INTO dbo.Recipes VALUES(@Name, @Description)";
                } else
                {
                    // update
                    sqlQuery = "UPDATE dbo.Recipes SET Name = @Name, Description = @Description WHERE Id = @Id";
                }
                    
                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.Add("@Id", System.Data.SqlDbType.VarChar, 1000).Value = recipeModel.Id;
                command.Parameters.Add("@Name", System.Data.SqlDbType.VarChar, 1000).Value = recipeModel.Name;
                command.Parameters.Add("@Description", System.Data.SqlDbType.VarChar, 1000).Value = recipeModel.Description;

                connection.Open();
                int newID = command.ExecuteNonQuery();
                connection.Close();

                return newID;
            }
        }
    }
}