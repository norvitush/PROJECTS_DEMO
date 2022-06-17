namespace MVVM_tree_OrgViewer
{

    public abstract class Employee
    {
        protected float _salaryBase;
        
        public Employee(string name, float salaryBase, string position)
        {
            _salaryBase = salaryBase;
            Name = name;
            Position = position;
        }

        public string Name { get; set; }
        public string Position { get; set; }
        public float Salary { get => GetSalary(30); }

        public abstract float GetSalary(int days);
    }

}
