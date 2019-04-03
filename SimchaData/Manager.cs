using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaData
{
    public class Manager
    {
        private string _connectionString;

        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Simcha> GetAllSimchas()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Simchas";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Simcha> simchas = new List<Simcha>();
            while (reader.Read())
            {
                simchas.Add(new Simcha
                {
                    Id = (int)reader["id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                });
            }
            connection.Close();
            connection.Dispose();
            return simchas;
        }

        public IEnumerable<Contributor> GetAllContributors()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contributor> contributors = new List<Contributor>();
            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                });
            }
            connection.Close();
            connection.Dispose();
            return contributors;
        }

        public IEnumerable<Diposit> GetDipositsById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Diposits WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Diposit> diposits = new List<Diposit>();
            while (reader.Read())
            {
                diposits.Add(new Diposit
                {
                    Id = (int)reader["id"],
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"],
                    ContributorId = (int)reader["ContributorId"]
                });
            }
            connection.Close();
            connection.Dispose();
            return diposits;
        }

        public IEnumerable<Contribution> GetAllContributions()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributions";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contribution> contributions = new List<Contribution>();
            while (reader.Read())
            {
                contributions.Add(new Contribution
                {
                    SimchaId = (int)reader["SimchaId"],
                    ContributorId = (int)reader["ContributorId"],
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"]
                });
            }
            connection.Close();
            connection.Dispose();
            return contributions;
        }

        public decimal GetBalance(int contributorId)
        {
            IEnumerable<History> histories = GetHistory(contributorId);
            return histories.Sum(b => b.Amount);
        }

        public IEnumerable<History> GetHistory(int ContributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Diposits WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", ContributorId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<History> history = new List<History>();
            while (reader.Read())
            {
                history.Add(new History
                {
                    Action = "Diposit",
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"]
                });
            }

            history.AddRange(addContributes(ContributorId));

            connection.Close();
            connection.Dispose();
            return history.OrderByDescending(h => h.Date);
        }

        private IEnumerable<History> addContributes(int ContributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributions WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", ContributorId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<History> histories = new List<History>();
            while (reader.Read())
            {
                histories.Add(new History
                {
                    Action = $"Contribution for {GetSimchaNameById((int)reader["SimchaId"])}",
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"]
                });
            }
            connection.Close();
            connection.Dispose();
            return histories;
        }

        public string GetSimchaNameById(int SimchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SimchaName FROM Simchas WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", SimchaId);
            connection.Open();
            
            string name = (string)cmd.ExecuteScalar();
            connection.Close();
            connection.Dispose();
            return name;
        }

        public string GetContributorNameById(int ContributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT FirstName, LastName FROM Contributors WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", ContributorId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            string name = (string)reader["FirstName"] + " " + (string)reader["LastName"];
            connection.Close();
            connection.Dispose();
            return name;
        }

        public void InsertSimcha(Simcha simcha)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas VALUES (@simchaName, @date)";
            cmd.Parameters.AddWithValue("@simchaName", simcha.SimchaName);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        public void InsertContributor(Contributor contributor)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors VALUES (@firstName, @lastName, @cellNumber, @date, @alwaysInclude) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            contributor.Id = (int)(decimal)cmd.ExecuteScalar(); 
            connection.Close();
            connection.Dispose();
        }

        public void UpdateContributor(Contributor contributor)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, CellNumber  = @cellNumber,
                               Date = @date, AlwaysInclude = @alwaysInclude WHERE id = @id";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            cmd.Parameters.AddWithValue("@id", contributor.Id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        public void InsertContribution(Contribution contribution)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas VALUES (@simchaId, @contributorId, @amount @date)";
            cmd.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
            cmd.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
            cmd.Parameters.AddWithValue("@amount", contribution.Amount);
            cmd.Parameters.AddWithValue("@date", contribution.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        public void InsertDiposit(Diposit diposit)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Diposits VALUES (@date, @amount, @contributorId)";
            cmd.Parameters.AddWithValue("@date", diposit.Date);
            cmd.Parameters.AddWithValue("@amount", diposit.Amount);
            cmd.Parameters.AddWithValue("@contributorId", diposit.ContributorId);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }
    }
}
