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

        public IEnumerable<SimchaView> GetAllSimchas()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.id, s.SimchaName, COUNT(c.ContributorId) AS Count, sum(ISNULL(c.Amount, 0)) AS Total, s.Date 
                                FROM Simchas s LEFT JOIN Contributions c ON s.id = c.SimchaId 
	                            GROUP BY s.id, s.SimchaName, s.Date";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<SimchaView> simchas = new List<SimchaView>();
            while (reader.Read())
            {
                simchas.Add(new SimchaView
                {
                    Id = (int)reader["id"],
                    SimchaName = (string)reader["SimchaName"],
                    Count = (int)reader["Count"],
                    Total = (decimal)reader["Total"],
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

        public decimal GetTotal()
        {
            decimal total = 0;
            IEnumerable<Contributor> contributors = GetAllContributors();
            foreach (Contributor c in contributors)
            {
                total += GetBalance(c.Id);
            }
            return total;
        }

        public IEnumerable<Diposit> GetDipositsById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Diposits WHERE ContributorId = @id";
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

        public IEnumerable<ContributionView> GetAllContributions(int SimchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<ContributionView> contributions = new List<ContributionView>();
            while (reader.Read())
            {
                contributions.Add(new ContributionView
                {
                    ContributorId = (int)reader["Id"],
                    Contribute = IsIn(SimchaId, (int)reader["Id"]).HasValue,
                    Name = (string)reader["FirstName"] + " " + (string)reader["LastName"],
                    Balance = GetBalance((int)reader["Id"]),
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Amount = IsIn(SimchaId, (int)reader["Id"]) ?? 5
                });
            }
            connection.Close();
            connection.Dispose();
            return contributions;
        }

        private decimal? IsIn(int SimchaId, int ContributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT ContributorId, Amount FROM Contributions WHERE SimchaId = @sid AND ContributorId = @cid";
            cmd.Parameters.AddWithValue("@sid", SimchaId);
            cmd.Parameters.AddWithValue("@cid", ContributorId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            decimal amount = (decimal)reader["Amount"];
            connection.Close();
            connection.Dispose();
            return amount;
        }
       
        public decimal GetBalance(int contributorId)
        {
            IEnumerable<History> contributes = addContributes(contributorId);
            IEnumerable<Diposit> diposits = GetDipositsById(contributorId);
            return diposits.Sum(b => b.Amount) - contributes.Sum(b => b.Amount);
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

        public int GetContributorCount()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Contributors";
            connection.Open();

            int count = (int)cmd.ExecuteScalar();
            connection.Close();
            connection.Dispose();
            return count;
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

        public void InsertContribution(List<Contribution> contributions)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributions VALUES (@simchaId, @contributorId, @amount, @date)";
            connection.Open();

            foreach (Contribution c in contributions)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@simchaId", c.SimchaId);
                cmd.Parameters.AddWithValue("@contributorId", c.ContributorId);
                cmd.Parameters.AddWithValue("@amount", c.Amount);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }

            connection.Close();
            connection.Dispose();
        }

        public void DeleteContributions(int SimchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Contributions WHERE SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", SimchaId);
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
