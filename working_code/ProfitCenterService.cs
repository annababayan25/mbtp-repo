using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MBTP.Models;

namespace MBTP.Retrieval
{
    public class ProfitCenterService
    {
        private readonly IConfiguration _configuration;

        public ProfitCenterService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ProfitCenters> GetAllLocations()
        {
            var locations = new List<ProfitCenters>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT PCID, Description AS ProfitCenterName FROM ProfitCenters", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                locations.Add(new ProfitCenters
                {
                    PCID = (int)reader["PCID"],
                    Description = reader["ProfitCenterName"].ToString()
                });
            }
            return locations;
        }
    }
}
