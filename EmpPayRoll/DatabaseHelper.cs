using Microsoft.Data.SqlClient;

namespace SalaryApp;

public class DatabaseHelper
{
    private static string connectionString =
        "Server=LAPTOP-OFTMO036;Database=SalaryDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }
}
