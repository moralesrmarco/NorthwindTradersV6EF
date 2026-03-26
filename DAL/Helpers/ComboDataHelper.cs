using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Helpers
{
    public class ComboDataHelper
    {

        private readonly string _connectionString;

        public ComboDataHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable LlenarCbo(string storedProcedure, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(storedProcedure, cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);
                    da.Fill(dt);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al llenar el ComboBox: " + ex.Message);
            }
            return dt;
        }
    }
}
