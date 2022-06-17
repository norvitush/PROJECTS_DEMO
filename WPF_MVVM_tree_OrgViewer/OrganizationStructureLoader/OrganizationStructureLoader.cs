using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MVVM_tree_OrgViewer
{
    public static class OrganizationStructureLoader
    {
        public static ObservableCollection<Department> LoadFromJSON(string path)
        {
            ObservableCollection<Department> OrganizationStruct = new ObservableCollection<Department>();

            if (File.Exists(path))
            {
                // чтение данных
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    UnitLoadTemplate[] FlatOrganizationStruct = JsonConvert.DeserializeObject<UnitLoadTemplate[]>(json);
                    ObservableCollection<UnitLoadTemplate> zeroLvl = new ObservableCollection<UnitLoadTemplate>(
                        FlatOrganizationStruct.Where(d => d.ParentId == 0)
                    );
                    foreach (var dep in zeroLvl)
                    {
                        Department currentLvl = new Department(dep.Title,null,dep.Id);
                        FillFromFlatList(FlatOrganizationStruct, currentLvl);
                        OrganizationStruct.Add(currentLvl);
                    }
                }
            }

            return OrganizationStruct;
        }
        private static void FillFromFlatList(IEnumerable<UnitLoadTemplate> flatList, Department targetDepartment)
        {
            IEnumerable<Department> includedDepartments = flatList
                .Where(d => d.ParentId == targetDepartment.Id && d.Type == OrganizationUnitType.Department.ToString())
                .Select(d=> new Department(d.Title,targetDepartment,d.Id));
            IEnumerable<UnitLoadTemplate> includedEmployees = flatList.Where(d => d.ParentId == targetDepartment.Id && d.Type != OrganizationUnitType.Department.ToString());
            foreach (var employee in includedEmployees)
            {
                switch (Enum.Parse(typeof(OrganizationUnitType), employee.Type))
                {
                    case OrganizationUnitType.Manager:
                        string position = "Department Manager";
                        float minSalary = Settings.MANAGER_MIN_SALARY;
                        if (targetDepartment.Parent==null)
                        {
                            position = "CEO";
                            minSalary = Settings.BOSS_MIN_SALARY;
                        }
                        targetDepartment.HireEmployee(OrganizationUnitType.Manager, employee.Title, position, minSalary);
                        break;
                    case OrganizationUnitType.Worker:
                        targetDepartment.HireEmployee(OrganizationUnitType.Worker, employee.Title, "Engineer", Settings.HOURLY_EMPLOYEE_SALARY);
                        break;
                    case OrganizationUnitType.Intern:
                        targetDepartment.HireEmployee(OrganizationUnitType.Intern, employee.Title, "Intern", Settings.FIXED_EMPLOYEE_SALARY);
                        break;
                    
                }
            }
            //рекурсивное заполнение дерева
            foreach (var subDep in includedDepartments)
            {
                targetDepartment.SubDepartments.Add(subDep);
                FillFromFlatList(flatList, subDep);
            }

        }
    }
}
