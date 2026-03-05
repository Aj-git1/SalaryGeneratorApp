using System.Data;
using Microsoft.Data.SqlClient;

namespace SalaryApp;

public class SalaryUtility
{
    public SalarySlip GenerateSalarySlip(Employee emp)
    {
        decimal hra = 0;
        decimal da = 0;
        decimal pf = 0;

        DatabaseHelper db = new DatabaseHelper();
        SqlConnection con = db.GetConnection();
        con.Open();

        SqlCommand cmd = new SqlCommand("select * from tblFormula", con);
        SqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            string componentName = dr["ComponentName"].ToString();
            string formula = dr["Formula"].ToString();

            string actualFormula = formula.Replace("BasicSalary", emp.BasicSalary.ToString());

            DataTable dt = new DataTable();
            decimal result = Convert.ToDecimal(dt.Compute(actualFormula, ""));

            if (componentName == "HRA") hra = result;
            if (componentName == "DA") da = result;
            if (componentName == "PF") pf = result;
        }

        con.Close();

        decimal grossSalary = emp.BasicSalary + hra + da;
        decimal netSalary = grossSalary - pf;

        SalarySlip slip = new SalarySlip();
        slip.EmpName = emp.EmpName;
        slip.BasicSalary = emp.BasicSalary;
        slip.HRA = hra;
        slip.DA = da;
        slip.GrossEarnings = grossSalary;
        slip.PF = pf;
        slip.NetSalary = netSalary;

        return slip;
    }

    public void SaveSalarySlip(int empId, SalarySlip slip)
    {
        DatabaseHelper db = new DatabaseHelper();
        SqlConnection con = db.GetConnection();
        con.Open();

        string query = "insert into tblSalarySlip values(@EmpId, @Basic, @HRA, @DA, @Gross, @PF, @Net)";

        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@EmpId", empId);
        cmd.Parameters.AddWithValue("@Basic", slip.BasicSalary);
        cmd.Parameters.AddWithValue("@HRA", slip.HRA);
        cmd.Parameters.AddWithValue("@DA", slip.DA);
        cmd.Parameters.AddWithValue("@Gross", slip.GrossEarnings);
        cmd.Parameters.AddWithValue("@PF", slip.PF);
        cmd.Parameters.AddWithValue("@Net", slip.NetSalary);
        cmd.ExecuteNonQuery();

        con.Close();
    }
}