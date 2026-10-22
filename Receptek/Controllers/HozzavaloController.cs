using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Receptek.Models;
using Receptek.Models.DTOs;

namespace Receptek.Controllers
{
    [Route("hozzavalo")]
    [ApiController]
    public class HozzavaloController : ControllerBase
    {
        string connectionstring =
            "Server=localhost;Database=receptek;User=root;Password=;";

        [HttpGet("getAllHozzavalo")]
        public object GetAllHozzavalo()
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = "SELECT * FROM hozzavalo";

            using var cmd = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            var hozzavalok = new List<Hozzavalo>();

            while (reader.Read())
            {
                var hozzavalo = new Hozzavalo
                {
                    Id = reader.GetInt32("id"),
                    IngredientName = reader.GetString("ingredientName"),
                    Quantity = reader.GetString("quantity"),
                    Description = reader.GetString("description"),
                    AddedTime = reader.GetDateTime("addedTime"),
                    UpdateTime = reader.GetDateTime("updateTime"),
                    SzakacsId = reader.GetInt32("szakacsId")
                };

                hozzavalok.Add(hozzavalo);
            }

            return StatusCode(200, new {message = "Sikeres lekérdezés.", result = hozzavalok });
        }

        [HttpGet("getHozzavaloById/{id}")]
        public object GetHozzavaloById(int id)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = "SELECT * FROM hozzavalo WHERE id = @id";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var hozzavalo = new Hozzavalo
                {
                    Id = reader.GetInt32("id"),
                    IngredientName = reader.GetString("ingredientName"),
                    Quantity = reader.GetString("quantity"),
                    Description = reader.GetString("description"),
                    AddedTime = reader.GetDateTime("addedTime"),
                    UpdateTime = reader.GetDateTime("updateTime"),
                    SzakacsId = reader.GetInt32("szakacsId")
                };

                return StatusCode(200, new
                {
                    message = "Sikeres lekérdezés.",
                    result = hozzavalo
                });
            }
            else
            {
                return StatusCode(404, new { message = "Nincs ilyen hozzávaló." });
            }
        }

        [HttpPost("addNewHozzavalo")]
        public object AddNewHozzavalo(HozzavaloDTO newHozzavalo)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = "INSERT INTO hozzavalo (ingredientName, quantity, description, addedTime, updateTime, szakacsId) VALUES (@nev, @mennyiseg, @leiras, NOW(), NOW(), @szakacsId)";

            using var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@nev", newHozzavalo.IngredientName);
            cmd.Parameters.AddWithValue("@mennyiseg", newHozzavalo.Quantity);
            cmd.Parameters.AddWithValue("@leiras", newHozzavalo.Description);
            cmd.Parameters.AddWithValue("@szakacsId", newHozzavalo.SzakacsId);

            int darab = cmd.ExecuteNonQuery();

            if (darab > 0)
            {
                return StatusCode(201, new { message = "Sikeres hozzáadás.", result = new { Id = cmd.LastInsertedId }});
            }
            else
            { return StatusCode(400, new { message = "Sikertelen hozzáadás." });
            }
        }

        [HttpPut("updateHozzavalo/{id}")]
        public object UpdateHozzavalo(int id, HozzavaloDTO updatedHozzavalo)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = @"UPDATE hozzavalo SET
                ingredientName = @nev,
                quantity = @mennyiseg,
                description = @leiras,
                updateTime = NOW(),
                szakacsId = @szakacsId
                WHERE id = @id";

            using var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@nev", updatedHozzavalo.IngredientName);
            cmd.Parameters.AddWithValue("@mennyiseg", updatedHozzavalo.Quantity);
            cmd.Parameters.AddWithValue("@leiras", updatedHozzavalo.Description);
            cmd.Parameters.AddWithValue("@szakacsId", updatedHozzavalo.SzakacsId);
            cmd.Parameters.AddWithValue("@id", id);

            int darab = cmd.ExecuteNonQuery();

            if (darab > 0)
            {
                return StatusCode(200, new { message = "Sikeres frissítés.", result = new { Id = id }});
            }
            else
            {
                return StatusCode(404, new { message = "Nincs ilyen hozzávaló." });
            }
        }

        [HttpDelete("deleteHozzavalo/{id}")]
        public object DeleteHozzavalo(int id)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = "DELETE FROM hozzavalo WHERE id = @id";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            int darab = cmd.ExecuteNonQuery();

            if (darab > 0)
            {
                return StatusCode(204);
            }
            else
            {
                return StatusCode(404, new { message = "Nincs ilyen hozzávaló." });
            }
        }

        [HttpGet("getSzakacsById/{id}")]
        public object GetSzakacsById(int id)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = "SELECT name, email FROM szakacs WHERE id = @id";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var szakacs = new
                {
                    Name = reader.GetString("name"),
                    Email = reader.GetString("email")
                };

                return StatusCode(200, new { message = "Sikeres lekérdezés.", result = szakacs });
            }
            else
            {
                return StatusCode(404, new { message = "Nincs ilyen szakács." });
            }
        }

        [HttpGet("getSzakacsWithHozzavalok/{id}")]
        public object GetSzakacsWithHozzavalok(int id)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = "SELECT szakacs.name, hozzavalo.ingredientName, hozzavalo.quantity FROM szakacs, hozzavalo WHERE szakacs.id = hozzavalo.szakacsId AND szakacs.id = @id";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                return StatusCode(404, new { message = "Nincs ilyen szakács." });
            }

            string nev = "";
            List<object> hozzavalok = new List<object>();

            while (reader.Read())
            {
                nev = reader.GetString("name");

                if (reader["ingredientName"] != DBNull.Value)
                {
                    hozzavalok.Add(new
                    {
                        IngredientName = reader.GetString("ingredientName"),
                        Quantity = reader.GetString("quantity")
                    });
                }
            }

            return StatusCode(200, new{message = "Sikeres lekérdezés.", result = new {
                    Name = nev,
                    Hozzavalok = hozzavalok
                }
            });
        }

        [HttpGet("getHozzavaloCount")]
        public object GetHozzavaloCount()
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();
                
            string sql = "SELECT COUNT(*) FROM hozzavalo";

            using var cmd = new MySqlCommand(sql, connection);

            int darab = Convert.ToInt32(cmd.ExecuteScalar());

            return StatusCode(200, new { message = "Sikeres lekérdezés.", result = darab });
        }

        [HttpGet("getHozzavaloCountBySzakacs/{id}")]
        public object GetHozzavaloCountBySzakacs(int id)
        {
            using var connection = new MySqlConnection(connectionstring);
            connection.Open();

            string sql = @"SELECT COUNT(*) FROM hozzavalo
                WHERE szakacsId = @id";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            int darab = Convert.ToInt32(cmd.ExecuteScalar());

            return StatusCode(200, new { message = "Sikeres lekérdezés.", result = darab });
        }
    }
}