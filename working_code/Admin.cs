using System.Data;
using Microsoft.Data.SqlClient;

namespace MBTP.Services
{
    public class AdministrationService
    {
        private readonly IConfiguration _configuration;

        public AdministrationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DataSet ReviewDistinctAlerts()
        {
            DataSet myDS = new DataSet();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (SqlCommand cmd = new SqlCommand("dbo.RetrieveDistinctAlerts", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter myDA = new SqlDataAdapter(cmd);
                    sqlConn.Open();
                    myDS.Clear();
                    myDA.Fill(myDS);
                    sqlConn.Close();
                }
                return myDS;
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine("SQL error: " + sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("General error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                throw;
            }
        }
        public DataSet RetrieveAddons()
        {
            DataSet myDS = new DataSet();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (SqlCommand cmd = new SqlCommand("dbo.RetrieveSpecialAddons", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter myDA = new SqlDataAdapter(cmd);
                    sqlConn.Open();
                    myDS.Clear();
                    myDA.Fill(myDS);
                    SqlDataAdapter myDA2 = new SqlDataAdapter(cmd);
                    cmd.CommandText = "dbo.RetrieveSpecialAddonGLcodes";
                    myDA2.Fill(myDS, "GLcodes");
                    sqlConn.Close();
                }
                return myDS;
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine("SQL error: " + sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("General error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                throw;
            }
        }
    }
}
