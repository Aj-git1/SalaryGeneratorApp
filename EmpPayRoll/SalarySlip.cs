namespace SalaryApp;

public class SalarySlip
{
    public string EmpName { get; set; } = "";
    public string Designation { get; set; } = "";
    public decimal BasicSalary { get; set; }
    public decimal HRA { get; set; }
    public decimal DA { get; set; }
    public decimal GrossEarnings { get; set; }
    public decimal PF { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
}
