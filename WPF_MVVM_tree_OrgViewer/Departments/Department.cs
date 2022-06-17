using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM_tree_OrgViewer
{
    //МОДЕЛЬ ДЕПАРТАМЕНТА
    public class Department : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private Department _parent;
        private Manager _manager;
        private ObservableCollection<Department> _subDepartments = new ObservableCollection<Department>();
        private ObservableCollection<Employee> _employees = new ObservableCollection<Employee>();

        public Department(string title, Department parent, int id)
        {
            _title = title;
            _parent = parent;
            _id = id;
        }

        public int SubDepartmentsCount => _subDepartments.Count;
        public int EmployeesCount => _employees.Count;

        public ObservableCollection<Department> SubDepartments
        {
            get => _subDepartments;
            set
            {
                _subDepartments = value;
                OnPropertyChanged("SubDepartments");
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
        public Department Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        public Manager Manager
        {
            get { return _manager; }
            set
            {
                _manager = value;
                OnPropertyChanged("Manager");
            }
        }

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set 
            {
                _employees = value; OnPropertyChanged("Employees");
            }
        }



        public float GetSalary(int days)
        {
            float summary = 0f;
            foreach (var employeer in Employees)
            {
                summary += employeer.GetSalary(days);
            }
            foreach (var dep in SubDepartments)
            {
                float DepartmentEmployeesSalary = dep.GetSalary(days);
                summary += DepartmentEmployeesSalary;
                if (dep.Manager!=null)
                {
                    float ManagerSalary = DepartmentEmployeesSalary * Settings.MANAGER_PROCENT/100;
                    summary += ManagerSalary;
                }
                
            }

            return summary;
        }

        public void HireEmployee(OrganizationUnitType candidateType, string name, string wantedPosition, float wantedSalary)
        {
            if (candidateType == OrganizationUnitType.Manager)
            {
                Manager = new Manager(name, this, salaryMinLimit: wantedSalary, wantedPosition);
            }
            else if (candidateType == OrganizationUnitType.Worker)
            {
                Employees.Add(new Worker(name, wantedPosition, wantedSalary));
            }
            else if (candidateType == OrganizationUnitType.Intern)
            {
                Employees.Add(new Intern(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
