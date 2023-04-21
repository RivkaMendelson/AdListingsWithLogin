using System.Data.SqlClient;

namespace AdWithLogin_data
{
    public class Database
    {
        private string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }


        public Person Login(string email, string password)
        {
            var person = GetByEmail(email);
            if (person == null)
            {
                return null;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, person.Password);
            if (!isValid)
            {
                return null;
            }

            return person;

        }

        public void AddPerson(Person person)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO People (Name, PhoneNumber, Email, Password) VALUES " +
                "(@name, @phone, @email, @password)";

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(person.Password);
            cmd.Parameters.AddWithValue("@name", person.Name);
            cmd.Parameters.AddWithValue("@phone", person.PhoneNumber);
            cmd.Parameters.AddWithValue("@email", person.Email);
            cmd.Parameters.AddWithValue("@password", passwordHash);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void NewAd(Ad ad, int personId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads (Title, Details, DateCreated, PersonId) VALUES " +
                "(@title, @details, @date, @personId)";

            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@details", ad.Details);
            cmd.Parameters.AddWithValue("@date", ad.DateCreated);
            cmd.Parameters.AddWithValue("@personId", personId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }


        public Person GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM People WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Person
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                Password = (string)reader["Password"],
            };
        }

        public List<AdWithPersonInfo> GetAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads a JOIN People p ON a.personId=p.Id";
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<AdWithPersonInfo> ads = new List<AdWithPersonInfo>();

            while(reader.Read())
            {
                ads.Add(new AdWithPersonInfo
                {
                    Id = (int)reader["id"],
                    Title = (string)reader["Title"],
                    Details = (string)reader["Details"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"]

                });

            }
            return ads;

        }

        public List<int> GetUserIds(int PersonId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads WHERE PersonId = @id";
            cmd.Parameters.AddWithValue("@id", PersonId);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<int> ids = new List<int>();

            while (reader.Read())
            {
                ids.Add ((int)reader["PersonId"]);

            }
            return ids;
        }
        public void Delete(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id and PersonId=@userId";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@userId", userId);

            connection.Open();

            cmd.ExecuteNonQuery();

        }
    }
}