
namespace StaffDb.DTOs;

public class DepartmentResponse
{
    // SQL column:
    // Id INT IDENTITY(1,1) PRIMARY KEY
    public int Id { get; set; }

    // SQL column:
    // Name NVARCHAR(100) NOT NULL
    public string Name { get; set; } = string.Empty;
}
