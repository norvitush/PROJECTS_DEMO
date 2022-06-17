namespace MVVM_tree_OrgViewer
{
    public sealed class Manager : Employee
    {
        private  Department _workplace;

        public Manager(string name,
                       Department workplace,
                       float salaryMinLimit = Settings.MANAGER_MIN_SALARY, string position = "Department manager")
            : base(name, salaryMinLimit, position)
        {
            _workplace = workplace;
        }

        public override float GetSalary(int days) => System.Math.Max(_workplace.GetSalary(days)*Settings.MANAGER_PROCENT/100,_salaryBase);
    }

}
