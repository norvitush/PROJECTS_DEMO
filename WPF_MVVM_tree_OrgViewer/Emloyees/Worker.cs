namespace MVVM_tree_OrgViewer
{
    public sealed class Worker : Employee
    {
        public Worker(string name,
                      string position = "Engineer",
                      float hourlySalary = Settings.HOURLY_EMPLOYEE_SALARY
                      )
        : base(name, hourlySalary, position)
        {
        }
        
        public override float GetSalary(int days) => _salaryBase * 8 * days;
    }

}
