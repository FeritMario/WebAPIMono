using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class RestaurantController : ApiController
    {
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post([FromBody] Restaurant restaurant)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=ijasamzakon24;Database=restoran";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);


            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "INSERT INTO restaurant values(@Id,@Name,@Rate)";
                command.Connection = connection;
                connection.CreateCommand();
                connection.Open();
                

                Guid id = Guid.NewGuid();
                restaurant.Id = id;
                command.Parameters.AddWithValue("@Id", restaurant.Id);

                command.Parameters.AddWithValue("@FirstName", restaurant.Name)
                    ;
                command.Parameters.AddWithValue("@LastName", restaurant.Rate);

                command.ExecuteNonQuery();
            }


            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get()
        {
            List<Restaurant> restaurants = new List<Restaurant>();

            string connectionString = "Host=localhost;Username=postgres;Password=ijasamzakon24;Database=restoran";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "select * from \"restoran\"";
                command.Connection = connection;
                connection.Open();

                NpgsqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No elements exist");
                }
                while (reader.Read())
                {
                    Restaurant restaurant = new Restaurant();

                    restaurant.Id = (Guid)reader["Id"];
                    restaurant.Name = (string)reader["Name"];
                    restaurant.Rate = (int)reader["Rate"];

                    restaurants.Add(restaurant);
                }
                return Request.CreateResponse(HttpStatusCode.OK, restaurants);
            }
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(Guid id)
        {

            try
            {
                Restaurant restaurant = GetId(id);

                if (restaurant == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Restaurant does not exist!");
                }
                return Request.CreateResponse(HttpStatusCode.OK, restaurant);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put(Guid id, [FromBody] Restaurant restaurant)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=ijasamzakon24;Database=restoran";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            Restaurant rest = GetId(id);

            if (rest == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This user does not exist");
            }
            try
            {
                using (connection)
                {
                    var queryBuilder = new StringBuilder("");
                    NpgsqlCommand command = new NpgsqlCommand();
                    queryBuilder.Append("UPDATE Customer SET ");
                    command.Connection = connection;
                    connection.Open();

                    if (restaurant.Name == null || restaurant.Name == "")
                    {
                        command.Parameters.AddWithValue("@Name", restaurant.Name = rest.Name);
                    }
                    queryBuilder.Append(" \"Name\" = @Name,");
                    command.Parameters.AddWithValue("@Rate", restaurant.Rate);
                    if (restaurant.Rate >0)
                    {
                        command.Parameters.AddWithValue("@Rate", restaurant.Rate = rest.Rate);
                    }
                    queryBuilder.Append(" \"Rate\" = @Rate,");
                    command.Parameters.AddWithValue("@Rate", restaurant.Rate);

                    if (queryBuilder.ToString().EndsWith(","))
                    {
                        if (queryBuilder.Length > 0)
                        {
                            queryBuilder.Remove(queryBuilder.Length - 1, 1);
                        }
                    }

                    queryBuilder.Append(" WHERE \"Id\" = @Id");
                    command.Parameters.AddWithValue("@Id", id);
                    command.CommandText = queryBuilder.ToString();
                    command.ExecuteNonQuery();
                    return Request.CreateResponse(HttpStatusCode.OK, "User updated successfuly!");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=mojabaza123;Database=rent-a-car";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                Restaurant customer = GetId(id);

                if (customer == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This user does not exist");
                }
                using (connection)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.CommandText = "DELETE FROM customer WHERE \"Id\"=@Id";
                    command.Connection = connection;
                    connection.Open();

                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                    return Request.CreateResponse(HttpStatusCode.OK, "User deleted successfuly!");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Couldn't delete!" + ex.Message);
            }
        }

        private Restaurant GetId(Guid id)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=mojabaza123;Database=rent-a-car";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "select * from \"Customer\" where \"Id\"=@Id";
                command.Connection = connection;
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Restaurant restaurant = new Restaurant();
                    restaurant.Id = id;
                    restaurant.Name = (string)reader["Name"];
                    restaurant.Rate = (int)reader["Rate"];
                    return restaurant;
                }
                return null;
            }
        }
    }


}
