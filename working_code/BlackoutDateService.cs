using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MBTP.Models;

namespace MBTP.Retrieval
{
    public class BlackoutService
    {
        private readonly IConfiguration _configuration;

        public BlackoutService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<BlackoutDate> GetAll()
        {
            var list = new List<BlackoutDate>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(@"SELECT b.BlackoutID, b.PCID, p.Description AS ProfitCenterName, b.StartDate, b.EndDate, b.Reason
                                            FROM BlackoutDates b
                                            INNER JOIN ProfitCenters p on b.PCID = p.PCID
                                            ORDER BY b.StartDate", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BlackoutDate
                {
                    BlackoutID = (int)reader["BlackoutID"],
                    PCID = (int)reader["PCID"],
                    ProfitCenterName = reader["ProfitCenterName"].ToString(),
                    StartDate = (DateTime)reader["StartDate"],
                    EndDate = (DateTime)reader["EndDate"],
                    Reason = reader["Reason"]?.ToString()
                });
            }
            return list;
        }
        
        
        public BlackoutDate GetById(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT * FROM BlackoutDates WHERE BlackoutID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new BlackoutDate
                {
                    BlackoutID = (int)reader["BlackoutID"],
                    PCID = (int)reader["PCID"],
                    StartDate = (DateTime)reader["StartDate"],
                    EndDate = (DateTime)reader["EndDate"],
                    Reason = reader["Reason"]?.ToString()
                };
            }
            return null;
        }
        

        public void Add(BlackoutDate blackout)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(@"
                INSERT INTO BlackoutDates (PCID, StartDate, EndDate, Reason) 
                VALUES (@PCID, @StartDate, @EndDate, @Reason)", conn);

            cmd.Parameters.AddWithValue("@PCID", blackout.PCID);
            cmd.Parameters.AddWithValue("@StartDate", blackout.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", blackout.EndDate);
            cmd.Parameters.AddWithValue("@Reason", blackout.Reason ?? "");

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            Console.WriteLine(result);
        }

        
        public void Update(BlackoutDate blackout)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(@"
                                            UPDATE BlackoutDates 
                                            SET StartDate = @StartDate, 
                                            EndDate = @EndDate, 
                                            Reason = @Reason 
                                            WHERE BlackoutID = @BlackoutID", conn);
            cmd.Parameters.AddWithValue("@BlackoutID", blackout.BlackoutID);
            cmd.Parameters.AddWithValue("@StartDate", blackout.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", blackout.EndDate);
            cmd.Parameters.AddWithValue("@Reason", blackout.Reason ?? "");
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        
        public void Delete(BlackoutDate blackout)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("DELETE FROM BlackoutDates WHERE BlackoutID = @BlackoutID", conn);
            cmd.Parameters.AddWithValue("@BlackoutID", blackout.BlackoutID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool HasOverlap(int PCID, DateTime startDate, DateTime endDate, int? excludeId = null)
        {
            string sql = @"
                SELECT COUNT(*) FROM BlackoutDates 
                WHERE PCID = @PCID
                AND StartDate <= @EndDate 
                AND EndDate >= @StartDate";

            if (excludeId.HasValue)
                sql += " AND BlackoutID != @BlackoutID";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PCID", PCID);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            if (excludeId.HasValue) cmd.Parameters.AddWithValue("@BlackoutID", excludeId);

            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool IsBlackout(int PCID, DateTime date)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM BlackoutDates
                WHERE PCID = @PCID
                AND StartDate <= @Date AND EndDate >= @Date", conn);

            cmd.Parameters.AddWithValue("@PCID", PCID);
            cmd.Parameters.AddWithValue("@Date", date.Date); 

            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

    }

}