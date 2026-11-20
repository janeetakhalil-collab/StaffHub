
namespace StaffDb.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Collection navigation property.
    // Represents multiple employees belonging to one department.
    // Initializes an empty collection.
    //
    // No Employees column is created in the Departments table.
    // The relationship is stored through the foreign key in Employees.
    //
    // SQL relationship:
    // FOREIGN KEY (DepartmentId)
    // REFERENCES Departments(Id)
   public ICollection<Employee> Employees {get;set;}=new List<Employee>();
   }