namespace MVVM_tree_OrgViewer
{
    public sealed class Intern : Employee
    {
        public Intern(string name,
                      float MonthlySalary = Settings.FIXED_EMPLOYEE_SALARY) 
        : base(name, MonthlySalary, "Intern")
        {
        }

        public override float GetSalary(int days) => _salaryBase;
    }

}
