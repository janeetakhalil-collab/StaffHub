
using System.ComponentModel.DataAnnotations;

namespace StaffDb.DTOs;

public class EmployeeRequest
{

    [Required(ErrorMessage = "Employee name is required.")]

    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Employee name must contain 2 to 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]

    [StringLength(150,
        ErrorMessage = "Email cannot exceed 150 characters.")]

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

  
    [Range(1, int.MaxValue,
        ErrorMessage = "Please provide a valid Department ID.")]
    public int DepartmentId { get; set; }

}
