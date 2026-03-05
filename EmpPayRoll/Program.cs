using Microsoft.Data.SqlClient;
using SalaryApp;

Console.WriteLine("=========================================");
Console.WriteLine("   SALARY GENERATOR - Console App");
Console.WriteLine("=========================================");
Console.WriteLine();

bool running = true;

while (running)
{
    Console.WriteLine("MENU:");
    Console.WriteLine("  1. Add Employee");
    Console.WriteLine("  2. Generate Salary Slip");
    Console.WriteLine("  3. View All Employees");
    Console.WriteLine("  0. Exit");
    Console.WriteLine();
    Console.Write("Enter your choice: ");

    string choice = Console.ReadLine() ?? "";
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            AddEmployee();
            break;

        case "2":
            GenerateSalarySlip();
            break;

        case "3":
            ViewAllEmployees();
            break;

        case "0":
            running = false;
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }

    Console.WriteLine();
}

void AddEmployee()
{
    Console.WriteLine("--- Add New Employee ---");

    Console.Write("Enter Name       : ");
    string name = Console.ReadLine() ?? "";

    Console.Write("Enter Designation: ");
    string designation = Console.ReadLine() ?? "";

    Console.Write("Enter Basic Salary (Rs.): ");
    decimal basicSalary = Convert.ToDecimal(Console.ReadLine());
    DatabaseHelper db = new DatabaseHelper();
    using (SqlConnection conn = db.GetConnection())
    {
        conn.Open();

        string query = @"INSERT INTO Employees (EmpName, Designation, BasicSalary) 
                         VALUES (@Name, @Designation, @Basic)";

        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Designation", designation);
        cmd.Parameters.AddWithValue("@Basic", basicSalary);
        cmd.ExecuteNonQuery();
    }

    Console.WriteLine("Employee added successfully!");
}

void GenerateSalarySlip()
{
    Console.WriteLine("--- Generate Salary Slip ---");
    Console.Write("Enter Employee ID: ");
    int empId = Convert.ToInt32(Console.ReadLine());

    Employee? emp = null;

    DatabaseHelper db = new DatabaseHelper();
    using (SqlConnection conn = db.GetConnection())
    {
        conn.Open();

        string query = "SELECT * FROM Employees WHERE EmpId = @EmpId";
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@EmpId", empId);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            emp = new Employee
            {
                EmpId = (int)reader["EmpId"],
                EmpName = reader["EmpName"].ToString()!,
                Designation = reader["Designation"].ToString()!,
                BasicSalary = (decimal)reader["BasicSalary"]
            };
        }
    }

    if (emp == null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }

    SalaryUtility utility = new SalaryUtility();
    SalarySlip slip = utility.GenerateSalarySlip(emp);

    PrintSalarySlip(slip);

    utility.SaveSalarySlip(emp.EmpId, slip);
    Console.WriteLine("Salary slip saved to database.");
}

void ViewAllEmployees()
{
    Console.WriteLine("--- All Employees ---");

    DatabaseHelper db = new DatabaseHelper();
    using (SqlConnection conn = db.GetConnection())
    {
        conn.Open();

        string query = "SELECT * FROM Employees";
        SqlCommand cmd = new SqlCommand(query, conn);
        SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine($"{"ID",-6} {"Name",-20} {"Designation",-20} {"Basic Salary",12}");
        Console.WriteLine(new string('-', 62));

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader["EmpId"],-6} " +
                $"{reader["EmpName"],-20} " +
                $"{reader["Designation"],-20} " +
                $"Rs. {reader["BasicSalary"],8}"
            );
        }
    }
}

void PrintSalarySlip(SalarySlip slip)
{
    Console.WriteLine();
    Console.WriteLine("==========================================");
    Console.WriteLine("            SALARY SLIP");
    Console.WriteLine("==========================================");
    Console.WriteLine($"  Employee   : {slip.EmpName}");
    Console.WriteLine($"  Designation: {slip.Designation}");
    Console.WriteLine("------------------------------------------");
    Console.WriteLine("  EARNINGS");
    Console.WriteLine($"  {"Basic Salary",-25} Rs. {slip.BasicSalary,8:N2}");
    Console.WriteLine($"  {"HRA (40% of Basic)",-25} Rs. {slip.HRA,8:N2}");
    Console.WriteLine($"  {"DA  (20% of Basic)",-25} Rs. {slip.DA,8:N2}");
    Console.WriteLine("------------------------------------------");
    Console.WriteLine($"  {"Gross Earnings",-25} Rs. {slip.GrossEarnings,8:N2}");
    Console.WriteLine("------------------------------------------");
    Console.WriteLine("  DEDUCTIONS");
    Console.WriteLine($"  {"PF  (12% of Basic)",-25} Rs. {slip.PF,8:N2}");
    Console.WriteLine("------------------------------------------");
    Console.WriteLine($"  {"Total Deductions",-25} Rs. {slip.TotalDeductions,8:N2}");
    Console.WriteLine("==========================================");
    Console.WriteLine($"  {"NET SALARY",-25} Rs. {slip.NetSalary,8:N2}");
    Console.WriteLine("==========================================");
    Console.WriteLine();
}
