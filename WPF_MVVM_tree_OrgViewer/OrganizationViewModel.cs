using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_tree_OrgViewer
{
    //БИЗНЕСС-ЛОГИКА
    public class OrganizationViewModel : INotifyPropertyChanged
    {
        private Department _selectedDepartment;

        private ObservableCollection<Department> _departments = new ObservableCollection<Department>();

        public OrganizationViewModel()
        {
            _departments = OrganizationStructureLoader.LoadFromJSON("org.json");
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged("selectedDepartment");
            }
        }

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged("Departments");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
