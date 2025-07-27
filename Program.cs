
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ITI_Lap_9
{
    class Program
    {
        static void Main()
        {
            var options = new DbContextOptionsBuilder<CompanyDBContext>()
                        .UseSqlite("Data Source=CompanyDB.db")
                        .Options;

            using var context = new CompanyDBContext(options);
            context.Database.EnsureCreated();

            while (true)
            {
                Console.Clear();
                DisplayMainMenu();

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1": AddDepartment(context); break;
                    case "2": EditDepartment(context); break;
                    case "3": DeleteDepartment(context); break;
                    case "4": ViewDepartments(context); break;
                    case "5": AddEmployee(context); break;
                    case "6": EditEmployee(context); break;
                    case "7": DeleteEmployee(context); break;
                    case "8": ViewEmployees(context); break;
                    case "9": AddProject(context); break;
                    case "10": EditProject(context); break;
                    case "11": DeleteProject(context); break;
                    case "12": ViewProjects(context); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        static void DisplayMainMenu()
        {
            
            Console.WriteLine("1.  Add Department");
            Console.WriteLine("2.  Edit Department");
            Console.WriteLine("3.  Delete Department");
            Console.WriteLine("4.  View Departments");
            Console.WriteLine("5.  Add Employee");
            Console.WriteLine("6.  Edit Employee");
            Console.WriteLine("7.  Delete Employee");
            Console.WriteLine("8.  View Employees");
            Console.WriteLine("9.  Add Project");
            Console.WriteLine("10. Edit Project");
            Console.WriteLine("11. Delete Project");
            Console.WriteLine("12. View Projects");
            Console.WriteLine("0.  Exit");
            Console.Write("Enter your choice: ");
        }

        static void AddDepartment(CompanyDBContext context)
        {
            Console.Write("Enter Department Name: ");
            var name = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name cannot be empty.");
                Pause();
                return;
            }

            context.Departments.Add(new Department { Name = name });
            context.SaveChanges();
            Console.WriteLine("Department added successfully!");
            Pause();
        }

        static void EditDepartment(CompanyDBContext context)
        {
            var departments = context.Departments.ToList();
            if (!departments.Any())
            {
                Console.WriteLine("No departments available.");
                Pause();
                return;
            }

            DisplayItems(departments, "Departments");
            Console.Write("Select department to edit: ");

            if (!TryGetSelection(departments.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var department = departments[selection - 1];
            Console.Write($"Enter new name ({department.Name}): ");
            var newName = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(newName))
            {
                Console.WriteLine("Name cannot be empty.");
                Pause();
                return;
            }

            department.Name = newName;
            context.SaveChanges();
            Console.WriteLine("Department updated successfully!");
            Pause();
        }

        static void DeleteDepartment(CompanyDBContext context)
        {
            var departments = context.Departments.ToList();
            if (!departments.Any())
            {
                Console.WriteLine("No departments available.");
                Pause();
                return;
            }

            DisplayItems(departments, "Departments");
            Console.Write("Select department to delete: ");

            if (!TryGetSelection(departments.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var department = departments[selection - 1];
            context.Departments.Remove(department);
            context.SaveChanges();
            Console.WriteLine("Department deleted successfully!");
            Pause();
        }

        static void ViewDepartments(CompanyDBContext context)
        {
            var departments = context.Departments.Include(d => d.Employees).ToList();

            if (!departments.Any())
            {
                Console.WriteLine("No departments available.");
            }
            else
            {
                Console.WriteLine("\nDepartment List:");
                foreach (var dept in departments)
                {
                    Console.WriteLine($"ID: {dept.DepartmentID} | Name: {dept.Name} | Employees: {dept.Employees.Count}");
                }
            }
            Pause();
        }

        static void AddEmployee(CompanyDBContext context)
        {
            Console.Write("First Name: ");
            var firstName = Console.ReadLine()?.Trim();

            Console.Write("Last Name: ");
            var lastName = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Both names are required.");
                Pause();
                return;
            }

            var departments = context.Departments.ToList();
            if (!departments.Any())
            {
                Console.WriteLine("No departments available. Please add a department first.");
                Pause();
                return;
            }

            DisplayItems(departments, "Departments");
            Console.Write("Select department: ");

            if (!TryGetSelection(departments.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var department = departments[selection - 1];
            context.Employees.Add(new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                DepartmentID = department.DepartmentID
            });

            context.SaveChanges();
            Console.WriteLine("Employee added successfully!");
            Pause();
        }

        static void EditEmployee(CompanyDBContext context)
        {
            var employees = context.Employees.Include(e => e.Department).ToList();
            if (!employees.Any())
            {
                Console.WriteLine("No employees available.");
                Pause();
                return;
            }

            DisplayEmployees(employees);
            Console.Write("Select employee to edit: ");

            if (!TryGetSelection(employees.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var employee = employees[selection - 1];
            Console.WriteLine("\n1. Edit Name");
            Console.WriteLine("2. Edit Department");
            Console.Write("Choose what to edit: ");

            var editChoice = Console.ReadLine();
            switch (editChoice)
            {
                case "1":
                    Console.Write($"First Name ({employee.FirstName}): ");
                    var newFirstName = Console.ReadLine()?.Trim();

                    Console.Write($"Last Name ({employee.LastName}): ");
                    var newLastName = Console.ReadLine()?.Trim();

                    if (!string.IsNullOrEmpty(newFirstName)) employee.FirstName = newFirstName;
                    if (!string.IsNullOrEmpty(newLastName)) employee.LastName = newLastName;
                    break;

                case "2":
                    var departments = context.Departments.ToList();
                    DisplayItems(departments, "Departments");
                    Console.Write("Select new department: ");

                    if (!TryGetSelection(departments.Count, out int deptSelection))
                    {
                        Console.WriteLine("Invalid selection.");
                        Pause();
                        return;
                    }
                    employee.DepartmentID = departments[deptSelection - 1].DepartmentID;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    Pause();
                    return;
            }

            context.SaveChanges();
            Console.WriteLine("Employee updated successfully!");
            Pause();
        }

        static void DeleteEmployee(CompanyDBContext context)
        {
            var employees = context.Employees.ToList();
            if (!employees.Any())
            {
                Console.WriteLine("No employees available.");
                Pause();
                return;
            }

            DisplayEmployees(employees);
            Console.Write("Select employee to delete: ");

            if (!TryGetSelection(employees.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var employee = employees[selection - 1];
            context.Employees.Remove(employee);
            context.SaveChanges();
            Console.WriteLine("Employee deleted successfully!");
            Pause();
        }

        static void ViewEmployees(CompanyDBContext context)
        {
            var employees = context.Employees.Include(e => e.Department).ToList();

            if (!employees.Any())
            {
                Console.WriteLine("No employees available.");
            }
            else
            {
                Console.WriteLine("\nEmployee List:");
                DisplayEmployees(employees);
            }
            Pause();
        }

        static void AddProject(CompanyDBContext context)
        {
            Console.Write("Project Name: ");
            var name = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name cannot be empty.");
                Pause();
                return;
            }

            Console.Write("Start Date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Invalid date format.");
                Pause();
                return;
            }

            Console.Write("End Date (optional, yyyy-MM-dd): ");
            var endDateInput = Console.ReadLine();
            DateTime? endDate = null;

            if (!string.IsNullOrWhiteSpace(endDateInput))
            {
                if (!DateTime.TryParse(endDateInput, out DateTime parsedEndDate))
                {
                    Console.WriteLine("Invalid date format.");
                    Pause();
                    return;
                }
                endDate = parsedEndDate;
            }

            context.Projects.Add(new Project
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate
            });

            context.SaveChanges();
            Console.WriteLine("Project added successfully!");
            Pause();
        }

        static void EditProject(CompanyDBContext context)
        {
            var projects = context.Projects.ToList();
            if (!projects.Any())
            {
                Console.WriteLine("No projects available.");
                Pause();
                return;
            }

            DisplayProjects(projects);
            Console.Write("Select project to edit: ");

            if (!TryGetSelection(projects.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var project = projects[selection - 1];
            Console.WriteLine("\n1. Edit Name");
            Console.WriteLine("2. Edit Start Date");
            Console.WriteLine("3. Edit End Date");
            Console.Write("Choose what to edit: ");

            var editChoice = Console.ReadLine();
            switch (editChoice)
            {
                case "1":
                    Console.Write($"Name ({project.Name}): ");
                    var newName = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(newName)) project.Name = newName;
                    break;

                case "2":
                    Console.Write($"Start Date ({project.StartDate:yyyy-MM-dd}): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime newStartDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        Pause();
                        return;
                    }
                    project.StartDate = newStartDate;
                    break;

                case "3":
                    Console.Write($"End Date ({(project.EndDate.HasValue ? project.EndDate.Value.ToString("yyyy-MM-dd") : "null")}): ");
                    var newEndDateInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newEndDateInput))
                    {
                        project.EndDate = null;
                    }
                    else if (!DateTime.TryParse(newEndDateInput, out DateTime newEndDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        Pause();
                        return;
                    }
                    else
                    {
                        project.EndDate = newEndDate;
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    Pause();
                    return;
            }

            context.SaveChanges();
            Console.WriteLine("Project updated successfully!");
            Pause();
        }

        static void DeleteProject(CompanyDBContext context)
        {
            var projects = context.Projects.ToList();
            if (!projects.Any())
            {
                Console.WriteLine("No projects available.");
                Pause();
                return;
            }

            DisplayProjects(projects);
            Console.Write("Select project to delete: ");

            if (!TryGetSelection(projects.Count, out int selection))
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            var project = projects[selection - 1];
            context.Projects.Remove(project);
            context.SaveChanges();
            Console.WriteLine("Project deleted successfully!");
            Pause();
        }

        static void ViewProjects(CompanyDBContext context)
        {
            var projects = context.Projects.ToList();

            if (!projects.Any())
            {
                Console.WriteLine("No projects available.");
            }
            else
            {
                Console.WriteLine("\nProject List:");
                DisplayProjects(projects);
            }
            Pause();
        }

        // Helper methods
        static void DisplayItems<T>(System.Collections.Generic.List<T> items, string title) where T : class
        {
            Console.WriteLine($"\n{title}:");
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i]}");
            }
        }

        static void DisplayEmployees(System.Collections.Generic.List<Employee> employees)
        {
            foreach (var emp in employees)
            {
                Console.WriteLine($"ID: {emp.EmployeeID} | Name: {emp.FirstName} {emp.LastName} | Department: {emp.Department?.Name ?? "None"}");
            }
        }

        static void DisplayProjects(System.Collections.Generic.List<Project> projects)
        {
            foreach (var proj in projects)
            {
                Console.WriteLine($"ID: {proj.ProjectID} | Name: {proj.Name} | " +
                    $"Dates: {proj.StartDate:yyyy-MM-dd} to {(proj.EndDate.HasValue ? proj.EndDate.Value.ToString("yyyy-MM-dd") : "Ongoing")}");
            }
        }

        static bool TryGetSelection(int max, out int selection)
        {
            return int.TryParse(Console.ReadLine(), out selection) && selection > 0 && selection <= max;
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}